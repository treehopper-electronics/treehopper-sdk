#include <stdint.h>
#include <functional>
#include "Pin.h"
#include "TreehopperBoard.h"
#include "Property.h"

Pin::Pin(uint8_t pinNumber, TreehopperBoard* board)
{
	Value = Property<bool>();
	Value.Getter = [this]() { return digitalValue;  };
	Value.Setter = [this](bool val){ digitalValue = val;  SetDigitalValue(val);  };
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
	Value = !Value;
}

void Pin::UpdateValue(uint8_t high, uint8_t low)
{
	bool newVal = (((uint16_t)high) << 8) + low;
	if (newVal != digitalValue)
	{
		digitalValue = newVal;
		if (ValueChanged != NULL)
			ValueChanged(digitalValue > 0);
	}
}

void Pin::SendCommand(byte* data, int length)
{
	Board->SendPinConfigCommand(PinNumber, data, length);
}