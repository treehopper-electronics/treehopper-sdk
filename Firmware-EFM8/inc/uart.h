/*
 * uart.h
 *
 *  Created on: Jul 23, 2015
 *      Author: jay
 */

#ifndef UART_H_
#define UART_H_

#include "stdint.h"
#include "uart_0.h"

typedef enum {
	UART_DISABLED, UART_STANDARD, UART_ONEWIRE
} UartConfiguration_t;

typedef enum {
	UART_CMD_TX, UART_CMD_RX, UART_CMD_ONEWIRE_RESET, UART_CMD_ONEWIRE_SEARCH
} UartCmd_t;

typedef struct {
	UartConfiguration_t Config;
	uint8_t TH1Val;
	uint8_t usePrescaler;
	uint8_t useOpenDrain;
} UartConfigData_t;

void UART_Init();
void UART_SetConfig(UartConfigData_t* config);
void UART_Transaction(uint8_t* transaction);
void UART_Enable();
void UART_Disable();
void UART_SendChar(uint8_t c);
void UART_StartDebugging115200();

static void sendUsbRxResponse(SI_VARIABLE_SEGMENT_POINTER(buffer,
        uint8_t,
        EFM8PDL_UART0_RX_BUFTYPE));

#endif /* UART_H_ */
