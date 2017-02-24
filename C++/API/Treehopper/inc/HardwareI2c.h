#pragma once

#include "I2c.h"

namespace Treehopper 
{
	class TreehopperUsb;
	/** The built-in I2c interface */
	class TREEHOPPER_API HardwareI2c : public I2c
	{
	public:
		HardwareI2c(TreehopperUsb* board);
		~HardwareI2c();

		/** Set the speed, in kHz, of the i2c port */
		void speed(double value);

		/** Get the speed, in kHz, of the i2c port */
		double speed();

		/** Set whether the i2c module is enabled */
		void enabled(bool value);

		/** Gets whether the i2c module is enabled */
		bool enabled();

		/** Send and/or receive data with the i2c module.
		@param[in] address the address of the slave i2c device you wish to communicate with
		@param[in] writeBuffer a pointer to the buffer of data to send
		@param[in] numBytesToWrite the number of bytes to write
		@param[out] readBuffer a pointer to the buffer to use for reading data into
		@param[in] numBytesToRead the number of bytes to read from the device
		*/
		void sendReceive(uint8_t address, uint8_t* writeBuffer, size_t numBytesToWrite,
			uint8_t* readBuffer = NULL, size_t numBytesToRead = 0);

	private:
		TreehopperUsb* device;
		double _speed;
		bool _enabled;
		void sendConfig();
	};
}