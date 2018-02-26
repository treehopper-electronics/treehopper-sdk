#pragma once

#include "Treehopper.Libraries.h"
#include "Displays/Led.h"
#include "Interface/Flushable.h"
#include <vector>

namespace Treehopper {
    namespace Libraries {
        namespace Displays {

            using namespace std;
            using namespace Treehopper::Libraries::Interface;

            /// <summary>
            /// Base class that all LED drivers inherit from.
            /// </summary>
            class LIBRARIES_API LedDriver : public Flushable {
                friend Led;
            public:
                /// <summary>
                /// Construct an LedDriver
                /// </summary>
                /// <param name="numLeds">The number of LEDs to construct</param>
                /// <param name="hasGlobalBrightnessControl">Whether the controller can globally adjust the LED _brightness.</param>
                /// <param name="hasIndividualBrightnessControl">Whether the controller has individual LED _brightness control</param>
                LedDriver(int numLeds, bool hasGlobalBrightnessControl, bool hasIndividualBrightnessControl);

                /// <summary>
                /// The collection of LEDs that belong to this driver.
                /// </summary>
                vector<Led> leds;

                /// <summary>
                /// Gets or sets whether this controller supports global _brightness control.
                /// </summary>
                const bool hasGlobalBrightnessControl;

                /// <summary>
                /// Gets or sets whether this controller's LEDs have individual _brightness control (through PWM or otherwise).
                /// </summary>
                const bool hasIndividualBrightnessControl;

                /** Gets the global brightness of the display. */
                double brightness();

                /** Sets the global brightness of the display.

                If the display's hasGlobalBrightnessControl is true, native brightness control will be used. Otherwise, if the display's hasIndividualBrightnessControl is true, global brightness will be emulated by setting all

                */
                void brightness(double value);

            protected:
                double _brightness = 0.0;
            private:
                virtual void setGlobalBrightness(double _brightness) = 0;

                /// <summary>
                /// Called by the LED when the _state changes
                /// </summary>
                /// <param name="led">The LED whose _state changed</param>
                virtual void ledStateChanged(Led led) = 0;

                /// <summary>
                /// Called by the LED when the _brightness changed
                /// </summary>
                /// <param name="led">The LED whose _brightness changed</param>
                virtual void ledBrightnessChanged(Led led) = 0;

                /// <summary>
                /// clear the display
                /// </summary>
                /// <returns>An awaitable task that completes when the display is cleared</returns>
                void clear();
            };
        }
    }
}