#pragma once
#include "Treehopper.Libraries.h"
#include "Adxl345Registers.h"

using namespace Treehopper::Libraries;

namespace Treehopper {
	namespace Libraries {
		namespace Sensors {
			namespace Inertial {
				class Adxl345
				{
				public:
					Adxl345(I2c& i2c, bool altAddress = false, int rate = 100) : _dev(!altAddress ? 0x53 : 0x1D, i2c, rate), registers(_dev)
					{
						registers.PowerCtl.Sleep = 0;
						registers.PowerCtl.Measure = 1;
						registers.DataFormat.Range = 0x03;
						registers.flush();
					}
				private:
					Adxl345Registers registers;
					SMBusDevice _dev;
				};
			}
		}
	}
}