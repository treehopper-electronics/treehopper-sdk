#pragma once

#include "Libraries/Treehopper.Libraries.h"
#include "TemperatureSensor.h"
#include "I2c.h"
#include "SMBusDevice.h"
#include <memory>

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Temperature {
                class LIBRARIES_API Lm75 : public virtual TemperatureSensor {
                public:
                    Lm75(I2c &i2c, bool a0 = false, bool a1 = false, bool a2 = false);

                    void update();

                private:
                    SMBusDevice dev;
                };
            }
        }
    }
}
