#pragma once

#include "I2c.h"

namespace Treehopper {
    class TreehopperUsb;

    /** The built-in I2c interface */
    class TREEHOPPER_API HardwareI2c : public I2c {
    public:
        explicit HardwareI2c(TreehopperUsb &board);

        ~HardwareI2c() override;

        void speed(double value) override;

        double speed() override;

        void enabled(bool value) override;

        bool enabled() override;

        std::vector<uint8_t> sendReceive(uint8_t address, std::vector<uint8_t> data, size_t numBytesToRead = 0) override;

    private:
        TreehopperUsb &board;
        double _speed;
        bool _enabled;

        void sendConfig();
    };
}