#pragma once

#include <Libraries/Sensors/ProximitySensor.h>
#include <I2c.h>
#include <SMBusDevice.h>
#include "Libraries/Treehopper.Libraries.h"
#include "AmbientLightSensor.h"
#include "Vcnl4010Registers.h"

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Optical {
                class LIBRARIES_API Vcnl4010 : public virtual ProximitySensor, public virtual AmbientLightSensor {
                public:
                    Vcnl4010(I2c &i2c, int rate = 100);

                    void update();

                    double rawProximity();

                private:
                    SMBusDevice dev;
                    Vcnl4010Registers registers;
                    double _rawProximity;
                };
            }
        }
    }
}
