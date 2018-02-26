#include "Sensors/Pressure/PressureSensor.h"

namespace Treehopper
{
	namespace Libraries
	{
		namespace Sensors
		{
			namespace Pressure
			{
				double PressureSensor::pascal()
				{
					if (autoUpdateWhenPropertyRead) update();

					return _pascal;
				}

				double PressureSensor::bar()
				{
					return pascal() / 100000.0;
				}

				double PressureSensor::atm()
				{
					return pascal() / (1.01325 * 100000.0);
				}

				double PressureSensor::psi()
				{
					return atm() / 14.7;
				}
			}
		}
	}
}