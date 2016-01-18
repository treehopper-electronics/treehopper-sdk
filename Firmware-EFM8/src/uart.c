/*
 * uart.c
 *
 *  Created on: Jul 23, 2015
 *      Author: jay
 */

#include "uart.h"
#include "uart_0.h"
#include <SI_EFM8UB1_Register_Enums.h>

#define TX_BIT		0x10
#define RX_BIT		0x20

static SI_VARIABLE_SEGMENT_POINTER(rxBuffer[16], uint8_t, SI_SEG_XDATA);

void UART_Init()
{

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

void UART_SetConfig()
{
	UART0_init(UART0_RX_ENABLE, UART0_WIDTH_8, UART0_MULTIPROC_DISABLE);
	UART0_readBuffer(rxBuffer, 16);

}


void UART0_transmitCompleteCb()
{

}

void UART0_receiveCompleteCb()
{
	// we filled up our buffer before we read everything. Oops... oh well, try again
	UART0_readBuffer(rxBuffer, 16);
}
