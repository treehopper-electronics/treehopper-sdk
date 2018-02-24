#include "Sensors/Inertial/Accelerometer.h"

namespace Treehopper
{
	namespace Libraries
	{
		namespace Sensors
		{
			namespace Inertial
			{
				Accelerometer::Accelerometer()
				{
				}

				Accelerometer::~Accelerometer()
				{
				}

				std::tuple<double, double, double> Accelerometer::getAcceleration()
				{
					if (autoUpdateWhenPropertyRead) update();
					return acceleration;
				}
			}
		}
	}
}