#pragma once
#include "Treehopper.Libraries.h"
#include "Pollable.h"
#include <tuple>
#include <Types.h>

using namespace Treehopper::Libraries;

namespace Treehopper
{
	namespace Libraries
	{
		namespace Sensors
		{
			namespace Inertial
			{
				class LIBRARIES_API Accelerometer : virtual Pollable
				{
				public:
					vector3_t& accelerometer();

				protected:
                    vector3_t _accelerometer;

				};
			}
		}
	}
}


