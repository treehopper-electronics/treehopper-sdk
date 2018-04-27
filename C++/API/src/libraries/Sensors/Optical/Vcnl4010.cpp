#include "Sensors/Optical/Vcnl4010.h"
#include <cmath>

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Optical {

                Vcnl4010::Vcnl4010(I2c &i2c, int rate) :
                        dev(0x13, i2c, rate),
                        registers(dev) {
                    registers.readRange(registers.command, registers.ambientLightParameters);
                    registers.proximityRate.setRate(Rates::Hz_7_8125);
                    registers.ledCurrent.irLedCurrentValue = 20;
                    registers.ambientLightParameters.setAlsRate(AlsRates::Hz_10);
                    registers.ambientLightParameters.autoOffsetCompensation = 1;
                    registers.ambientLightParameters.averagingSamples = 5;
                    registers.writeRange(registers.command, registers.ambientLightParameters);
                }

                void Optical::Vcnl4010::update() {
                    // start ambient and prox conversion
                    registers.command.alsOnDemandStart = 1;
                    registers.command.proxOnDemandStart = 1;
                    registers.command.write();

                    while (true) {
                        registers.command.read();
                        if (registers.command.proxDataReady == 1 && registers.command.alsDataReady == 1)
                            break;
                    }

                    registers.ambientLightResult.read();
                    registers.proximityResult.read();

                    // from datasheet
                    _lux = registers.ambientLightResult.value * 0.25;

                    _rawProximity = registers.proximityResult.value;

                    // derived empirically
                    if (registers.proximityResult.value < 2298)
                        _meters = INFINITY;
                    else
                        _meters =
                                81.0 * pow(registers.proximityResult.value - 2298, -0.475) / 100; // empirically derived
                }

                double Vcnl4010::rawProximity() {
                    if (autoUpdateWhenPropertyRead) update();

                    return _rawProximity;
                }
            }
        }
    }
}
