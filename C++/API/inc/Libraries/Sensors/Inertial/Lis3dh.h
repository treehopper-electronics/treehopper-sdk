#pragma once

#include "Treehopper.Libraries.h"
#include "Sensors/Inertial/Lis3dhRegisters.h"
#include "Accelerometer.h"

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Inertial {
                class LIBRARIES_API Lis3dh : public virtual Accelerometer {
                public:
                    Lis3dh(I2c &i2c, bool sdo = false, int rate = 100);

                    void update();

                private:
                    Lis3dhRegisters registers;
                    SMBusDevice _dev;
                };
            }
        }
    }
}