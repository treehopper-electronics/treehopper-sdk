#include "Sensors/Pressure/Bme280.h"

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Pressure {
                Bme280::Bme280(I2c& i2c, bool sdoPin, int rate) :
                Bmp280(i2c, sdoPin, rate)
                {
                    registers.readRange(registers.h2, registers.h6);

                    // RegisterGenerator doesn't get the endianness right on the h4/h5 12-bit values, so manually create them:
                    h4 = (short)((short)((registers.h4.value << 4 | registers.h4h5.h4Low) << 4) >> 4);
                    h5 = (short)((short)((registers.h5.value << 4 | registers.h4h5.h5Low) << 4) >> 4);

                    registers.ctrlHumidity.setOversampling(Oversamplings::Oversampling_x16);
                    registers.ctrlHumidity.write();
                    registers.ctrlMeasure.write();
                }

                void Bme280::update() {
                    Bmp280::update();

                    // now the BME stuff
                    double var_H;

                    var_H = tFine - 76800.0;
                    var_H = (registers.humidity.value - (h4 * 64.0 + h5 / 16384.0 * var_H)) *
                            registers.h2.value / 65536.0 *
                            (1.0 + registers.h6.value / 67108864.0 * var_H *
                                   (1.0 + registers.h3.value / 67108864.0 * var_H));

                    var_H = var_H * (1.0 - registers.h1.value * var_H / 524288.0);

                    if (var_H > 100.0)
                        var_H = 100.0;
                    else if (var_H < 0.0)
                        var_H = 0.0;

                    _relativeHumidity = var_H;
                }
            }
        }
    }
}

