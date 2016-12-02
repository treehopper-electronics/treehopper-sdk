/*
 * i2c.c
 *
 *  Created on: Jul 23, 2015
 *      Author: jay
 */

#include <SI_EFM8UB1_Register_Enums.h>
#include "i2c_0.h"
#include "i2c.h"
#define SCL_BIT		0x40
#define SDA_BIT		0x8

volatile bit commandComplete;

I2C0_TransferError_t transferError;

void I2C_Init() {

}

void I2C_SetConfig(I2cConfigData_t* config) {
	if (config->IsEnabled) {
		SFRPAGE = 0x00;
		TH0 = config->TH0Val;
		I2C0_reset();
		I2C_Enable();
		I2C0_init(I2C0_TIMER0, true);
	} else {
		I2C_Disable();
	}
}

void I2C_Enable() {
	P0SKIP &= ~(SCL_BIT | SDA_BIT);
	XBR0 |= XBR0_SMB0E__ENABLED;
}

void I2C_Disable() {
	P0SKIP |= SCL_BIT | SDA_BIT;
	XBR0 &= ~XBR0_SMB0E__ENABLED;
}

void I2C_Transaction(uint8_t address, uint8_t* tx, uint8_t* rx, uint8_t txlen,
		uint8_t rxlen) {
	if(txlen > 0) {
		transferError = -1;
		commandComplete = 0;
		I2C0_transfer(address << 1, tx, NULL, txlen, 0);
		while (!commandComplete)
			;

		rx[0] = transferError;
	}

	if(rxlen > 0 & transferError != -1) {
		transferError = -1;
		commandComplete = 0;
		I2C0_transfer(address << 1 | 1, NULL, &rx[1], 0, rxlen);
		while (!commandComplete)
			;
		rx[0] = transferError;
	}
}
void I2C0_commandReceivedCb() {

}

void I2C0_transferCompleteCb() {
	commandComplete = 1;
}

void I2C0_errorCb(I2C0_TransferError_t error) {
	commandComplete = 1;
	transferError = error;
}
