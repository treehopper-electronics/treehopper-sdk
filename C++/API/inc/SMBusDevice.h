#pragma once

#include <vector>
#include "I2c.h"

namespace Treehopper {
    class TREEHOPPER_API SMBusDevice {
    public:
        SMBusDevice(uint8_t address, I2c &i2cModule, int rateKHz = 100);

        ~SMBusDevice();

        /**
         * Read a byte from the device
         * @return the byte read
         */
        uint8_t readByte();

        /**
         * Write a byte to the device
         * @param data the byte to write
         */
        void writeByte(uint8_t data);

        /**
         * Write a vector of bytes
         * @param data the data to write
         */
        void writeData(std::vector<uint8_t> data);

        /**
         * Read the specified number of bytes
         * @param count the number of bytes to read
         * @return a vector of bytes
         */
        std::vector<uint8_t> readData(size_t count);

        /**
         * Read the specified 8-bit register
         * @param reg the register to read
         * @return the byte read
         */
        uint8_t readByteData(uint8_t reg);

        /**
         * Read the specified little-endian 16-bit register
         * @param reg the address of the register
         * @return the 16-bit little-endian word read
         */
        uint16_t readWordData(uint8_t reg);

        /**
         * Read the specified big-endian 16-bit register
         * @param reg the address of the register
         * @return the 16-bit big-endian word read
         */
        uint16_t readWordDataBE(uint8_t reg);

        /**
         * Read a 16-bit little-endian word from the device
         * @return the 16-bit little-endian word read
         */
        uint16_t readWord();

        /**
         * Read a 16-bit big-endian word from the device
         * @return the 16-bit big-endian word read
         */
        uint16_t readWordBE();

        /**
         * Write a byte to the specified register
         * @param reg the register to write
         * @param data the data to write
         */
        void writeByteData(uint8_t reg, uint8_t data);

        /**
         * Write a 16-bit little-endian word to the specified register
         * @param reg the register to write
         * @param data the 16-bit little-endian word to write
         */
        void writeWordData(uint8_t reg, uint16_t data);

        /**
         * Write a 16-bit big-endian word to the specified register
         * @param reg the register to write
         * @param data the 16-bit big-endian word to write
         */
        void writeWordDataBE(uint8_t reg, uint16_t data);

        /**
         * Read a buffer of data starting at the specified register
         * @param reg the register to start reading from
         * @param count the number of bytes to read
         * @return a vector of bytes read from the device
         */
        std::vector<uint8_t> readBufferData(uint8_t reg, size_t count);

        /**
         * Write a buffer of data to the specified register
         * @param reg the register to start writing to
         * @param data a vector of bytes to write to the device
         */
        void writeBufferData(uint8_t reg, std::vector<uint8_t> data);


    private:
        uint8_t address;
        int rateKhz;
        I2c &i2c;
    };
}