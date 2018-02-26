#include "Sensors/Magnetic/Magnetometer.h"

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Magnetic {
                vector3_t Magnetometer::magnetometer() {
                    if(autoUpdateWhenPropertyRead) update();

                    return _magnetometer;
                }
            }
        }
    }
}