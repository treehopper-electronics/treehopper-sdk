#include "stdafx.h"
#include <stdint.h>
#include <functional>
#include "Pin.h"
#include "TreehopperUsb.h"
namespace Treehopper 
{
	enum class PinConfigCommands
	{
		Reserved = 0,
		MakeDigitalInput,
		MakePushPullOutput,
		MakeOpenDrainOutput,
		MakeAnalogInput,
		SetDigitalValue,
	};

	Pin::Pin(TreehopperUsb* board, uint8_t pinNumber)
	{
		PinNumber = pinNumber;
		this->board = board;
		pinMode = PinMode::Reserved;
		referenceLevel = AdcReferenceLevel::VREF_3V3;
	}
	void Pin::makePushPullOutput()
	{
		setMode(PinMode::PushPullOutput);
	}

	void Pin::makeDigitalInput()
	{
		setMode(PinMode::DigitalInput);
	}

	void Pin::makeAnalogInput()
	{
		setMode(PinMode::AnalogInput);
	}

	void Pin::setDigitalValue(bool value)
	{
		if (value == digitalValue) return;

		digitalValue = value;

		if (pinMode != PinMode::PushPullOutput && pinMode != PinMode::OpenDrainOutput)
			makePushPullOutput();

		uint8_t cmd[] = { (uint8_t)PinConfigCommands::SetDigitalValue, value };
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

		if (pinMode == PinMode::AnalogInput)
		{
			uint8_t cmd[2];
			cmd[0] = (uint8_t)PinConfigCommands::MakeAnalogInput;
			cmd[1] = 0;
			SendCommand(cmd, 2);
		}
	}

	void Pin::updateValue(uint8_t high, uint8_t low)
	{
		if (pinMode == PinMode::DigitalInput)
		{
			bool newVal = (((uint16_t)high) << 8) + low;
			if (newVal != digitalValue)
			{
				digitalValue = newVal;
				if (DigitalValueChanged != NULL)
					DigitalValueChanged(digitalValue > 0);
			}
		}
		else if (pinMode == PinMode::AnalogInput)
		{
			int val = ((int)high) << 8;
			val += (int)low;
			double voltage = 0;

			switch (referenceLevel)
			{
			case AdcReferenceLevel::VREF_3V3:
				voltage = (double)val * 3.3 / 4092;
				break;

			case AdcReferenceLevel::VREF_1V65:
				voltage = (double)val * 1.65 / 4092;
				break;

			case AdcReferenceLevel::VREF_1V8:
				voltage = (double)val * 1.8 / 4092;
				break;

			case AdcReferenceLevel::VREF_2V4:
				voltage = (double)val * 2.4 / 4092;
				break;

			case AdcReferenceLevel::VREF_3V6:
				voltage = (double)val * 3.6 / 4092;
				break;

			case AdcReferenceLevel::VREF_3V3_DERIVED:
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
		case PinMode::AnalogInput:
			cmd[0] = (uint8_t)PinConfigCommands::MakeAnalogInput;
			cmd[1] = 0;
			SendCommand(cmd, 2);
			break;
		case PinMode::DigitalInput:
			cmd[0] = (uint8_t)PinConfigCommands::MakeDigitalInput;
			cmd[1] = 0;
			SendCommand(cmd, 2);
			break;
		case PinMode::OpenDrainOutput:
			cmd[0] = (uint8_t)PinConfigCommands::MakeOpenDrainOutput;
			cmd[1] = 0;
			SendCommand(cmd, 2);
			break;
		case PinMode::PushPullOutput:
			cmd[0] = (uint8_t)PinConfigCommands::MakePushPullOutput;
			cmd[1] = 0;
			SendCommand(cmd, 2);
			break;
		}
	}

	void Pin::SendCommand(uint8_t* cmd, size_t len)
	{
		uint8_t data[6];
		data[0] = PinNumber;
		memcpy(&data[1], cmd, len);
		board->sendPinConfigPacket(data, 6);
	}
}