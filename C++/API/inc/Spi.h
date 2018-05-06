#pragma once

#include "Treehopper.h"
#include "ChipSelectMode.h"
#include "SpiBurstMode.h"
#include "SpiMode.h"
#include <stdint.h>
#include <cstddef>
#include <vector>

namespace Treehopper {
    class SpiChipSelectPin;

    /** An SPI port */
    class Spi {
    public:
        /** Gets whether the port is enabled */
        virtual bool enabled() = 0;

        /** Sets whether the port is enabled */
        virtual void enabled(bool) = 0;

        /** Send/receive data out of this SPI port

        \param[in] dataToWrite		vector of input data
        \param[in] chipSelect		pin to use as chip-select, or nullptr. Defaults to nullptr.
        \param[in] chipSelectMode	The ChipSelectMode to use if chipSelect != nullptr. Defaults to ChipSelectMode::SpiActiveLow.
        \param[in] speed			The speed, in MHz, to clock the data at. Defaults to 6.
        \param[in] burstMode		The burst mode to use. Defaults to BurstMode::NoBurst.
        \param[in] spiMode			The SPI mode to use. Defaults to SpiMode::Mode00.
        \returns                    A std::vector<uint8_t> of the received data
        */
        virtual std::vector<uint8_t> sendReceive(std::vector<uint8_t> dataToWrite, SpiChipSelectPin *chipSelect = nullptr,
                                                 ChipSelectMode chipSelectMode = ChipSelectMode::SpiActiveLow, double speed = 6,
                                                 SpiBurstMode burstMode = SpiBurstMode::NoBurst, SpiMode spiMode=SpiMode::Mode00) = 0;
    };
}
