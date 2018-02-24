#pragma once
#include "Treehopper.Libraries.h"
#include "Pollable.h"

using namespace Treehopper::Libraries;

namespace Treehopper
{
	namespace Libraries
	{
		namespace Sensors
		{
			namespace Pressure
			{
				class LIBRARIES_API PressureSensor : virtual public Pollable
				{
				public:
					PressureSensor();
					~PressureSensor();
					double bar();
					double atm();
					double psi();
					double pascal();

				protected:
					double _pascal;
				};
			}
		}
	}
}