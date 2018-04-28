#pragma once

#include <tuple>
#include <Libraries/Types.h>
#include "Libraries/Treehopper.Libraries.h"
#include "Libraries/Pollable.h"

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
