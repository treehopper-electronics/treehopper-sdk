#include "Libraries/Sensors/Temperature/Mlx90614.h"

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Temperature {
                Mlx90614::Mlx90614(I2c &i2c, int rate) :
                        dev(0x5B, i2c, rate),
                        ambient(dev, 0x06),
                        object(dev, 0x07) {}


                Mlx90614::TempRegister::TempRegister(SMBusDevice &dev, uint8_t reg)
                        : dev(dev),
                          reg(reg) {}

                void Mlx90614::TempRegister::update() {
                    auto data = dev.readWordData(reg);
                    data &= 0x7FFF; // chop off the error bit of the high byte
                    _celsius = data * 0.02 - 273.15;
                }
            }
        }
    }
}
