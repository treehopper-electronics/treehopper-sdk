#include "Sensors/Inertial/Gyroscope.h"

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Inertial {
                vector3_t Gyroscope::gyroscope() {
                    if (autoUpdateWhenPropertyRead) update();

                    return _gyroscope;
                }
            }
        }
    }
}