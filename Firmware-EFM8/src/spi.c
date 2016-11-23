/*
 * spi.c
 *
 *  Created on: Jul 23, 2015
 *      Author: jay
 */

#include "spi.h"
#include <SI_EFM8UB1_Register_Enums.h>
#include "spi_0.h"
#include "gpio.h"

#define SCK_BIT		1
#define MISO_BIT	2
#define MOSI_BIT	4

uint8_t csPin;
uint8_t csPolarity;

void SPI_Init() {

}

void SPI_Transaction(uint8_t* dataToSend, uint8_t* dataToReceive, uint8_t count) {
	SPI0_pollTransfer(dataToSend, dataToReceive, SPI0_TRANSFER_RXTX, count);
}

void SPI_SetConfig(SpiConfigData_t* config) {
	uint8_t spiMode;
	csPin = config->CsPin;
	csPolarity = config->CsPolarity;
	switch (config->SpiMode) {
	case 0:
		spiMode = SPI0_CLKMODE_0;
		break;
	case 1:
		spiMode = SPI0_CLKMODE_1;
		break;
	case 2:
		spiMode = SPI0_CLKMODE_2;
		break;
	case 3:
		spiMode = SPI0_CLKMODE_3;
		break;
	}

	if (config->IsEnabled) {
		SFRPAGE = 0x20;
		SPI0CKR = config->CkrVal;
		SPI0_init(spiMode, true, false);
		SPI_Enable();
		if (csPin != 0) {
			GPIO_MakeOutput(csPin, PushPullOutput);
			GPIO_WriteValue(csPin, !csPolarity);
		}

	} else
		SPI_Disable();

}

void SPI_Enable() {
	P0MDOUT |= SCK_BIT | MISO_BIT | MOSI_BIT;
	P0SKIP &= ~(SCK_BIT | MISO_BIT | MOSI_BIT);

	// which pins should be open-drain vs push-pull?
	XBR0 |= XBR0_SPI0E__ENABLED;
}

void SPI_Disable() {
	P0SKIP |= SCK_BIT | MISO_BIT | MOSI_BIT;
	P0MDOUT &= ~(SCK_BIT | MISO_BIT | MOSI_BIT);
	XBR0 &= ~XBR0_SPI0E__ENABLED;
}

void SPI_ActivateCs() {
	if (csPin != 0)
		GPIO_WriteValue(csPin, csPolarity);
}

void SPI_DeactivateCs() {
	if (csPin != 0)
		GPIO_WriteValue(csPin, !csPolarity);
}
