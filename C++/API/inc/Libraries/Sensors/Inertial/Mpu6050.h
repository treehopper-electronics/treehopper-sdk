#pragma once

#include "Libraries/Treehopper.Libraries.h"
#include "Mpu6050Registers.h"
#include "Accelerometer.h"
#include "Gyroscope.h"
#include "SMBusDevice.h"
#include "Libraries/Sensors/Temperature/TemperatureSensor.h"

using namespace Treehopper::Libraries::Sensors::Temperature;

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Inertial {
                class LIBRARIES_API Mpu6050
                        : virtual public Accelerometer,
                          virtual public Gyroscope,
                          virtual public TemperatureSensor {
                public:
                    Mpu6050(I2c &i2c, bool addressPin = false, int rate = 100);

                    void update();

                protected:
                    Mpu6050Registers _registers;
                private:
                    SMBusDevice dev;
                };
            }
        }
    }
}
