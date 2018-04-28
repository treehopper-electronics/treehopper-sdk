#include <Libraries/Sensors/Humidity/HumiditySensor.h>

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Humidity {
                double HumiditySensor::relativeHumidity() {
                    if (autoUpdateWhenPropertyRead) update();
                    return _relativeHumidity;
                }
            }
        }
    }
}