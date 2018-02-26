#include "Sensors/Temperature/Lm75.h"

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Temperature {
                Lm75::Lm75(I2c &i2c, bool a0, bool a1, bool a2)
                        : dev((0x48 | (a0 ? 1 : 0) | ((a1 ? 1 : 0) << 1) | ((a2 ? 1 : 0) << 2)), i2c) {
                }

                void Lm75::update() {
                    auto data = dev.readWordDataBE(0x00);
                    _celsius = ((int16_t) data / 32.0) / 8.0;
                }
            }
        }
    }
}