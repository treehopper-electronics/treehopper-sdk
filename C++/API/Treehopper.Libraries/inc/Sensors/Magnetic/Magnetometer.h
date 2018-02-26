#pragma once

#include <tuple>
#include <Types.h>
#include "Treehopper.Libraries.h"
#include "Pollable.h"

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Magnetic {
                class LIBRARIES_API Magnetometer : virtual public Pollable {
                public:
                    vector3_t magnetometer();
                protected:
                    vector3_t _magnetometer;
                };
            }
        }
    }
}
