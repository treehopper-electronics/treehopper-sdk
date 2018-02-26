#pragma once

#include <tuple>
#include <Types.h>
#include "Treehopper.Libraries.h"
#include "Pollable.h"

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Inertial {
                class LIBRARIES_API Gyroscope : virtual public Pollable {
                public:
                    vector3_t gyroscope();

                protected:
                    vector3_t _gyroscope;
                };
            }
        }
    }
}
