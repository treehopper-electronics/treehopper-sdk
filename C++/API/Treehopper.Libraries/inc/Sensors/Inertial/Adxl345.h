#pragma once
#include "Treehopper.Libraries.h"
#include "Adxl345Registers.h"
#include "Accelerometer.h"

using namespace Treehopper::Libraries;

namespace Treehopper { namespace Libraries { namespace Sensors { namespace Inertial {
	class Adxl345 : public Accelerometer
	{
	public:
		Adxl345(I2c &i2c, bool altAddress = false, int rate = 100);
        void update();
	private:
		Adxl345Registers registers;
		SMBusDevice _dev;
	};
} } } }
