#include "Displays/Dm632.h"
#include "Displays/Utility.h"
#include <math.h>
namespace Treehopper {
	namespace Libraries {
		namespace Displays {
			Dm632::Dm632(Spi & spi, SpiChipSelectPin * lat, double speed) : ChainableShiftRegisterOutput(spi, lat, 32, speed), LedDriver(16, true, true)
			{
			}

			Dm632::Dm632(ChainableShiftRegisterOutput & upstreamDevice) : ChainableShiftRegisterOutput(upstreamDevice, 32), LedDriver(16, true, true)
			{
			}

			void Dm632::setGlobalBrightness(double _brightness)
			{

			}

			void Dm632::ledStateChanged(Led led)
			{
				update(led);
			}

			void Dm632::ledBrightnessChanged(Led led)
			{
				update(led);
			}

			void Dm632::flush(bool force)
			{
				ChainableShiftRegisterOutput::flush(force);
			}

			void Dm632::updateFromCurrentValue()
			{
				for(unsigned int i = 0; i < leds.size(); i++)
				{
					leds[i].state(true);
					uint16_t curr = currentValue[i * 2] << 8 | currentValue[i * 2 + 1];
					leds[i].brightness((double)curr / UINT16_MAX);
				}
			}

			void Dm632::update(Led& led)
			{
				if (led.state())
				{
					uint16_t brightness = (uint16_t)round(Utility::brightnessToCieLuminance(led.brightness() * _brightness) * 65535);
					currentValue[led.channel * 2 + 1] = (uint8_t)(brightness >> 8);
					currentValue[led.channel * 2] = (uint8_t)(brightness & 0xFF);
				}
				else
				{
					currentValue[led.channel * 2] = 0x00;
					currentValue[led.channel * 2 + 1] = 0x00;
				}
			}
		}
	}
}