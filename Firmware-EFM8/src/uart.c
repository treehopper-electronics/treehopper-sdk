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

#define TX_BIT		0x10
#define RX_BIT		0x20

#define BUFF_LEN	32
SI_SEGMENT_VARIABLE(txBuffer[BUFF_LEN],   uint8_t, SI_SEG_XDATA);
SI_SEGMENT_VARIABLE(rxBuffer[BUFF_LEN+1], uint8_t, SI_SEG_XDATA);
SI_SEGMENT_VARIABLE(temp, uint8_t, SI_SEG_XDATA);
UartConfiguration_t mode;

volatile uint8_t txBusy = 0;

void UART_Init()
{
	UART0_init(UART0_RX_ENABLE, UART0_WIDTH_8, UART0_MULTIPROC_DISABLE);
}

void UART_Enable()
{
	P0SKIP &= ~(TX_BIT | RX_BIT);
	XBR0 |= XBR0_URT0E__ENABLED;
}

void UART_Disable()
{
	P0SKIP |= TX_BIT | RX_BIT;
	XBR0 &= ~XBR0_URT0E__ENABLED;
}

void UART_SetConfig(UartConfigData_t* config)
{
	SFRPAGE = 0x00;

	mode = config->Config; // save the UART mode so we know how to interpret future commands

	switch(config->Config)
	{
	case UART_DISABLED:
		UART_Disable();
		break;


	case UART_STANDARD:
		UART_Enable();
		TH1 = config->TH1Val;

		if(config->usePrescaler)
			CKCON0 &= ~CKCON0_T1M__BMASK;
		else
			CKCON0 |= CKCON0_T1M__BMASK;

		if(config->useOpenDrain)
			P0MDOUT &= ~TX_BIT;
		else
			P0MDOUT |= TX_BIT;

		UART0_readBuffer(rxBuffer, BUFF_LEN);
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

void UART_Transaction(uint8_t* txBuff)
{
	uint8_t val;
	uint8_t i;
	uint8_t j;

	uint8_t len = txBuff[1];
	switch(txBuff[0])
	{
	case UART_CMD_TX:
		if(mode == UART_STANDARD) {
			memcpy(txBuffer, &(txBuff[2]), len); // copy to our own private buffer
			while(UART0_txBytesRemaining() > 0); // wait for any pending Tx data
			UART0_writeBuffer(txBuffer, len);

		} else if(mode == UART_ONEWIRE) {
			while(UART0_txBytesRemaining() > 0); // wait for any pending Tx data
			for(i = 0; i < len; i++)
			{
				for(j = 0; j < 8; j++)
				{
					txBuffer[8*i + j] = ((txBuff[2+i] >> j) & 0x01) > 0 ? 0xff : 0x00;
				}
			}
			txBusy = 1;
			UART0_writeBuffer(txBuffer, 8*len);
			while(txBusy);
			temp = 0xff;
			USBD_Write(EP2IN, &temp, 1, false );
		}
		break;

	case UART_CMD_RX:
		if(mode == UART_STANDARD) {
			rxBuffer[BUFF_LEN] = BUFF_LEN - UART0_rxBytesRemaining();
			USBD_Write(EP2IN, rxBuffer, BUFF_LEN+1, false );
			memset(rxBuffer, 0, BUFF_LEN);
			UART0_readBuffer(rxBuffer, BUFF_LEN);

		} else if(mode == UART_ONEWIRE) {
			IE_ES0 = 0; // disable UART interrupts
			// write 0xFFs and read it back
			// len holds the number of bytes we want to read
			for(i=0;i<len;i++)
			{
				val = 0;
				for(j = 0; j<8; j++)
				{
//					UART0_readBuffer(&temp, 1);
//					UART0_write(0xFF);
					SBUF0 = 0xff;
					while(!SCON0_TI);
					while(!SCON0_RI);
					temp = SBUF0;
//					temp = UART0_read();
					if(temp == 0xff)
						val += 1 << j;


				}
				rxBuffer[i] = val;
			}
			rxBuffer[BUFF_LEN] = len;
			IE_ES0 = 1; // enable UART interrupts
			USBD_Write(EP2IN, rxBuffer, BUFF_LEN+1, false );

		}

		break;

	case UART_CMD_ONEWIRE_RESET:
		// switch to 9600
		SFRPAGE = 0x00;
		TH1 = 48;
		CKCON0 &= ~CKCON0_T1M__BMASK;
		txBusy = 1;
		UART0_write(0xF0);
		val = UART0_read();
		while(txBusy);
		// switch back to 115200
		CKCON0 |= CKCON0_T1M__BMASK;
		USBD_Write(EP2IN, &val, 1, false );
		break;

	}

}


void UART0_transmitCompleteCb()
{
	txBusy = 0;
}

void UART0_receiveCompleteCb()
{
	// we filled up our buffer before we read everything. Oops... oh well, try again
	UART0_readBuffer(rxBuffer, BUFF_LEN);
}
