/*
 * treehopper.c
 *
 *  Created on: Jul 19, 2015
 *      Author: jay
 */

#include <SI_EFM8UB1_Register_Enums.h>
#include <efm8_usb.h>
#include "descriptors.h"
#include "treehopper.h"
#include "gpio.h"
#include "adc.h"
#include "pwm.h"
#include "led.h"
#include "spi.h"
#include "i2c.h"
#include "serialNumber.h"
#include "softPwm.h"
#include "uart.h"
#include "parallel.h"

// GLOBALS
SI_SEGMENT_VARIABLE( Treehopper_ReportData[TREEHOPPER_NUM_PINS*2+1], uint8_t, SI_SEG_XDATA);
SI_SEGMENT_VARIABLE( lastReportData[TREEHOPPER_NUM_PINS*2+1], uint8_t, SI_SEG_XDATA);
SI_SEGMENT_VARIABLE(Treehopper_PinConfig, pinConfigPacket_t, SI_SEG_XDATA);
SI_SEGMENT_VARIABLE( Treehopper_PeripheralConfig[262], uint8_t, SI_SEG_XDATA); // 255+7 bytes for larger SPI header.
SI_SEGMENT_VARIABLE( Treehopper_RxBuffer[255], uint8_t, SI_SEG_XDATA);

// PROTOTYPES
void ProcessPeripheralConfigPacket();
void ProcessPinConfigPacket();
void SendPinStatus();

uint16_t timeout = 0;

// LOCALS
SI_SEGMENT_VARIABLE(pins[TREEHOPPER_NUM_PINS], uint8_t, SI_SEG_XDATA);

void Treehopper_Init() {
	int i;
	// re-init all the buffers
	memset(Treehopper_ReportData, 0, sizeof(Treehopper_ReportData));
	memset(lastReportData, 0, sizeof(lastReportData));
	memset(&Treehopper_PinConfig, 0, sizeof(pinConfigPacket_t));
	memset(Treehopper_PeripheralConfig, 0, sizeof(Treehopper_PeripheralConfig));

	memset(Treehopper_RxBuffer, 0, sizeof(Treehopper_RxBuffer));

	SerialNumber_Init();
	LED_Init();
	PWM_Init();
	SPI_Disable();
	UART_Disable();
	I2C_Disable();
	PWM_Disable();
	SoftPwm_Init();

	for(i=0; i<TREEHOPPER_NUM_PINS;i++)
	{
		GPIO_MakeInput(i, true);

		// While we want all the pins to be weakly-pulled-up inputs by default, we don't actually want to send
		// input reports on these pins, since this unnecessarily calls GPIO_ReadValue() continuously on each pin.
		//
		// More importantly, if they're DigitalInputs, it prevents missing the first pin change when it *is* a digital input,
		// since SendPinStatus() will certainly send at least once before we receive any pin config packets
		// (and thus, get discarded by an unexpecting host API that doesn't think any pins are DigitalInputs).
		// Since SendPinStatus() only sends when pins change, we'll never get our initial digital value to the host.
		//
		// To remedy this, make the inputs anything other than an input so that SendPinStatus() sends (and records) 0xff
		// current values. Then, when the pin mode is changed to a DigitalInput, we're guaranteed to get a pin report packet
		// flushed out (since the high byte of a DigitalInput report is always 0, and never 0xff).
		pins[i] = ReservedPin;

	}

}

void configureDevice(uint8_t config) {
	// we may have to pass specific configurations in the future, but for now, just re-init everything
	Treehopper_Init();
}

void Treehopper_Task() {
	DEBUG_HIGH();
	if (!USBD_EpIsBusy(EP_PinConfig)) {
		ProcessPinConfigPacket();
	}
	if (!USBD_EpIsBusy(EP_PeripheralConfig)) {
		ProcessPeripheralConfigPacket();
	}
	if(!USBD_EpIsBusy(EP_PinStatus))
		SendPinStatus();
	DEBUG_LOW();
}

void SendPinStatus() {
	uint8_t i = 0;
	uint16_t val;
	Treehopper_ReportData[0] = DeviceResponse_CurrentReadings;
	for (i = 0; i < 20; i++) {
		switch (pins[i]) {
		case DigitalInput:
			Treehopper_ReportData[i * 2 + 1] = GPIO_ReadValue(i);
			Treehopper_ReportData[i * 2 + 2] = 0;
			break;
		case AnalogInput:
			val = ADC_GetVal(i);
			Treehopper_ReportData[i * 2 + 1] = val >> 8;
			Treehopper_ReportData[i * 2 + 2] = val & 0xFF;
			break;
		default:
			Treehopper_ReportData[i * 2 + 1] = 0xff;
			Treehopper_ReportData[i * 2 + 2] = 0xff;
		}
	}
	// if the pins have changed, send the update. Otherwise no need!
	if(memcmp(lastReportData, Treehopper_ReportData, sizeof(Treehopper_ReportData)) != 0)
	{
		USBD_Write(EP1IN, Treehopper_ReportData, sizeof(Treehopper_ReportData), false);

		// save the old report so we can compare to it next time around to see if anything has changed
		memcpy(lastReportData, Treehopper_ReportData, sizeof(Treehopper_ReportData));
	}
}

void ProcessPinConfigPacket() {
	switch (Treehopper_PinConfig.PinCommand) {
		case PinConfig_MakeDigitalInput:
			GPIO_MakeInput(Treehopper_PinConfig.PinNumber, true);
			break;
		case PinConfig_MakeAnalogInput:
			ADC_Enable(Treehopper_PinConfig.PinNumber,
					Treehopper_PinConfig.PinConfigData[0]);
			break;
		case PinConfig_MakePushPullOutput:
			GPIO_MakeOutput(Treehopper_PinConfig.PinNumber, PushPullOutput);
			break;
		case PinConfig_MakeOpenDrainOutput:
			GPIO_MakeOutput(Treehopper_PinConfig.PinNumber, OpenDrainOutput);
			break;
		case PinConfig_SetDigitalValue:
			GPIO_WriteValue(Treehopper_PinConfig.PinNumber,
					Treehopper_PinConfig.PinConfigData[0]);
			break;
		}

	memset(&Treehopper_PinConfig, 0, sizeof(pinConfigPacket_t)); // reset the buffer to zero to avoid accidentally re-processing data
	// when we're all done, re-arm the endpoint.
	USBD_Read(EP_PinConfig, (uint8_t *)&Treehopper_PinConfig, sizeof(pinConfigPacket_t), false);
}

// this gets called whenever we received peripheral config data from the host
void ProcessPeripheralConfigPacket() {
	uint8_t totalTransactionBytes;
	uint8_t totalReadBytes;
	uint8_t burst;
	SpiConfigData_t spiConfig;
	switch (Treehopper_PeripheralConfig[0]) {
	case ConfigureDevice:
		configureDevice(Treehopper_PeripheralConfig[1]);
		break;

	case PWMConfig:
		PWM_SetConfig(&(Treehopper_PeripheralConfig[1]));
		break;

	case LedConfig:
		LED_SetVal(Treehopper_PeripheralConfig[1]);
		break;

	case SPIConfig:
		SPI_SetConfig(Treehopper_PeripheralConfig[1]);
		break;
	case SPITransaction:
		spiConfig.CsPin = Treehopper_PeripheralConfig[1];
		spiConfig.CsMode = Treehopper_PeripheralConfig[2];
		spiConfig.CkrVal = Treehopper_PeripheralConfig[3];
		spiConfig.SpiMode = Treehopper_PeripheralConfig[4];

		burst = Treehopper_PeripheralConfig[5];
		totalTransactionBytes = Treehopper_PeripheralConfig[6];

		// check to see if we need to fetch more bytes for large transactions
		if(burst != Burst_Rx && totalTransactionBytes > 64-7 )
		{
			timeout = 0;
			// we can request all the remaining bytes at once; just hang in while() until they all come in.
			USBD_Read(EP_PeripheralConfig, &Treehopper_PeripheralConfig[64], (totalTransactionBytes+7)-64, false);
			while(timeout++ < 65000 && USBD_EpIsBusy(EP_PeripheralConfig));
			USBD_AbortTransfer(EP_PeripheralConfig);
		}

		SPI_Transaction(&spiConfig, totalTransactionBytes, &Treehopper_PeripheralConfig[7], Treehopper_RxBuffer);

		// if we're doing a Tx burst, we don't care about Rx data -- don't bother sending it
		if (burst != Burst_Tx) {
			timeout = 0;
			USBD_Write(EP_PeripheralResponse, Treehopper_RxBuffer, totalTransactionBytes, false);
			while(timeout++ < 65000 && USBD_EpIsBusy(EP_PeripheralResponse));
			USBD_AbortTransfer(EP_PeripheralResponse);
		}
		break;

	case I2CConfig:
		I2C_SetConfig((I2cConfigData_t*) &(Treehopper_PeripheralConfig[1]));
		break;

	case I2CTransaction:
		totalTransactionBytes = Treehopper_PeripheralConfig[2];
		totalReadBytes = Treehopper_PeripheralConfig[3];

		if(totalTransactionBytes > 64-4)
		{
			timeout = 0;
			USBD_Read(EP_PeripheralConfig, &Treehopper_PeripheralConfig[64], (totalTransactionBytes+4)-64, false);
			while(timeout++ < 65000 && USBD_EpIsBusy(EP_PeripheralConfig));
			USBD_AbortTransfer(EP_PeripheralConfig);
		}

		I2C_Transaction(Treehopper_PeripheralConfig[1], &Treehopper_PeripheralConfig[4],
						Treehopper_RxBuffer, totalTransactionBytes, totalReadBytes);

		timeout = 0;
		USBD_Write(EP2IN, Treehopper_RxBuffer, totalReadBytes+1, false);
		while(timeout++ < 65000 && USBD_EpIsBusy(EP_PeripheralResponse));
		USBD_AbortTransfer(EP_PeripheralResponse);
		break;

	case UARTConfig:
		UART_SetConfig((UartConfigData_t*) &(Treehopper_PeripheralConfig[1]));
		break;

	case UARTTransaction:
		UART_Transaction(&(Treehopper_PeripheralConfig[1]));
		break;

	case FirmwareUpdateSerial:
		SerialNumber_update(&(Treehopper_PeripheralConfig[2]),
				Treehopper_PeripheralConfig[1]);
		break;

	case FirmwareUpdateName:
		SerialNumber_updateName(&(Treehopper_PeripheralConfig[2]),
				Treehopper_PeripheralConfig[1]);
		break;

	case SoftPwmConfig:
		SoftPwm_SetConfig((softPwmPinConfig_t*)&(Treehopper_PeripheralConfig[2]),
				Treehopper_PeripheralConfig[1]);
		break;

	case Reboot:
		USBD_Stop();
		SFRPAGE = 0x00;
		RSTSRC = RSTSRC_SWRSF__SET | RSTSRC_PORSF__SET;
		break;

	case EnterBootloader:
		USBD_Stop();
		*((uint8_t SI_SEG_DATA *) 0x00) = 0xA5;
		SFRPAGE = 0x00;
		RSTSRC = RSTSRC_SWRSF__SET | RSTSRC_PORSF__SET;
		break;

	case ParallelConfig:
		Parallel_SetConfig(&(Treehopper_PeripheralConfig[1]));
		break;

	case ParallelTransaction:
		Parallel_Transaction(&(Treehopper_PeripheralConfig[1]));
		break;
	}

	memset(Treehopper_PeripheralConfig, 0, sizeof(Treehopper_PeripheralConfig)); // reset the buffer to zero to avoid accidentally re-processing data
	// when we're all done, re-arm the endpoint.
	USBD_Read(EP_PeripheralConfig, Treehopper_PeripheralConfig, 64, false);
}
