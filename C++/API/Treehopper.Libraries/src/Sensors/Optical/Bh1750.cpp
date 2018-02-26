#include "Sensors/Optical/Bh1750.h"

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Optical {
                Bh1750::Bh1750(I2c &i2c, bool addressPin, int rate) :
                        dev((uint8_t) (addressPin ? 0x5C : 0x23), i2c, rate) {
                    dev.writeByte((uint8_t) 0x07);
                    resolution(LuxResolution::High);
                }

                void Bh1750::update() {
                    _lux = dev.readWordBE() / 1.2;
                }

                Bh1750::LuxResolution Bh1750::resolution() {
                    return _resolution;
                }

                void Bh1750::resolution(Bh1750::LuxResolution resolution) {
                    if (resolution == _resolution) return;

                    _resolution = resolution;
                    dev.writeByte((uint8_t) (0x10 | (uint8_t) _resolution));
                }
            }
        }
    }
}
