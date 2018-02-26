#include "Sensors/Temperature/TemperatureSensor.h"

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Temperature {
                TemperatureSensor::TemperatureSensor() {
                }

                TemperatureSensor::~TemperatureSensor() {
                }

                double TemperatureSensor::toFahrenheit(double celsius) {
                    return ((celsius * 9.0) / 5.0) + 32.0;
                }

                double TemperatureSensor::toKelvin(double celsius) {
                    return celsius + 273.15;
                }

                double TemperatureSensor::celsius() {
                    if (autoUpdateWhenPropertyRead) update();
                    return _celsius;
                }

                double TemperatureSensor::fahrenheit() {
                    return toFahrenheit(celsius());
                }

                double TemperatureSensor::kelvin() {
                    return toKelvin(celsius());
                }
            }
        }
    }
}