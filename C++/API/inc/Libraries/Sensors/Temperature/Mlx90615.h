#pragma once

#include "Libraries/Treehopper.Libraries.h"
#include "TemperatureSensor.h"
#include "I2c.h"
#include "SMBusDevice.h"
#include "Mlx90614.h"
#include <memory>

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Temperature {
                class LIBRARIES_API Mlx90615 {
                public:
                    Mlx90615(I2c &i2c, int rate = 100) :
                            dev(0x5b, i2c, rate),
                            ambient(dev, 0x26),
                            object(dev, 0x27) {}

                    Mlx90614::TempRegister object;
                    Mlx90614::TempRegister ambient;
                protected:
                    SMBusDevice dev;
                };
            }
        }
    }
}
