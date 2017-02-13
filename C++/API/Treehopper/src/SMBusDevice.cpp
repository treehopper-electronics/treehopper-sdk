#include "stdafx.h"
#include "SMBusDevice.h"
#include <algorithm>
using namespace std;

namespace Treehopper 
{
	SMBusDevice::SMBusDevice(uint8_t address, I2c& i2cModule, int rateKHz) : i2c(i2cModule)
	{
		if (address > 0x7f)
			throw "The address parameter expects a 7-bit address that doesn't include a Read/Write bit. The maximum address is 0x7F";
		this->address = address;
		this->rateKhz = rateKHz;
		i2c.enabled(true);
	}

	SMBusDevice::~SMBusDevice()
	{
	}

	uint8_t SMBusDevice::readByte()
	{
		i2c.speed(rateKhz);

		// S Addr Rd [A] [Data] NA P
		uint8_t data;
		i2c.sendReceive(address, NULL, 0, &data, 1);
		return data;
	}

	void SMBusDevice::writeByte(uint8_t data)
	{
		i2c.speed(rateKhz);

		// S Addr Wr [A] Data [A] P
		i2c.sendReceive(address, &data, 1, NULL, 0);
	}

	void SMBusDevice::writeData(uint8_t* dataOut, size_t count)
	{
		i2c.speed(rateKhz);
		i2c.sendReceive(address, dataOut, count, NULL, 0);
	}

	void SMBusDevice::readData(uint8_t* dataIn, size_t count)
	{
		i2c.speed(rateKhz);
		i2c.sendReceive(address, NULL, 0, dataIn, count);
	}

	uint8_t SMBusDevice::readByteData(uint8_t reg)
	{
		i2c.speed(rateKhz);
		uint8_t data;
		// S Addr Wr [A] Comm [A] S Addr Rd [A] [Data] NA P
		i2c.sendReceive(address, &reg, 1, &data, 1);
		return data;
	}

	uint16_t SMBusDevice::readWordData(uint8_t reg)
	{
		uint8_t data[2];
		i2c.speed(rateKhz);
		// S Addr Wr [A] Comm [A] S Addr Rd [A] [DataLow] A [DataHigh] NA P
		i2c.sendReceive(address, &reg, 1, data, 2);
		return (uint16_t)((data[1] << 8) | data[0]);
	}

	uint16_t SMBusDevice::readWordDataBE(uint8_t reg)
	{
		uint8_t data[2];
		i2c.speed(rateKhz);
		// S Addr Wr [A] Comm [A] S Addr Rd [A] [DataLow] A [DataHigh] NA P
		i2c.sendReceive(address, &reg, 1, data, 2);
		return (uint16_t)((data[0] << 8) | data[1]);
	}

	uint16_t SMBusDevice::readWord()
	{
		uint8_t data[2];
		i2c.speed(rateKhz);
		// S Addr Wr [A] Comm [A] S Addr Rd [A] [DataLow] A [DataHigh] NA P
		i2c.sendReceive(address, NULL, 0, data, 2);
		return (uint16_t)((data[1] << 8) | data[0]);
	}

	uint16_t SMBusDevice::readWordBE()
	{
		uint8_t data[2];
		i2c.speed(rateKhz);
		// S Addr Wr [A] Comm [A] S Addr Rd [A] [DataHigh] A [DataLow] NA P
		i2c.sendReceive(address, NULL, 0, data, 2);
		return (uint16_t)((data[0] << 8) | data[1]);
	}

	void SMBusDevice::writeByteData(uint8_t reg, uint8_t data)
	{
		uint8_t dataToSend[2];
		dataToSend[0] = reg;
		dataToSend[1] = data;
		i2c.speed(rateKhz);
		// S Addr Wr [A] Comm [A] Data [A] P
		i2c.sendReceive(address, dataToSend, 2, NULL, 0);
	}

	void SMBusDevice::writeWordData(uint8_t reg, uint16_t data)
	{
		uint8_t dataToSend[3];
		dataToSend[0] = reg;
		dataToSend[1] = (data & 0xFF);
		dataToSend[2] = (data >> 8);
		i2c.speed(rateKhz);
		// S Addr Wr [A] Comm [A] DataLow [A] DataHigh [A] P
		i2c.sendReceive(address, dataToSend, 3, NULL, 0);
	}

	void SMBusDevice::writeWordDataBE(uint8_t reg, uint16_t data)
	{
		uint8_t dataToSend[3];
		dataToSend[0] = reg;
		dataToSend[1] = (data >> 8);
		dataToSend[2] = (data & 0xFF);
		i2c.speed(rateKhz);
		// S Addr Wr [A] Comm [A] DataHigh [A] DataLow [A] P
		i2c.sendReceive(address, dataToSend, 3, NULL, 0);
	}

	void SMBusDevice::readBufferData(uint8_t reg, uint8_t* inBuffer, size_t count)
	{
		i2c.speed(rateKhz);
		i2c.sendReceive(address, &reg, 1, inBuffer, count);
	}

	void SMBusDevice::writeBufferData(uint8_t reg, uint8_t* outBuffer, size_t count)
	{
		i2c.speed(rateKhz);
		uint8_t* data = new uint8_t[count + 1];
		data[0] = reg;
		copy(outBuffer, outBuffer + count, data + 1);
		i2c.sendReceive(address, data, count + 1, NULL, 0);
		delete[] data;
	}
}
