#pragma once
#include "I2c.h"
namespace Treehopper 
{
	class TREEHOPPER_API SMBusDevice
	{
	public:
		SMBusDevice(uint8_t address, I2c& i2cModule, int rateKHz = 100);
		~SMBusDevice();

		uint8_t readByte();
		void writeByte(uint8_t data);
		void writeData(uint8_t* outBuffer, size_t count);
		void readData(uint8_t* inBuffer, size_t count);
		uint8_t readByteData(uint8_t reg);
		uint16_t readWordData(uint8_t reg);
		uint16_t readWordDataBE(uint8_t reg);
		uint16_t readWord();
		uint16_t readWordBE();
		void writeByteData(uint8_t reg, uint8_t data);
		void writeWordData(uint8_t reg, uint16_t data);
		void writeWordDataBE(uint8_t reg, uint16_t data);
		void readBufferData(uint8_t reg, uint8_t* inBuffer, size_t count);
		void writeBufferData(uint8_t reg, uint8_t* outBuffer, size_t count);


	private:
		uint8_t address;
		int rateKhz;
		I2c& i2c;
	};
}