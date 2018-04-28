#include <thread>
#include "Libraries/Sensors/Inertial/Bno055.h"

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Inertial {
                Bno055::Bno055(I2c &i2c, bool altAddress, int rate)
                        : _dev((uint8_t) (altAddress ? 0x28 : 0x29), i2c, rate),
                          registers(_dev) {
                    registers.operatingMode.setOperatingMode(OperatingModes::ConfigMode);
                    registers.operatingMode.write();
                    registers.unitSel.accel = 1;
                    registers.unitSel.gyro = 0;
                    registers.unitSel.write();
                    registers.sysTrigger.resetSys = 1;
                    registers.sysTrigger.write();
                    registers.sysTrigger.resetSys = 0;
                    int id = 0;
                    do {
                        registers.chipId.read();
                        id = registers.chipId.value;
                    } while (id != 0xA0);
                    std::this_thread::sleep_for(std::chrono::milliseconds(50));
                    registers.powerMode.setPowerMode(PowerModes::Normal);
                    registers.powerMode.write();
                    std::this_thread::sleep_for(std::chrono::milliseconds(10));
                    registers.sysTrigger.selfTest = 0;
                    registers.sysTrigger.write();
                    std::this_thread::sleep_for(std::chrono::milliseconds(10));
                    registers.operatingMode.setOperatingMode(OperatingModes::NineDegreesOfFreedom);
                    registers.operatingMode.write();
                    std::this_thread::sleep_for(std::chrono::milliseconds(20));
                }

                void Bno055::update() {
                    registers.readRange(registers.accelX, registers.temp);

                    _accelerometer.x = registers.accelX.value / 1000.0f;
                    _accelerometer.y = registers.accelY.value / 1000.0f;
                    _accelerometer.z = registers.accelZ.value / 1000.0f;

                    _magnetometer.x = registers.magnetometerX.value / 16.0f;
                    _magnetometer.y = registers.magnetometerY.value / 16.0f;
                    _magnetometer.z = registers.magnetometerZ.value / 16.0f;

                    _gyroscope.x = registers.gyroX.value / 16.0f;
                    _gyroscope.y = registers.gyroY.value / 16.0f;
                    _gyroscope.z = registers.gyroZ.value / 16.0f;

                    _linearAcceleration.x = registers.linX.value / 1000.0f;
                    _linearAcceleration.y = registers.linY.value / 1000.0f;
                    _linearAcceleration.z = registers.linZ.value / 1000.0f;

                    _gravity.x = registers.gravX.value / 980.0f;
                    _gravity.y = registers.gravY.value / 980.0f;
                    _gravity.z = registers.gravZ.value / 980.0f;

                    _eularAngles.pitch = registers.eulPitch.value / 100.0f;
                    _eularAngles.roll = registers.eulRoll.value / 100.0f;
                    _eularAngles.yaw = registers.eulHeading.value / 100.0f;

                    _quaternion.w = registers.quaW.value / 16384.0f;
                    _quaternion.x = registers.quaX.value / 16384.0f;
                    _quaternion.y = registers.quaY.value / 16384.0f;
                    _quaternion.z = registers.quaZ.value / 16384.0f;

                    _celsius = (char) (registers.temp.value);
                }

                vector3_t Bno055::linearAcceleration() {
                    if (autoUpdateWhenPropertyRead) update();

                    return _linearAcceleration;
                }

                vector3_t Bno055::gravity() {
                    if (autoUpdateWhenPropertyRead) update();

                    return _gravity;
                }

                quaternion_t Bno055::quaternion() {
                    if (autoUpdateWhenPropertyRead) update();

                    return _quaternion;
                }

                eularAngles_t Bno055::eularAngles() {
                    if (autoUpdateWhenPropertyRead) update();

                    return _eularAngles;
                }
            }
        }
    }
}