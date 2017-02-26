#pragma once
#include "Treehopper.h"
#include "ChipSelectMode.h"
#include "SpiBurstMode.h"
#include "SpiMode.h"
#include <stdint.h>

namespace Treehopper {
	class SpiChipSelectPin;

	/** An SPI port */
	class Spi
	{
	public:
		/** Gets whether the port is enabled */
		virtual bool enabled() = 0;

		/** Sets whether the port is enabled */
		virtual void enabled(bool) = 0;

		/** Send/receive data out of this SPI port 
		
		\param[in] dataToWrite		Pointer to data to send, or NULL
		\param[in] numBytesToWrite	The number of bytes to write
		\param[out] readBuffer		Pointer to buffer to store read data, or NULL. If not NULL, must be large enough to store numBytesToWrite data.
		\param[in] chipSelect		pin to use as chip-select, or NULL
		\param[in] chipSelectMode	The ChipSelectMode to use if chipSelect != NULL
		\param[in] speed			The speed, in MHz, to clock the data at
		\param[in] burstMode		The burst mode to use
		\param[in] spiMode			The SPI mode to use
		*/
		virtual void sendReceive(
			uint8_t* dataToWrite,
			int	numBytesToWrite,
			uint8_t* readBuffer,
			SpiChipSelectPin* chipSelect = NULL,
			ChipSelectMode chipSelectMode = ChipSelectMode::SpiActiveLow,
			double speed = 1,
			SpiBurstMode burstMode = SpiBurstMode::NoBurst,
			SpiMode spiMode = SpiMode::Mode00
		) = 0;
	};
}
