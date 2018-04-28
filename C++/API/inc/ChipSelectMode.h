#pragma once
namespace Treehopper {
	enum class ChipSelectMode
	{
		/** CS is asserted low, the SPI transaction takes place, and then the signal is returned high. */
		SpiActiveLow,

		/** CS is asserted high, the SPI transaction takes place, and then the signal is returned low. */
		SpiActiveHigh,

		/** CS is pulsed high, and then the SPI transaction takes place. */
		PulseHighAtBeginning,

		/** The SPI transaction takes place, and once finished, CS is pulsed high */
		PulseHighAtEnd,

		/** CS is pulsed low, and then the SPI transaction takes place. */
		PulseLowAtBeginning,

		/** The SPI transaction takes place, and once finished, CS is pulsed low */
		PulseLowAtEnd
	};
}