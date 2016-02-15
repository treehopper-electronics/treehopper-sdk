#include "Pwm.h"
#include "Pin.h"
#include "TreehopperBoard.h"
#include <cmath>

	Pwm::Pwm(Pin* pin)
	{
		_pin = pin;

		IsEnabled = Property<bool>();
		IsEnabled.Getter = [this]()
		{
			return isEnabled;
		};
		IsEnabled.Setter = [this](bool val)
		{
			isEnabled = val;
			if (isEnabled)
			{
				uint8_t data[] = { CmdMakePWMPin, frequency };
				_pin->SendCommand(data, 2);
				_pin->State = PinStatePWM;
			}
			else
			{
				uint8_t data[] = { CmdMakePWMPin, 0xFF };
				_pin->SendCommand(data, 2); // magic byte to disable PWM module
			}
		};

		Frequency = Property<PwmFrequency>();
		Frequency.Getter = [this]() 
		{ 
			return frequency;
		};
		Frequency.Setter = [this](PwmFrequency freq)
		{
			frequency = freq;
			if (IsEnabled)
			{
				uint8_t data[] = { CmdMakePWMPin, frequency };
				_pin->SendCommand(data, 2);
			}
		};

		DutyCycle = Property<double>();
		DutyCycle.Getter = [this]() 
		{ 
			return dutyCycle; 
		};
		DutyCycle.Setter = [this](double val) 
		{ 
			dutyCycle = val;
			uint16_t reg = (uint16_t)round(dutyCycle * 1024.0);
			uint8_t high = reg >> 2;
			uint8_t low = reg & 0x3;
			uint8_t data[] = { CmdSetPWMValue, high, low };
			_pin->SendCommand(data, 2);
		};

		PulseWidth = Property<double>();
		PulseWidth.Getter = [this]()
		{
			return pulseWidth;
		};
		PulseWidth.Setter = [this](double value)
		{
			pulseWidth = value;
		};

		Frequency = Freq_750HZ;

	}
