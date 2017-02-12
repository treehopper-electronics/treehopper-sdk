#pragma once

#include "I2c.h"

namespace Treehopper 
{
	class TreehopperUsb;
	class TREEHOPPER_API HardwareI2c : public I2c
	{
	public:
		HardwareI2c(TreehopperUsb* board);
		~HardwareI2c();
		void setSpeed(double value);
		double getSpeed();

		void setEnabled(bool value);
		bool getEnabled();

		void sendReceive(uint8_t address, uint8_t* writeBuffer, size_t numBytesToWrite,
			uint8_t* readBuffer = NULL, size_t numBytesToRead = 0);

	private:
		TreehopperUsb* device;
		double speed;
		bool enabled;
		void sendConfig();
	};
}