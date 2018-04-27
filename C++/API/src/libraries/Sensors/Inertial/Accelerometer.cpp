#include "Sensors/Inertial/Accelerometer.h"

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Inertial {
                vector3_t &Accelerometer::accelerometer() {
                    if (autoUpdateWhenPropertyRead) update();
                    return _accelerometer;
                }
            }
        }
    }
}