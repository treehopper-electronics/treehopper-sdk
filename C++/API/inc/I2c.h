#pragma once

#include "Treehopper.h"
#include <stdint.h>
#include <cstddef>
#include <vector>

namespace Treehopper {
    /** Base I2c interface */
    class TREEHOPPER_API I2c {
    public:
        virtual ~I2c() {}

        /** Set the speed, in kHz, to use. */
        virtual void speed(double value) = 0;

        /** Gets the current speed, in kHz. */
        virtual double speed() = 0;

        /** Sets whether the module is enabled */
        virtual void enabled(bool value) = 0;

        /** Gets whether the module is enabled */
        virtual bool enabled() = 0;

        /** Send and/or receive data with the i2c module.
        @param[in] address the address of the slave i2c board you wish to communicate with
        @param[in] data a vector containing the bytes to write
        @param[in] numBytesToRead the number of bytes to read from the board
        @returns the read data, or nullptr.
        */
        virtual std::vector<uint8_t> sendReceive(uint8_t address, std::vector<uint8_t> data,
                                                 size_t numBytesToRead = 0) = 0;
    };
}