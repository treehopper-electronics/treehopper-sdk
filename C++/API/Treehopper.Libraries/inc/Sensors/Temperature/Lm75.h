#pragma once
#include "Treehopper.Libraries/inc/Treehopper.Libraries.h"
#include "TemperatureSensor.h"
#include "Treehopper/inc/I2c.h"
#include "Treehopper/inc/SMBusDevice.h"
#include <memory>
namespace Treehopper
{
	namespace Libraries
	{
		namespace Sensors
		{
			namespace Temperature
			{
				class LIBRARIES_API Lm75 : public TemperatureSensor
				{
				public:
					Lm75(I2c& i2c, bool a0 = false, bool a1 = false, bool a2 = false);
					~Lm75();
					void update();
				private:
					SMBusDevice dev;
				};
			}
		}
	}
}