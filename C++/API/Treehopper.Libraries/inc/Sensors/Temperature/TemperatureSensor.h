#pragma once

#include "Treehopper.Libraries.h"
#include "Pollable.h"

using namespace Treehopper::Libraries;

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Temperature {
                class LIBRARIES_API TemperatureSensor : virtual public Pollable {
                public:
                    TemperatureSensor();

                    ~TemperatureSensor();

                    static double toFahrenheit(double celsius);

                    static double toKelvin(double celsius);

                    double celsius();

                    double fahrenheit();

                    double kelvin();

                protected:
                    double _celsius;

                };
            }
        }
    }
}
