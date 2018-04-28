#include <Libraries/Sensors/Optical/AmbientLightSensor.h>

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Optical {
                double AmbientLightSensor::lux() {
                    if (autoUpdateWhenPropertyRead) update();

                    return _lux;
                }
            }
        }
    }
}
