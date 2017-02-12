#include "stdafx.h"
#include "Sensors/Temperature/TemperatureSensor.h"
#include "stdafx.h"
namespace Treehopper
{
	namespace Libraries
	{
		namespace Sensors
		{
			namespace Temperature
			{
				TemperatureSensor::TemperatureSensor()
				{
				}

				TemperatureSensor::~TemperatureSensor()
				{
				}

				double TemperatureSensor::getCelsius()
				{
					if (autoUpdateWhenPropertyRead) update();
					return celsius;
				}

				double TemperatureSensor::getFahrenheit()
				{
					return ((getCelsius() * 9.0) / 5.0) + 32.0;
				}

				double TemperatureSensor::getKelvin()
				{
					return getCelsius() + 273.15;
				}
			}
		}
	}
}