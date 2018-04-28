#include <Libraries/Sensors/Inertial/Adxl345.h>

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Inertial {
                Adxl345::Adxl345(Treehopper::I2c &i2c, bool altAddress, int rate)
                        : _dev(!altAddress ? 0x53 : 0x1D, i2c, rate), registers(_dev) {
                    registers.powerCtl.sleep = 0;
                    registers.powerCtl.measure = 1;
                    registers.dataFormat.range = 0x03;
                    registers.dataFormat.fullRes = 1;
                    registers.writeRange(registers.powerCtl, registers.dataFormat);
                }

                void Adxl345::update() {
                    registers.readRange(registers.dataX, registers.dataZ);

                    _accelerometer.x = registers.dataX.value / 255.0f;
                    _accelerometer.y = registers.dataY.value / 255.0f;
                    _accelerometer.z = registers.dataZ.value / 255.0f;
                }
            }
        }
    }
}
