/*
 * spi.h
 *
 *  Created on: Jul 23, 2015
 *      Author: jay
 */

#ifndef SPI_H_
#define SPI_H_

#include "SI_EFM8UB1_Register_Enums.h"


typedef enum ChipSelectMode
{
	 /// <summary>
	/// CS is asserted low, the SPI transaction takes place, and then the signal is returned high.
	/// </summary>
	CsMode_SpiActiveLow,
	/// <summary>
	/// CS is asserted high, the SPI transaction takes place, and then the signal is returned low.
	/// </summary>
	CsMode_SpiActiveHigh,

	/// <summary>
	/// CS is pulsed high, and then the SPI transaction takes place.
	/// </summary>
	CsMode_PulseHighAtBeginning,

	/// <summary>
	/// The SPI transaction takes place, and once finished, CS is pulsed high
	/// </summary>
	CsMode_PulseHighAtEnd,

	/// <summary>
	/// CS is pulsed low, and then the SPI transaction takes place.
	/// </summary>
	CsMode_PulseLowAtBeginning,

	/// <summary>
	/// The SPI transaction takes place, and once finished, CS is pulsed low
	/// </summary>
	CsMode_PulseLowAtEnd

} ChipSelectMode_t;


typedef struct SpiConfigData {
	uint8_t IsEnabled;
	uint8_t SpiMode;
	uint8_t CkrVal;
	uint8_t CsPin;
	ChipSelectMode_t CsMode;
} SpiConfigData_t;

void SPI_Init();
void SPI_SetConfig(SpiConfigData_t* config);
void SPI_Transaction(uint8_t* dataToSend, uint8_t* dataToReceive, uint8_t count);
void SPI_Enable();
void SPI_Disable();
void SPI_ActivateCs();
void SPI_DeactivateCs();

#endif /* SPI_H_ */
