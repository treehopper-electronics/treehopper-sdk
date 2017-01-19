#include <stdint.h>
#include <functional>
#include "Pin.h"
#include "TreehopperBoard.h"
#include "Property.h"

Pin::Pin(uint8_t pinNumber, TreehopperBoard* board)
{
	DigitalValue = Property<bool>();
	DigitalValue.Getter = [this]() { return digitalValue;  };
	DigitalValue.Setter = [this](bool val){ digitalValue = val;  SetDigitalValue(val);  };
	PinNumber = pinNumber;
	Board = board;
	State = PinStateReservedPin;
}
void Pin::MakeDigitalOutput()
{
	if (State == PinStateDigitalOutput)
		return;
	uint8_t cmd = CmdMakeDigitalOutput;
	SendCommand(&cmd, 1);
	State = PinStateDigitalOutput;
}

void Pin::MakeDigitalInput()
{
	if (State == PinStateDigitalInput)
		return;
	uint8_t cmd = CmdMakeDigitalInput;
	SendCommand(&cmd, 1);
	State = PinStateDigitalInput;
}

void Pin::MakeAnalogInput()
{
	if (State == PinStateAnalogInput)
		return;
	uint8_t cmd = CmdMakeAnalogInput;
	SendCommand(&cmd, 1);
	State = PinStateAnalogInput;
}

void Pin::SetDigitalValue(bool val)
{
	if (State != PinStateDigitalOutput)
		MakeDigitalOutput();
	uint8_t cmd[] = { CmdSetDigitalValue, val };
	SendCommand(cmd, 2);
}
bool Pin::GetDigitalValue()
{
	return digitalValue;
}
void Pin::ToggleOutput()
{
	DigitalValue = !DigitalValue;
}

void Pin::UpdateValue(uint8_t high, uint8_t low)
{
	if (State == PinStateDigitalInput)
	{
		bool newVal = (((uint16_t)high) << 8) + low;
		if (newVal != digitalValue)
		{
			digitalValue = newVal;
			if (DigitalValueChanged != NULL)
				DigitalValueChanged(digitalValue > 0);
		}
	}
	else if (State == PinStateAnalogInput)
	{
		int val = ((int)high) << 8;
		val += (int)low;
		double voltage = (double)val / 204.8f;

		if (AnalogValue != val) // compare the actual ADC values, not the floating-point conversions.
		{
			AnalogValue = val;
			AnalogVoltage = voltage;
			if (AnalogValueChanged != NULL)
			{
				AnalogValueChanged(AnalogValue);
			}
			if (AnalogVoltageChanged != NULL)
			{
				AnalogVoltageChanged(AnalogVoltage);
			}
		}
	}
	

}

void Pin::SendCommand(byte* data, int length)
{
	Board->SendPinConfigCommand(PinNumber, data, length);
}