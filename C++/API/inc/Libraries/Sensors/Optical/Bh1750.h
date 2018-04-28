#pragma once

#include <Libraries/Pollable.h>
#include <I2c.h>
#include <SMBusDevice.h>
#include "AmbientLightSensor.h"

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Optical {
                class LIBRARIES_API Bh1750 : public virtual AmbientLightSensor {
                public:
                    enum class LuxResolution {
                        Medium,
                        High,
                        Low
                    };

                    Bh1750(I2c &i2c, bool addressPin = false, int rate = 100);

                    void update();

                    LuxResolution resolution();

                    void resolution(LuxResolution resolution);

                private:
                    SMBusDevice dev;
                    LuxResolution _resolution;
                };
            }
        }
    }
}
