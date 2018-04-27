#pragma once

#include <Treehopper.Libraries.h>
#include <Pollable.h>

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Humidity {
                class LIBRARIES_API HumiditySensor : virtual public Pollable {
                public:
                    double relativeHumidity();

                protected:
                    double _relativeHumidity;
                };
            }
        }
    }
}

