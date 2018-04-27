#pragma once

#include "Treehopper.Libraries.h"
#include "Sensors/Inertial/Mpu6050.h"
#include "Sensors/Magnetic/Ak8975.h"

using namespace Treehopper::Libraries::Sensors::Magnetic;

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Inertial {
                class LIBRARIES_API Mpu9250
                        : virtual public Mpu6050,
                          virtual public Magnetometer {
                public:
                    Mpu9250(I2c &i2c, bool addressPin = false, int rate = 100);

                    void update();

                    bool enableMagnetometer = true;
                private:
                    Ak8975 mag;
                };
            }
        }
    }
}
