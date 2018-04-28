#pragma once

#include "Libraries/Treehopper.Libraries.h"
#include "Libraries/Pollable.h"
#include <tuple>
#include <Libraries/Types.h>

using namespace Treehopper::Libraries;

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Inertial {
                class LIBRARIES_API Accelerometer : virtual public Pollable {
                public:
                    vector3_t &accelerometer();

                protected:
                    vector3_t _accelerometer;
                };
            }
        }
    }
}
