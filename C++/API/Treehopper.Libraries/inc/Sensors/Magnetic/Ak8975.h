#pragma once

#include <I2c.h>
#include "Treehopper.Libraries.h"
#include "Magnetometer.h"
#include "Ak8975Registers.h"

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Magnetic {
                class LIBRARIES_API Ak8975 : public virtual Magnetometer {
                public:
                    Ak8975(I2c &i2c, int rate = 100);

                    void update();

                private:
                    SMBusDevice dev;
                    Ak8975Registers registers;
                };
            }
        }
    }
}