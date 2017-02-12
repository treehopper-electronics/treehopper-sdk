#pragma once
#include "Treehopper.Libraries/inc/Treehopper.Libraries.h"
#include "Treehopper.Libraries/inc/Pollable.h"
using namespace Treehopper::Libraries;

namespace Treehopper
{
	namespace Libraries
	{
		namespace Sensors
		{
			namespace Temperature
			{
				class LIBRARIES_API TemperatureSensor : Pollable
				{
				public:
					TemperatureSensor();
					~TemperatureSensor();
					double getCelsius();
					double getFahrenheit();
					double getKelvin();

				protected:
					double celsius;

				};
			}
		}
	}
}


