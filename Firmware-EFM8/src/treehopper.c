/*
 * treehopper.c
 *
 *  Created on: Jul 19, 2015
 *      Author: jay
 */

#include "Treehopper.h"
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

// GLOBALS
volatile bit PeripheralConfigPacketReady;
volatile bit PinConfigPacketReady;
volatile bit SendPinStatusPacket;

SI_SEGMENT_VARIABLE( Treehopper_ReportData[TREEHOPPER_NUM_PINS*2+1], uint8_t, SI_SEG_XDATA);
SI_SEGMENT_VARIABLE(Treehopper_PinConfig, pinConfigPacket_t, SI_SEG_XDATA);
SI_SEGMENT_VARIABLE( Treehopper_PeripheralConfig[64], uint8_t, SI_SEG_XDATA);

SI_SEGMENT_VARIABLE( Treehopper_TxBuffer[255], uint8_t, SI_SEG_XDATA);
SI_SEGMENT_VARIABLE( Treehopper_RxBuffer[255], uint8_t, SI_SEG_XDATA);

// PROTOTYPES
void ProcessPeripheralConfigPacket();
void ProcessPinConfigPacket();
void SendPinStatus();

// LOCALS
uint8_t pins[TREEHOPPER_NUM_PINS];

void Treehopper_Init() {
	memset(&Treehopper_ReportData, 0, 64);
}

void Treehopper_Task() {
	if (PinConfigPacketReady) {
		ProcessPinConfigPacket();
		PinConfigPacketReady = false;
	}
	if (PeripheralConfigPacketReady) {
		ProcessPeripheralConfigPacket();
		PeripheralConfigPacketReady = false;
	}

	if (SendPinStatusPacket) {
		SendPinStatus();
		SendPinStatusPacket = false;
	}
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
			Treehopper_ReportData[i * 2 + 1] = 0;
			Treehopper_ReportData[i * 2 + 2] = 0;
		}
	}
	USBD_Write(EP1IN, &Treehopper_ReportData, sizeof(Treehopper_ReportData), true);
}

void ProcessPinConfigPacket() {
	switch (Treehopper_PinConfig.PinCommand) {
	case PinConfig_MakeDigitalInput:
		pins[Treehopper_PinConfig.PinNumber] = DigitalInput;
		GPIO_MakeInput(Treehopper_PinConfig.PinNumber, true);
		break;
	case PinConfig_MakeAnalogInput:
		pins[Treehopper_PinConfig.PinNumber] = AnalogInput;
		ADC_Enable(Treehopper_PinConfig.PinNumber,
				Treehopper_PinConfig.PinConfigData[0]);
		break;
	case PinConfig_MakePushPullOutput:
		pins[Treehopper_PinConfig.PinNumber] = PushPullOutput;
		GPIO_MakeOutput(Treehopper_PinConfig.PinNumber, PushPullOutput);
		break;
	case PinConfig_MakeOpenDrainOutput:
		pins[Treehopper_PinConfig.PinNumber] = OpenDrainOutput;
		GPIO_MakeOutput(Treehopper_PinConfig.PinNumber, OpenDrainOutput);
		break;
	case PinConfig_SetDigitalValue:
		GPIO_WriteValue(Treehopper_PinConfig.PinNumber,
				Treehopper_PinConfig.PinConfigData[0]);
		break;
	}
}

// this gets called whenever we received peripheral config data from the host
void ProcessPeripheralConfigPacket() {
	uint16_t i;
	uint8_t totalWriteBytes;
	uint8_t totalReadBytes;
	uint8_t offset;
	uint8_t count;
	switch (Treehopper_PeripheralConfig[0]) {
	case PWMConfig:
		PWM_SetConfig(&(Treehopper_PeripheralConfig[1]));
		break;

	case LedConfig:
		LED_SetVal(Treehopper_PeripheralConfig[1]);
		break;

	case SPIConfig:
		SPI_SetConfig((SpiConfigData_t*) &(Treehopper_PeripheralConfig[1]));
		break;
	case SPITransaction:
		totalWriteBytes = Treehopper_PeripheralConfig[1];
		offset = Treehopper_PeripheralConfig[2];
		count = Treehopper_PeripheralConfig[3];
		memcpy(Treehopper_TxBuffer, &(Treehopper_PeripheralConfig[5]), count);

		// check whether we're done copying, or if we don't care about Tx data
		if (totalWriteBytes == offset + count
				|| Treehopper_PeripheralConfig[4] == Burst_Rx) {
			SPI_ActivateCs();
			SPI_Transaction(Treehopper_TxBuffer, Treehopper_RxBuffer,
					totalWriteBytes);
			SPI_DeactivateCs();
		}
		// if we're doing a Tx burst, we don't care about Rx data -- don't bother sending it
		if (Treehopper_PeripheralConfig[4] != Burst_Tx) {
			USBD_Write(EP2IN, Treehopper_RxBuffer, totalWriteBytes, false);
		}
		break;

	case I2CConfig:
		I2C_SetConfig((I2cConfigData_t*) &(Treehopper_PeripheralConfig[1]));
		break;
	case I2CTransaction:
		totalWriteBytes = Treehopper_PeripheralConfig[2];
		offset = Treehopper_PeripheralConfig[3];
		count = Treehopper_PeripheralConfig[4];
		totalReadBytes = Treehopper_PeripheralConfig[5];
		memcpy(Treehopper_TxBuffer, &(Treehopper_PeripheralConfig[7]), count);

		// check whether we're done copying, or if we don't care about Tx data
		if (totalWriteBytes == offset + count
				|| Treehopper_PeripheralConfig[6] == Burst_Rx) {
			I2C_Transaction(Treehopper_PeripheralConfig[1], Treehopper_TxBuffer,
					Treehopper_RxBuffer, totalWriteBytes, totalReadBytes);
		}
		// if we're doing a Tx burst, we don't care about Rx data -- don't bother sending it
		if (Treehopper_PeripheralConfig[6] != Burst_Tx && totalReadBytes > 0) {
			USBD_Write(EP2IN, Treehopper_RxBuffer, totalReadBytes, false);
		}
		break;

	case UARTConfig:
		UART_SetConfig((UartConfigData_t*) &(Treehopper_PeripheralConfig[1]));
		break;

	case UARTTransaction:
		UART_Transaction(&(Treehopper_PeripheralConfig[1]));
		break;
	case FirmwareUpdateSerial:
		serialNumber_update(&(Treehopper_PeripheralConfig[2]),
				Treehopper_PeripheralConfig[1]);
		break;
	case FirmwareUpdateName:
		serialNumber_updateName(&(Treehopper_PeripheralConfig[2]),
				Treehopper_PeripheralConfig[1]);
		break;
	case SoftPwmConfig:
		SoftPwm_SetConfig(&(Treehopper_PeripheralConfig[2]),
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
	}
	// when we're all done, re-arm the endpoint.
	USBD_Read(EP2OUT, &Treehopper_PeripheralConfig, 64, true);

}
