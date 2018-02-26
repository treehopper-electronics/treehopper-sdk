#pragma once

#include "Treehopper.Libraries.h"

namespace Treehopper
{
    namespace Libraries {
		class LIBRARIES_API Pollable
		{
		public:
			bool autoUpdateWhenPropertyRead = true;
			virtual void update() = 0;
		};
	}
}
