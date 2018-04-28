#pragma once

#include "Libraries/Treehopper.Libraries.h"
#include "Libraries/Pollable.h"

using namespace Treehopper::Libraries;

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Temperature {
                class LIBRARIES_API TemperatureSensor : virtual public Pollable {
                public:
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
