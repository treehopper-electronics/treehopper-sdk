#pragma once

#include "Treehopper.Libraries.h"
#include "Adxl345Registers.h"
#include "Accelerometer.h"

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Inertial {
                class LIBRARIES_API Adxl345 : virtual public Accelerometer {
                public:
                    Adxl345(I2c &i2c, bool altAddress = false, int rate = 100);

                    void update();

                private:
                    Adxl345Registers registers;
                    SMBusDevice _dev;
                };
            }
        }
    }
}
