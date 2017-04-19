#include "Utility.h"
#include "Displays/Led.h"
#include "Displays/LedDriver.h"
namespace Treehopper {
	namespace Libraries {
		namespace Displays {
			using namespace Treehopper;
			Led::Led(LedDriver& driver, int channel, bool hasBrightnessControl) : driver(driver), channel(channel), hasBrightnessControl(hasBrightnessControl)
			{
			}
			void Led::brightness(double value)
			{
				if (Utility::closeTo(value, _brightness)) return;
				if (!hasBrightnessControl) return;
				_brightness = value;
				driver.ledBrightnessChanged(*this);

				if (driver.autoFlush)
					driver.flush();
			}
			double Led::brightness()
			{
				return _brightness;
			}
			bool Led::state()
			{
				return _state;
			}
			void Led::state(bool value)
			{
				if (_state == value) return;
				_state = value;
				driver.ledStateChanged(*this);

				if (driver.autoFlush)
					driver.flush();
			}
		}
	}
}
