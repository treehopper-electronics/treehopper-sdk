#pragma once

#include "Spi.h"
#include "DigitalOut.h"

namespace Treehopper {
    class TREEHOPPER_API SpiChipSelectPin : public DigitalOut {
    public:
        /** Gets the pin number of the pin */
        int pinNumber;

        /** Gets the SPI module this pin can be used with */
        Spi *spiModule;
    };
}