#pragma once

#include "Treehopper.Libraries.h"

namespace Treehopper
{
	namespace Libraries
	{
		class LIBRARIES_API Pollable
		{
		public:
			Pollable();
			~Pollable();
			bool autoUpdateWhenPropertyRead;
			virtual void update() = 0;
		};
	}
}
