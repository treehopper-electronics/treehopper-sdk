#include "Libraries/Sensors/Inertial/Lis3dh.h"

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Inertial {
                Lis3dh::Lis3dh(I2c &i2c, bool sdo, int rate) :
                        _dev((uint8_t) (sdo ? 0x19 : 0x18), i2c, rate), registers(_dev) {
                    registers.ctrl1.xAxisEnable = 1;
                    registers.ctrl1.yAxisEnable = 1;
                    registers.ctrl1.zAxisEnable = 1;
                    registers.ctrl1.setOutputDataRate(OutputDataRates::Hz_1);
                    registers.ctrl1.lowPowerEnable = 0;
                    registers.ctrl1.write();
                }

                void Lis3dh::update() {
                    registers.readRange(registers.outX, registers.outZ);
                    _accelerometer.x = (float) registers.outX.value;
                    _accelerometer.y = (float) registers.outY.value;
                    _accelerometer.z = (float) registers.outZ.value;
                }
            }
        }
    }
}