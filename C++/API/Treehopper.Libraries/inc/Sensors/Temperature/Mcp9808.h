#pragma once

#include "Treehopper.Libraries/inc/Treehopper.Libraries.h"
#include "Treehopper.Libraries/inc/Sensors/Temperature/TemperatureSensor.h"
#include "Treehopper/inc/I2c.h"
#include "Treehopper/inc/SMBusDevice.h"

namespace Treehopper
{
	namespace Libraries
	{
		namespace Sensors
		{
			namespace Temperature
			{
				class LIBRARIES_API Mcp9808 : public TemperatureSensor
				{
				public:
					Mcp9808(I2c& i2c, byte_t address);
					Mcp9808(I2c& i2c, bool a0 = false, bool a1 = false, bool a2 = false);
					void update();
				protected:
					SMBusDevice dev;
				};
			}
		}
	}

}