#pragma once

#include "Treehopper.h"
#include "Spi.h"

namespace Treehopper {
    class TreehopperUsb;

    class TREEHOPPER_API HardwareSpi : public Spi {
    public:
        explicit HardwareSpi(TreehopperUsb &board);

        bool enabled() override;

        void enabled(bool) override;

        void sendReceive(
                uint8_t *dataToWrite,
                int numBytesToWrite,
                uint8_t *readBuffer,
                SpiChipSelectPin *chipSelect,
                ChipSelectMode chipSelectMode,
                double speed,
                SpiBurstMode burstMode,
                SpiMode spiMode
        ) override;

    private:
        TreehopperUsb &board;
        bool _enabled;
    };
}
