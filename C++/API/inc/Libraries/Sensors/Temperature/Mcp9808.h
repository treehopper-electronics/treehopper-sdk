#pragma once

#include "Treehopper.Libraries.h"
#include "Sensors/Temperature/TemperatureSensor.h"
#include "I2c.h"
#include "SMBusDevice.h"

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Temperature {
                class LIBRARIES_API Mcp9808 : public virtual TemperatureSensor {
                public:
                    Mcp9808(I2c &i2c, uint8_t address);

                    Mcp9808(I2c &i2c, bool a0 = false, bool a1 = false, bool a2 = false);

                    void update();

                protected:
                    SMBusDevice dev;
                };
            }
        }
    }
}
