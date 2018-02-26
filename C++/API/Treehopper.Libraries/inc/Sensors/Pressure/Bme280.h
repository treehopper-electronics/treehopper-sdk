#pragma once

#include <HardwareI2c.h>
#include <Sensors/Humidity/HumiditySensor.h>
#include "Treehopper.Libraries.h"
#include "Bmp280.h"

using namespace Treehopper::Libraries::Sensors::Humidity;

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Pressure {
                class LIBRARIES_API Bme280 : public Bmp280, public HumiditySensor {
                public:
                    Bme280(I2c &i2c, bool sdoPin, int rate = 100);

                    void update();

                private:
                    short h4;
                    short h5;
                };
            }
        }
    }
}