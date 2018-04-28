#include "Libraries/Sensors/Temperature/Mcp9808.h"

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Temperature {
                Mcp9808::Mcp9808(I2c &i2c, bool a0, bool a1, bool a2)
                        : Mcp9808(i2c, (uint8_t) ((0x18 | (a0 ? 1 : 0) | ((a1 ? 1 : 0) << 1) | ((a2 ? 1 : 0) << 2)))) {
                }

                Mcp9808::Mcp9808(I2c &i2c, uint8_t address) : dev(address, i2c) {
                }

                void Mcp9808::update() {
                    auto data = dev.readWordDataBE(0x05);
                    double temp = data & 0x0FFF;
                    temp /= 16.0;
                    if ((data & 0x1000) > 0)
                        temp -= 256;
                    _celsius = temp;
                }
            }
        }
    }
}