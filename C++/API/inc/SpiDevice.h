#pragma once

#include "Treehopper.h"
#include "Spi.h"
#include "SpiChipSelectPin.h"

namespace Treehopper {
/** Represents a peripheral on the SPI bus.

This class is essentially used to "save your settings" (chip select, clock rate, SPI mode, etc) when it comes to multi-peripheral projects. It was more useful before all these settings ended up as params in Spi.sendReceive, but it's still nice to use this as a container; SPI peripheral drivers can pass values directly from the constructor to this class's constructor, and never worry about that stuff again.
*/
    class SpiDevice {
    public:
        /// <summary>
        /// Construct an SPI device attached to a particular module
        /// </summary>
        /// <param name="spiModule">The module this device is attached to</param>
        /// <param name="chipSelect">The chip select pin used by this device</param>
        /// <param name="chipSelectMode">The ChipSelectMode to use with this device</param>
        /// <param name="speedMhz">The speed to operate this device at</param>
        /// <param name="mode">The SpiMode of this device</param>
        SpiDevice(Spi &spiModule, SpiChipSelectPin *chipSelect,
                  ChipSelectMode chipSelectMode = ChipSelectMode::SpiActiveLow, double speedMhz = 1,
                  SpiMode mode = SpiMode::Mode00)
                : spi(spiModule), chipSelect(chipSelect), chipSelectMode(chipSelectMode), frequency(speedMhz),
                  mode(mode) {
            chipSelect->makePushPullOutput();
            spi.enabled(true);
        }

        /** Starts an SPI send/receive transaction
        \param[in] dataToSend	The transmit buffer where the data is
        \param[in] burst	The SPI burst mode to use when performing this transaction
        \returns            A vector of received data.
        */
        std::vector<uint8_t> sendReceive(std::vector<uint8_t> dataToSend, SpiBurstMode burst) {
            return spi.sendReceive(dataToSend, chipSelect, chipSelectMode, frequency, burst, mode);
        }

    private:
        Spi &spi;
        ChipSelectMode chipSelectMode;
        SpiChipSelectPin *chipSelect;
        double frequency;
        SpiMode mode;

    };
}