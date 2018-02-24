#pragma once

#include "I2c.h"

namespace Treehopper 
{
	class TreehopperUsb;
	/** The built-in I2c interface */
	class TREEHOPPER_API HardwareI2c : public I2c
	{
	public:
		HardwareI2c(TreehopperUsb& board);
		~HardwareI2c();
		virtual void speed(double value);
		virtual double speed();
		virtual void enabled(bool value);
		virtual bool enabled();
		virtual std::vector<uint8_t> sendReceive(uint8_t address, std::vector<uint8_t> data, size_t numBytesToRead = 0);

	private:
		TreehopperUsb& board;
		double _speed;
		bool _enabled;
		void sendConfig();
	};
}