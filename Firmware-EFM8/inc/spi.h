/*
 * spi.h
 *
 *  Created on: Jul 23, 2015
 *      Author: jay
 */

#ifndef SPI_H_
#define SPI_H_

#include "SI_EFM8UB1_Register_Enums.h"

typedef struct SpiConfigData {
	uint8_t IsEnabled;
	uint8_t SpiMode;
	uint8_t CkrVal;
	uint8_t CsPin;
	uint8_t CsPolarity;
} SpiConfigData_t;

void SPI_Init();
void SPI_SetConfig(SpiConfigData_t* config);
void SPI_Transaction(uint8_t* dataToSend, uint8_t* dataToReceive, uint8_t count);
void SPI_Enable();
void SPI_Disable();
void SPI_ActivateCs();
void SPI_DeactivateCs();

#endif /* SPI_H_ */
