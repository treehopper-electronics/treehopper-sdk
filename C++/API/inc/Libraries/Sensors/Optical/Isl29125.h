#pragma once

#include <Libraries/Pollable.h>
#include <I2c.h>
#include <SMBusDevice.h>
#include <DigitalIn.h>
#include "AmbientLightSensor.h"

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Optical {
                class LIBRARIES_API Isl29125 : public virtual Pollable {
                public:
                    Isl29125(I2c &i2c, DigitalIn intPin = NULL, int rate = 100);

                    void update();


                private:

                };
            }
        }
    }
}
