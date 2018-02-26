#pragma once

#include "Treehopper.Libraries.h"
#include "Displays/LedDriver.h"
#include "Interface/PortExpander/ChainableShiftRegisterOutput.h"
#include <bitset>

namespace Treehopper {
    namespace Libraries {
        namespace Displays {

            using namespace Treehopper::Libraries::Interface::PortExpander;

            /** Library for the SiTI DM632, DM633, and DM634 16-channel, 16-bit PWM-capable shift-register-type LED driver */
            class LIBRARIES_API Dm632 : public ChainableShiftRegisterOutput, public LedDriver {
            public:
                /**
                Construct a DM632 16-channel, 16-bit PWM LED controller attached directly to an SPI port
                \param[in] spi	The SPI port this LED driver is attached to
                \param[in] lat		The GPIO pin attached to the LAT pin
                \param[in] speed	The speed, in MHz, to use when communicating
                */
                Dm632(Spi &spi, SpiChipSelectPin *lat, double speed = 6);

                /**
                Construct a DM632 16-channel, 16-bit PWM LED controller attached directly to an SPI port
                \param[in]	upstreamDevice	The upstream shift register this DM632 is attached to
                */
                Dm632(ChainableShiftRegisterOutput &upstreamDevice);

                virtual void flush(bool force = false);

                virtual void updateFromCurrentValue();

            private:
                virtual void setGlobalBrightness(double _brightness);

                virtual void ledStateChanged(Led led);

                virtual void ledBrightnessChanged(Led led);

                void update(Led &led);
            };
        }
    }
}
