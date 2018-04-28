#include "Libraries/Displays/LedDriver.h"
#include "Utility.h"

namespace Treehopper {
    namespace Libraries {
        namespace Displays {
            using namespace Treehopper;

            LedDriver::LedDriver(int numLeds, bool hasGlobalBrightnessControl, bool hasIndividualBrightnessControl)
                    : hasGlobalBrightnessControl(hasGlobalBrightnessControl),
                      hasIndividualBrightnessControl(hasIndividualBrightnessControl) {
                for (int i = 0; i < numLeds; i++) {
                    leds.emplace_back(*this, i, hasIndividualBrightnessControl);
                }
                _brightness = 1.0; // we can't call brightness() here since it contains calls to virtual functions
            }

            double LedDriver::brightness() {
                return _brightness;
            }

            void LedDriver::brightness(double value) {
                if (Utility::closeTo(value, _brightness)) return;
                if (value < 0 || value > 1)
                    Utility::error("Valid _brightness is from 0 to 1");
                _brightness = value;

                if (hasGlobalBrightnessControl) // might be more efficient
                    setGlobalBrightness(_brightness);
                else if (hasIndividualBrightnessControl) {
                    bool savedAutoflushState = autoFlush;
                    autoFlush = false;
                    for (auto led : leds)
                        led.brightness(_brightness);
                    flush();
                    autoFlush = savedAutoflushState;
                }
            }

            void LedDriver::clear() {
                bool autoflushSave = autoFlush;
                autoFlush = false;
                for (auto led : leds) {
                    led.state(false);
                }

                flush(true);
                autoFlush = autoflushSave;
            }
        }
    }
}
