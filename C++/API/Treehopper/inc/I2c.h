#pragma once
#include "Treehopper.h"
#include <stdint.h>

using namespace std;
namespace Treehopper 
{
	/** Base I2c interface */
	class TREEHOPPER_API I2c
	{
	public:
		~I2c() { }

		/** Set the speed, in kHz, to use. */
		virtual void speed(double value) = 0;

		/** Gets the current speed, in kHz. */
		virtual double speed() = 0;

		/** Sets whether the module is enabled */
		virtual void enabled(bool value) = 0;

		/** Gets whether the module is enabled */
		virtual bool enabled() = 0;

		/** Send and/or receive data from the module */
		virtual void sendReceive(uint8_t address, uint8_t* writeBuffer, size_t numBytesToWrite,
			uint8_t* readBuffer = NULL, size_t numBytesToRead = 0) = 0;
	};
}