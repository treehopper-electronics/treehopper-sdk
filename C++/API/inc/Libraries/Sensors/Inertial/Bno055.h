#pragma once

#include <SMBusDevice.h>
#include <Libraries/Sensors/Temperature/TemperatureSensor.h>
#include <Libraries/Sensors/Magnetic/Magnetometer.h>
#include "Accelerometer.h"
#include "Bno055Registers.h"
#include "Gyroscope.h"

using namespace Treehopper::Libraries::Sensors::Temperature;
using namespace Treehopper::Libraries::Sensors::Magnetic;

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Inertial {
                class Bno055
                        : public virtual Accelerometer,
                          public virtual Gyroscope,
                          public virtual Magnetometer,
                          public TemperatureSensor {
                public:
                    Bno055(I2c &i2c, bool altAddress = false, int rate = 100);

                    void update();

                    vector3_t linearAcceleration();

                    vector3_t gravity();

                    quaternion_t quaternion();

                    eularAngles_t eularAngles();

                private:
                    Bno055Registers registers;
                    SMBusDevice _dev;
                    vector3_t _linearAcceleration;
                    vector3_t _gravity;
                    quaternion_t _quaternion;
                    eularAngles_t _eularAngles;
                };
            }
        }
    }
}