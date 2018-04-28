#pragma once

#include <tuple>
#include <Libraries/Types.h>
#include "Libraries/Treehopper.Libraries.h"
#include "Libraries/Pollable.h"

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
