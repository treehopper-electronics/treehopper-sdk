#include "SMBusDevice.h"
#include <algorithm>

using namespace std;

namespace Treehopper {
    SMBusDevice::SMBusDevice(uint8_t address, I2c &i2cModule, int rateKHz) : i2c(i2cModule) {
        if (address > 0x7f)
            throw "The address parameter expects a 7-bit address that doesn't include a Read/Write bit. The maximum address is 0x7F";
        this->address = address;
        this->rateKhz = rateKHz;
        i2c.enabled(true);
    }

    SMBusDevice::~SMBusDevice() {
    }

    uint8_t SMBusDevice::readByte() {
        i2c.speed(rateKhz);

        // S Addr Rd [A] [Data] NA P
        return i2c.sendReceive(address, std::vector<uint8_t>(), 1)[0];
    }

    void SMBusDevice::writeByte(uint8_t data) {
        i2c.speed(rateKhz);
        std::vector<uint8_t> vec = {data};

        // S Addr Wr [A] Data [A] P
        i2c.sendReceive(address, vec, 0);
    }

    void SMBusDevice::writeData(std::vector<uint8_t> data) {
        i2c.speed(rateKhz);
        i2c.sendReceive(address, data, 0);
    }

    std::vector<uint8_t> SMBusDevice::readData(size_t count) {
        i2c.speed(rateKhz);
        return i2c.sendReceive(address, std::vector<uint8_t>(), count);
    }

    uint8_t SMBusDevice::readByteData(uint8_t reg) {
        i2c.speed(rateKhz);
        std::vector<uint8_t> data = {reg};
        // S Addr Wr [A] Comm [A] S Addr Rd [A] [Data] NA P
        return i2c.sendReceive(address, data, 1)[0];
    }

    uint16_t SMBusDevice::readWordData(uint8_t reg) {
        i2c.speed(rateKhz);
        std::vector<uint8_t> regData = {reg};
        // S Addr Wr [A] Comm [A] S Addr Rd [A] [DataLow] A [DataHigh] NA P
        auto data = i2c.sendReceive(address, regData, 2);
        return (uint16_t) ((data[1] << 8) | data[0]);
    }

    uint16_t SMBusDevice::readWordDataBE(uint8_t reg) {
        std::vector<uint8_t> regData = {reg};
        i2c.speed(rateKhz);
        // S Addr Wr [A] Comm [A] S Addr Rd [A] [DataLow] A [DataHigh] NA P
        auto data = i2c.sendReceive(address, regData, 2);
        return (uint16_t) ((data[0] << 8) | data[1]);
    }

    uint16_t SMBusDevice::readWord() {
        i2c.speed(rateKhz);
        // S Addr Wr [A] Comm [A] S Addr Rd [A] [DataLow] A [DataHigh] NA P
        auto data = i2c.sendReceive(address, std::vector<uint8_t>(), 2);
        return (uint16_t) ((data[1] << 8) | data[0]);
    }

    uint16_t SMBusDevice::readWordBE() {
        i2c.speed(rateKhz);
        // S Addr Wr [A] Comm [A] S Addr Rd [A] [DataHigh] A [DataLow] NA P
        auto data = i2c.sendReceive(address, std::vector<uint8_t>(), 2);
        return (uint16_t) ((data[0] << 8) | data[1]);
    }

    void SMBusDevice::writeByteData(uint8_t reg, uint8_t data) {
        std::vector<uint8_t> dataToSend = {reg, data};
        i2c.speed(rateKhz);
        // S Addr Wr [A] Comm [A] Data [A] P
        i2c.sendReceive(address, dataToSend);
    }

    void SMBusDevice::writeWordData(uint8_t reg, uint16_t data) {
        std::vector<uint8_t> dataToSend = {reg, (uint8_t) (data & 0xFF), (uint8_t) (data >> 8)};
        i2c.speed(rateKhz);
        // S Addr Wr [A] Comm [A] DataLow [A] DataHigh [A] P
        i2c.sendReceive(address, dataToSend);
    }

    void SMBusDevice::writeWordDataBE(uint8_t reg, uint16_t data) {
        std::vector<uint8_t> dataToSend = {reg, (uint8_t) (data >> 8), (uint8_t) (data & 0xFF)};
        i2c.speed(rateKhz);
        // S Addr Wr [A] Comm [A] DataHigh [A] DataLow [A] P
        i2c.sendReceive(address, dataToSend, 0);
    }

    std::vector<uint8_t> SMBusDevice::readBufferData(uint8_t reg, size_t count) {
        std::vector<uint8_t> regData = {reg};
        i2c.speed(rateKhz);
        return i2c.sendReceive(address, regData, count);
    }

    void SMBusDevice::writeBufferData(uint8_t reg, std::vector<uint8_t> data) {
        data.insert(data.begin(), reg);
        i2c.speed(rateKhz);
        i2c.sendReceive(address, data, 0);
    }
}
