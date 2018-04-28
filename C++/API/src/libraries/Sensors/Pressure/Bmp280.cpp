#include <SMBusDevice.h>
#include "Libraries/Sensors/Pressure/Bmp280.h"
#include <cmath>

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Pressure {
                Bmp280::Bmp280(I2c &i2c, bool sdoPin, int rate) :
                        dev(0x76 | (sdoPin ? 1 : 0), i2c, rate),
                        registers(dev),
                        referencePressure(101325) {
                    registers.ctrlMeasure.setMode(Modes::Normal);
                    registers.ctrlMeasure.setOversamplingPressure(OversamplingPressures::Oversampling_x16);
                    registers.ctrlMeasure.setOversamplingTemperature(OversamplingTemperatures::Oversampling_x16);
                    registers.ctrlMeasure.write();

                    registers.readRange(registers.t1, registers.h1);
                }

                void Bmp280::update() {
                    registers.readRange(registers.pressure,
                                        registers.humidity); // even though this the BMP280, assume it's a BME280 so the bus is less chatty.
                    // From Appendix A of the Bosch BMP280 datasheet
                    auto var1 =
                            (registers.temperature.value / 16384.0 - registers.t1.value / 1024.0) * registers.t2.value;
                    auto var2 = ((registers.temperature.value / 131072.0 - registers.t1.value / 8192.0) *
                                 (registers.temperature.value / 131072.0 - registers.t1.value / 8192.0)) *
                                registers.t3.value;
                    tFine = (var1 + var2);
                    _celsius = (var1 + var2) / 5120.0;

                    double p;
                    var1 = tFine / 2.0 - 64000.0;
                    var2 = var1 * var1 * registers.p6.value / 32768.0;
                    var2 = var2 + var1 * registers.p5.value * 2.0;
                    var2 = (var2 / 4.0) + registers.p4.value * 65536.0;
                    var1 = (registers.p3.value * var1 * var1 / 524288.0 + registers.p2.value * var1) / 524288.0;
                    var1 = (1.0 + var1 / 32768.0) * registers.p1.value;
                    if (var1 == 0.0) {
                        // avoid exception caused by division by zero
                    } else {
                        p = 1048576.0 - registers.pressure.value;
                        p = (p - (var2 / 4096.0)) * 6250.0 / var1;
                        var1 = registers.p9.value * p * p / 2147483648.0;
                        var2 = p * registers.p8.value / 32768.0;
                        p = p + (var1 + var2 + registers.p7.value) / 16.0;
                        _pascal = p;
                        auto kelvin = TemperatureSensor::toKelvin(_celsius);
                        _altitude = altitudeFromPressure(kelvin, _pascal);
                    }
                }

                double Bmp280::altitudeFromPressure(double temperature, double pressure) {
                    auto M = 0.0289644; // molar mass of earths' air
                    auto g = 9.80665; // gravity
                    auto R = 8.31432; // universal gas constant
                    if (referencePressure / pressure < 101325 / 22632.1) {
                        auto d = -0.0065;
                        auto e = 0;
                        auto j = pow(pressure / referencePressure, R * d / (g * M));
                        return e + temperature * (1 / j - 1) / d;
                    }
                    if (referencePressure / pressure < 101325 / 5474.89) {
                        auto e = 11000;
                        auto b = temperature - 71.5;
                        auto f = R * b * log(pressure / referencePressure) / (-g * M);
                        auto l = 101325;
                        auto c = 22632.1;
                        auto h = R * b * log(l / c) / (-g * M) + e;
                        return h + f;
                    }
                    return NAN;
                }

                double Bmp280::altitude() {
                    if (autoUpdateWhenPropertyRead)
                        update();
                    return _altitude;
                }

                Bmp280::~Bmp280() {

                }
            }
        }
    }
}