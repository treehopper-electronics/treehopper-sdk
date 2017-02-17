#include "stdafx.h"
#include "Treehopper.Libraries/inc/Sensors/Temperature/Mcp9808.h"

namespace Treehopper
{
	namespace Libraries
	{
		namespace Sensors
		{
			namespace Temperature
			{
				Mcp9808::Mcp9808(I2c& i2c, bool a0, bool a1, bool a2)
					: Mcp9808(i2c, (uint8_t)((0x18 | (a0 ? 1 : 0) | ((a1 ? 1 : 0) << 1) | ((a2 ? 1 : 0) << 2))))
				{
				}

				Mcp9808::Mcp9808(I2c& i2c, uint8_t address) : dev(address, i2c)
				{
				}

				void Mcp9808::update()
				{
					auto data = dev.readWordDataBE(0x05);
					celsius = ((int16_t)(data << 3)) / 128.0;
				}
			}
		}
	}
}