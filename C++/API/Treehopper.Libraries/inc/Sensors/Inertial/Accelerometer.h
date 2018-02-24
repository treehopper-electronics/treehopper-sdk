#pragma once
#include "Treehopper.Libraries.h"
#include "Pollable.h"
#include <tuple>

using namespace Treehopper::Libraries;

namespace Treehopper
{
	namespace Libraries
	{
		namespace Sensors
		{
			namespace Inertial
			{
				class LIBRARIES_API Accelerometer : Pollable
				{
				public:
					Accelerometer();
					~Accelerometer();
					std::tuple<double, double, double> getAcceleration();

				protected:
					std::tuple<double, double, double>  acceleration;

				};
			}
		}
	}
}


