#include "stdafx.h"
#include <stdint.h>
#include <functional>
#include "Pin.h"
#include "TreehopperUsb.h"

enum PinConfigCommands
{
	PinConfigReserved,
	PinConfigMakeDigitalInput,
	PinConfigMakePushPullOutput,
	PinConfigMakeOpenDrainOutput,
	PinConfigMakeAnalogInput,
	PinConfigSetDigitalValue,
};

Pin::Pin(TreehopperUsb* board, uint8_t pinNumber)
{
	PinNumber = pinNumber;
	this->board = board;
	pinMode = PinModeReservedPin;
	referenceLevel = VREF_3V3;
}
void Pin::makePushPullOutput()
{
	setMode(PinModePushPullOutput);
}

void Pin::makeDigitalInput()
{
	setMode(PinModeDigitalInput);
}

void Pin::makeAnalogInput()
{
	setMode(PinModeAnalogInput);
}

void Pin::setDigitalValue(bool value)
{
	if (value == digitalValue) return;

	digitalValue = value;

	if (pinMode != PinModePushPullOutput && pinMode != PinModeOpenDrainOutput)
		makePushPullOutput();

	uint8_t cmd[] = { PinConfigSetDigitalValue, value };
	SendCommand(cmd, 2);
}
bool Pin::getDigitalValue()
{
	return digitalValue;
}
void Pin::toggleOutput()
{
	setDigitalValue(!getDigitalValue());
}

AdcReferenceLevel Pin::getReferenceLevel()
{
	return referenceLevel;
}

void Pin::setReferenceLevel(AdcReferenceLevel value)
{
	if (referenceLevel == value) return;
	
	referenceLevel = value;

	if (pinMode == PinModeAnalogInput)
	{
		uint8_t cmd[2];
		cmd[0] = PinConfigMakeAnalogInput;
		cmd[1] = 0;
		SendCommand(cmd, 2);
	}
}

void Pin::updateValue(uint8_t high, uint8_t low)
{
	if (pinMode == PinModeDigitalInput)
	{
		bool newVal = (((uint16_t)high) << 8) + low;
		if (newVal != digitalValue)
		{
			digitalValue = newVal;
			if (DigitalValueChanged != NULL)
				DigitalValueChanged(digitalValue > 0);
		}
	}
	else if (pinMode == PinModeAnalogInput)
	{
		int val = ((int)high) << 8;
		val += (int)low;
		double voltage = 0;
		
		switch (referenceLevel)
		{
		case VREF_3V3:
			voltage = (double)val * 3.3 / 4092;
			break;

		case VREF_1V65:
			voltage = (double)val * 1.65 / 4092;
			break;

		case VREF_1V8:
			voltage = (double)val * 1.8 / 4092;
			break;

		case VREF_2V4:
			voltage = (double)val * 2.4 / 4092;
			break;

		case VREF_3V6:
			voltage = (double)val * 3.6 / 4092;
			break;

		case VREF_3V3_DERIVED:
			voltage = (double)val * 3.3 / 4092;
			break;
		}
		

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

void Pin::setMode(PinMode value)
{
	if (value == pinMode) return;
	
	pinMode = value;
	
	uint8_t cmd[2];
	switch (pinMode)
	{
	case PinModeAnalogInput:
		cmd[0] = PinConfigMakeAnalogInput;
		cmd[1] = 0;
		SendCommand(cmd, 2);
		break;
	case PinModeDigitalInput:
		cmd[0] = PinConfigMakeDigitalInput;
		cmd[1] = 0;
		SendCommand(cmd, 2);
		break;
	case PinModeOpenDrainOutput:
		cmd[0] = PinConfigMakeOpenDrainOutput;
		cmd[1] = 0;
		SendCommand(cmd, 2);
		break;
	case PinModePushPullOutput:
		cmd[0] = PinConfigMakePushPullOutput;
		cmd[1] = 0;
		SendCommand(cmd, 2);
		break;
	}
}

void Pin::SendCommand(uint8_t* cmd, int len)
{
	uint8_t data[6];
	data[0] = PinNumber;
	memcpy(&data[1], cmd, len);
	board->sendPinConfigPacket(data, 6);
}