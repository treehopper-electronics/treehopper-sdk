#pragma once
#include "Treehopper.h"
#include "Spi.h"

namespace Treehopper
{
	class TreehopperUsb;

	class TREEHOPPER_API HardwareSpi : public Spi
	{
	public:
		HardwareSpi(TreehopperUsb& board);
		virtual bool enabled();
		virtual void enabled(bool);
		virtual void sendReceive(
			uint8_t* dataToWrite,
			int	numBytesToWrite,
			uint8_t* readBuffer,
			SpiChipSelectPin* chipSelect = NULL,
			ChipSelectMode chipSelectMode = ChipSelectMode::SpiActiveLow,
			double speed = 1,
			SpiBurstMode burstMode = SpiBurstMode::NoBurst,
			SpiMode spiMode = SpiMode::Mode00
		);
	private:
		TreehopperUsb& board;
		bool _enabled;
	};
}
