#pragma once

#include <Pollable.h>

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Optical {
                class LIBRARIES_API AmbientLightSensor : public virtual Pollable {
                public:
                    double lux();

                protected:
                    double _lux;
                };
            }
        }
    }
}
