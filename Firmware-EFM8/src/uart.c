/*
 * uart.c
 *
 *  Created on: Jul 23, 2015
 *      Author: jay
 */

#include "uart.h"
#include "uart_0.h"
#include "efm8_usb.h"
#include <SI_EFM8UB1_Register_Enums.h>
#include "stdbool.h"

#define TX_BIT		0x10
#define RX_BIT		0x20

#define RX_BUFF_LEN	32
//extern SI_SEGMENT_VARIABLE(rxRemaining, uint8_t,  SI_SEG_XDATA); // use this if we need to make it faster
SI_SEGMENT_VARIABLE(rxBufferA[RX_BUFF_LEN+1], uint8_t, SI_SEG_IDATA);
SI_SEGMENT_VARIABLE(rxBufferB[RX_BUFF_LEN+1], uint8_t, SI_SEG_IDATA);

#define TX_BUFF_LEN 63
SI_SEGMENT_VARIABLE(txBuffer[TX_BUFF_LEN+1], uint8_t, SI_SEG_XDATA);

SI_SEGMENT_VARIABLE(temp, uint8_t, SI_SEG_XDATA);

bool rxTargetA;
bool rxBufferFull;


UartConfiguration_t mode;

volatile uint8_t txBusy = 0;

uint8_t lastDevice = false;

uint8_t oneWire_Reset();

uint8_t oneWire_readBit();
void oneWire_writeBit(uint8_t val);
void oneWire_writeByte(uint8_t val);
uint8_t oneWire_readByte();
void oneWire_FindSlaves();
uint8_t oneWire_Search(uint8_t nextNode);

void UART_Init() {
	UART0_init(UART0_RX_ENABLE, UART0_WIDTH_8, UART0_MULTIPROC_DISABLE);
}

void UART_Enable() {
	P0SKIP &= ~(TX_BIT | RX_BIT);
	XBR0 |= XBR0_URT0E__ENABLED;
}

void UART_Disable() {
	P0SKIP |= TX_BIT | RX_BIT;
	XBR0 &= ~XBR0_URT0E__ENABLED;
}

void UART_SetConfig(UartConfigData_t* config) {
	SFRPAGE = 0x00;

	mode = config->Config; // save the UART mode so we know how to interpret future commands

	switch (config->Config) {
	case UART_DISABLED:
		UART_Disable();
		break;

	case UART_STANDARD:
		UART_Enable();
		TH1 = config->TH1Val;

		if (config->usePrescaler)
			CKCON0 &= ~CKCON0_T1M__BMASK;
		else
			CKCON0 |= CKCON0_T1M__BMASK;

		if (config->useOpenDrain)
			P0MDOUT &= ~TX_BIT;
		else
			P0MDOUT |= TX_BIT;

		rxTargetA = true;
		rxBufferFull = false;
		UART0_readBuffer(rxBufferA, RX_BUFF_LEN);
		break;
	case UART_ONEWIRE:
		// 115200 baud
		TH1 = 48;
		CKCON0 |= CKCON0_T1M__BMASK;
		P0MDOUT &= ~TX_BIT; // use open drain
		UART_Enable();
		UART0_abortRead(); // just in case
		break;
	}

}

void UART_StartDebugging115200()
{
	UartConfigData_t config;
	config.Config = 1;
	config.TH1Val = 152;
	config.usePrescaler = true;
	config.useOpenDrain = false;
	UART_SetConfig(&config);
}

void UART_SendChar(uint8_t c)
{
	IE_ES0 = 0;
	SFRPAGE = 0x00;
	SCON0_TI = 0;
	SBUF0 = c;
	while (!SCON0_TI)
		;
}

void UART_Transaction(uint8_t* txBuff) {
	uint8_t val;
	uint8_t i;
	uint8_t len = txBuff[1];

	switch (txBuff[0]) {
	case UART_CMD_TX:
		if (mode == UART_STANDARD) {
//			IE_ES0 = 0; // disable UART interrupts
//			for (i = 0; i < len; i++) {
//				SCON0_TI = 0;
//				SBUF0 = txBuff[i + 2];
//				while (!SCON0_TI)
//					;
//			}
//			IE_ES0 = 1;
			UART0_writeBuffer(txBuffer, len);

			temp = 0xff;
			USBD_Write(EP2IN, &temp, 1, false);

		} else if (mode == UART_ONEWIRE) {
			IE_ES0 = 0; // disable UART interrupts
			for (i = 0; i < len; i++) {
				oneWire_writeByte(txBuff[2 + i]);
			}
			IE_ES0 = 1;
			temp = 0xff;
			USBD_Write(EP2IN, &temp, 1, false);
		}
		break;

	case UART_CMD_RX:
		if (mode == UART_STANDARD) {
			if(rxBufferFull){
				if(rxTargetA){ // currently targetting A, B is already full
					rxBufferB[RX_BUFF_LEN] = RX_BUFF_LEN;
					USBD_Write(EP2IN, rxBufferB, RX_BUFF_LEN + 1, false);
				} else {
					rxBufferA[RX_BUFF_LEN] = RX_BUFF_LEN;
					USBD_Write(EP2IN, rxBufferA, RX_BUFF_LEN + 1, false);
				}
				rxBufferFull = false;
			} else {
				if(rxTargetA){
					rxBufferA[RX_BUFF_LEN] = RX_BUFF_LEN - UART0_rxBytesRemaining();
					UART0_readBuffer(rxBufferB, RX_BUFF_LEN); // redirect incoming bytes to fresh buffer
					USBD_Write(EP2IN, rxBufferA, RX_BUFF_LEN + 1, false);
				} else {
					rxBufferB[RX_BUFF_LEN] = RX_BUFF_LEN - UART0_rxBytesRemaining();
					UART0_readBuffer(rxBufferA, RX_BUFF_LEN); // redirect incoming bytes to fresh buffer
					USBD_Write(EP2IN, rxBufferB, RX_BUFF_LEN + 1, false);
				}
			}
//			rxBufferA[BUFF_LEN] = BUFF_LEN - UART0_rxBytesRemaining();
//			USBD_Write(EP2IN, rxBufferA, BUFF_LEN + 1, false);
//			memset(rxBufferA, 0, BUFF_LEN);
//			UART0_readBuffer(rxBufferA, BUFF_LEN);

		} else if (mode == UART_ONEWIRE) {
			IE_ES0 = 0; // disable UART interrupts
			for (i = 0; i < len; i++) {
				rxBufferA[i] = oneWire_readByte();
			}
			rxBufferA[RX_BUFF_LEN] = len;
			IE_ES0 = 1; // enable UART interrupts
			USBD_Write(EP2IN, rxBufferA, RX_BUFF_LEN + 1, false);
		}

		break;

	case UART_CMD_ONEWIRE_RESET:
		IE_ES0 = 0; // disable UART interrupts
		val = oneWire_Reset();
		IE_ES0 = 1;
		USBD_Write(EP2IN, &val, 1, false);
		break;

	case UART_CMD_ONEWIRE_SEARCH:
		IE_ES0 = 0; // disable UART interrupts
		oneWire_FindSlaves();
		IE_ES0 = 1;
		break;
	}

}

void oneWire_writeByte(uint8_t val) {
	uint8_t i;
	for (i = 0; i < 8; i++) {
		oneWire_writeBit((val >> i) & 0x01);
	}
}

uint8_t oneWire_readByte() {
	uint8_t i;
	uint8_t val = 0;
	for (i = 0; i < 8; i++) {
		val |= (oneWire_readBit() << i);
	}
	return val;
}

uint8_t oneWire_readBit() {
	uint8_t temp;
	SCON0_TI = 0;
	temp = SBUF0; // just in case we have a byte in the buffer already
	SCON0_REN = 1;
	SBUF0 = 0xff;
	while (!SCON0_TI)
		;
	while (!SCON0_RI)
		;
	temp = SBUF0;
	temp = (temp == 0xff);

	if (temp)
		return true;
	else
		return false;
}

void oneWire_writeBit(uint8_t val) {
	SCON0_REN = 0;
	SCON0_TI = 0;
	if (val)
		SBUF0 = 0xff;
	else
		SBUF0 = 0x00;

	while (!SCON0_TI)
		;

}

xdata uint8_t id[8];
xdata uint8_t nextDevice;
void oneWire_FindSlaves() {
	nextDevice = 65;
	memset(id, 0, 8);

	while (nextDevice) {
		nextDevice = oneWire_Search(nextDevice);
		rxBufferA[0] = 0x00; // not done yet
		memcpy(&rxBufferA[1], id, 8);
		while (USBD_EpIsBusy(EP2IN))
			;
		USBD_Write(EP2IN, rxBufferA, 9, false);
	}

	rxBufferA[0] = 0xff; // done
	while (USBD_EpIsBusy(EP2IN))
		;
	USBD_Write(EP2IN, rxBufferA, 9, false);
}

// from https://electricimp.com/docs/resources/onewire/
uint8_t oneWire_Search(uint8_t nextNode) {
	uint8_t i;
	uint8_t byte;
	bool bitVal;
	bool complementBit;
	uint8_t lastForkPoint = 0;
	if (oneWire_Reset()) // no devices found
	{

		oneWire_writeByte(0xF0);

		for (i = 64; i > 0; i--) {
			byte = (i - 1) / 8;

			bitVal = oneWire_readBit();
			complementBit = oneWire_readBit();
			if (complementBit) {
				if (bitVal) {
					// both bits are 1 which indicates that there are no further devices on the bus
					// so put pointer back to the start and break out of the loop
					lastForkPoint = 0;
					break;
				}
			} else if (!bitVal) {
				// first and second bits are both 0: we'rd at a node
				if (nextNode > i || (nextNode != i && (id[byte] & 1))) {
					// Take the '1' direction on this point
					bitVal = 1;
					lastForkPoint = i;
				}
			}

			// Write the 'direction' bit. For example, if it's 1 then all further
			// devices with a 0 at the current ID bit location will go offline
			oneWire_writeBit(bitVal);

			// Write the bit to the current ID record
			id[byte] = (id[byte] >> 1) + 0x80 * bitVal;
		}
	}

	return lastForkPoint;
}

uint8_t oneWire_Reset() {
	uint8_t val;
	// switch to 9600
	SFRPAGE = 0x00;
	TH1 = 48;
	CKCON0 &= ~CKCON0_T1M__BMASK;

	SCON0_TI = 0;
	SCON0_REN = 1;
	SBUF0 = 0xf0;
	while (!SCON0_TI)
		;
	while (!SCON0_RI)
		;
	val = SBUF0;
	// switch back to 115200
	CKCON0 |= CKCON0_T1M__BMASK;
	return (val != 0xf0);
}

void UART0_transmitCompleteCb() {
	txBusy = 0;
}

void UART0_receiveCompleteCb() {
	if(mode == UART_STANDARD){
		rxBufferFull = true;
		if(rxTargetA){
			rxTargetA = false;
			UART0_readBuffer(rxBufferB, RX_BUFF_LEN);
		} else {
			rxTargetA = true;
			UART0_readBuffer(rxBufferA, RX_BUFF_LEN);
		}
	} else {
		UART0_readBuffer(rxBufferA, RX_BUFF_LEN);
	}
}
