#pragma once

#include <Libraries/Sensors/ProximitySensor.h>

using namespace Treehopper::Libraries;

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            double ProximitySensor::centimeters() {
                return meters() * 100;
            }

            double ProximitySensor::inches() {
                return meters() * 39.3701;
            }

            double ProximitySensor::millimeters() {
                return meters() * 1000;
            }

            double ProximitySensor::feet() {
                return meters() * 3.28085;
            }

            double ProximitySensor::meters() {
                if (autoUpdateWhenPropertyRead) update();

                return _meters;
            }
        }
    }
}
