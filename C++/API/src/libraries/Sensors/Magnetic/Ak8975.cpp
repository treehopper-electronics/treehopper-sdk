#include "Sensors/Magnetic/Ak8975.h"

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Magnetic {
                Ak8975::Ak8975(I2c &i2c, int rate) : dev(0x0c, i2c, rate), registers(dev) {}

                void Ak8975::update() {
                    registers.control.mode = 1;
                    registers.control.write();
                    while (true) {
                        registers.status1.read();
                        if (registers.status1.drdy == 1)
                            break;
                    }

                    registers.readRange(registers.hx, registers.hz);
                    _magnetometer.x = registers.hx.value;
                    _magnetometer.y = registers.hy.value;
                    _magnetometer.z = registers.hz.value;
                }
            }
        }
    }
}