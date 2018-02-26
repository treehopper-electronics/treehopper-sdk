#pragma once
#include "Treehopper.Libraries.h"
#include "Sensors/Pressure/PressureSensor.h"
#include "Sensors/Temperature/TemperatureSensor.h"
#include "Sensors/Pressure/Bmp280Registers.h"
#include "I2c.h"
#include <memory>

using namespace Treehopper::Libraries;
using namespace Treehopper::Libraries::Sensors::Temperature;
using namespace Treehopper::Libraries::Sensors::Pressure;

namespace Treehopper
{
	namespace Libraries
	{
		namespace Sensors
		{
			namespace Pressure
			{
                class Bmp280Registers;
				class LIBRARIES_API Bmp280 : public PressureSensor, public TemperatureSensor
				{
				public:
					Bmp280(I2c& i2c, bool sdoPin=false, int rate=100);
					~Bmp280();
					void update();
					double altitude();
					double referencePressure;
				protected:
					double tFine;
                    Bmp280Registers registers;
				private:
					SMBusDevice dev;
					double altitudeFromPressure(double temperature, double pressure);
					double _altitude;
				};
			}
		}
	}
}