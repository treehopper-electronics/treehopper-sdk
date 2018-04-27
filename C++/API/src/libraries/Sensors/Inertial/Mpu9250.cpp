#include "Sensors/Inertial/Mpu9250.h"

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Inertial {
                Mpu9250::Mpu9250(I2c &i2c, bool addressPin, int rate) :
                        Mpu6050(i2c, addressPin, rate),
                        mag(i2c, rate) {
                    mag.autoUpdateWhenPropertyRead = false;
                    _registers.intPinCfg.bypassEn = 1;
                    _registers.intPinCfg.latchIntEn = 1;
                    _registers.intPinCfg.write();
                }

                void Mpu9250::update() {
                    Mpu6050::update();

                    if (!enableMagnetometer) return;
                    mag.update();
                    _magnetometer = mag.magnetometer();
                }
            }
        }
    }
}
