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
                class LIBRARIES_API Mlx90614 {
                public:
                    class TempRegister : public virtual TemperatureSensor {
                    public:
                        TempRegister(SMBusDevice &dev, uint8_t reg);

                        void update();

                    private:
                        SMBusDevice &dev;
                        uint8_t reg;
                    };

                    Mlx90614(I2c &i2c, int rate = 100);

                    TempRegister object;
                    TempRegister ambient;

                protected:
                    SMBusDevice dev;
                };
            }
        }
    }
}
