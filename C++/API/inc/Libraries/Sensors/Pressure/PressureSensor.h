#pragma once

#include "Libraries/Treehopper.Libraries.h"
#include "Libraries/Pollable.h"

using namespace Treehopper::Libraries;

namespace Treehopper {
    namespace Libraries {
        namespace Sensors {
            namespace Pressure {
                class LIBRARIES_API PressureSensor : virtual public Pollable {
                public:
                    double bar();

                    double atm();

                    double psi();

                    double pascal();

                protected:
                    double _pascal;
                };
            }
        }
    }
}