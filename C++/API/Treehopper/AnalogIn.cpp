#include "AnalogIn.h"
#include "TreehopperBoard.h"
#include "Pin.h"
#include <cmath>

AnalogIn::AnalogIn(Pin* Pin)
{
	pin = Pin;

	IsEnabled.Getter = 
		[&]() 
		{ 
			return isEnabled;  
		};

	IsEnabled.Setter = 
		[&](bool val)
		{
			isEnabled = val;
			if (isEnabled)
			{
				if (pin->State == PinStateAnalogInput)
					return;
				uint8_t cmd = CmdMakeAnalogInput;
				pin->SendCommand(&cmd, 1);
				pin->State = PinStateAnalogInput;
			}
			else
			{
				pin->MakeDigitalInput();
			}
			
		};
}


void AnalogIn::UpdateAnalogValue(uint8_t highByte, uint8_t lowByte)
{
	int val = ((int)highByte) << 8;
	val += (int)lowByte;
	double analogVal = (double)val / 204.8f;

	if (Value != val) // compare the actual ADC values, not the floating-point conversions.
	{
		Value = val;
		Voltage = analogVal;
		if (ValueChanged != NULL)
		{
			ValueChanged(Value);
		}
		if (VoltageChanged != NULL)
		{
			VoltageChanged(Voltage);
		}
	}
}
