#include "I2c.h"
#include "Libraries/Sensors/Inertial/Mpu6050.h"

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Inertial {
                Mpu6050::Mpu6050(I2c &i2c, bool addressPin, int rate) :
                        dev(addressPin ? 0x69 : 0x68, i2c, rate),
                        _registers(dev) {
                    _registers.powerMgmt1.read();
                    _registers.powerMgmt1.reset = 1;
                    _registers.powerMgmt1.write();
                    _registers.powerMgmt1.reset = 0;
                    _registers.powerMgmt1.sleep = 0;
                    _registers.powerMgmt1.write();
                    _registers.powerMgmt1.clockSel = 1;
                    _registers.powerMgmt1.write();
                    _registers.configuration.dlpf = 3;
                    _registers.configuration.write();
                    _registers.sampleRateDivider.value = 4;
                    _registers.sampleRateDivider.write();
                    _registers.accelConfig2.read();
                    _registers.accelConfig2.accelFchoice = 0;
                    _registers.accelConfig2.dlpfCfg = 3;
                    _registers.accelConfig2.write();
                    _registers.powerMgmt1.read();
                }

                void Mpu6050::update() {
                    _registers.readRange(_registers.accel_x, _registers.gyro_z);
                    // TODO: let user set scale
                    auto accelScale = 2.0f / 32768.0f;
                    auto gyroScale = 250.0f / 32768.0f;
                    _accelerometer.x = _registers.accel_x.value * accelScale;
                    _accelerometer.y = _registers.accel_y.value * accelScale;
                    _accelerometer.z = _registers.accel_z.value * accelScale;

                    _celsius = _registers.temp.value / 333.87 + 21.0;

                    _gyroscope.x = _registers.gyro_x.value * gyroScale;
                    _gyroscope.y = _registers.gyro_y.value * gyroScale;
                    _gyroscope.z = _registers.gyro_z.value * gyroScale;
                }
            }
        }
    }
}
