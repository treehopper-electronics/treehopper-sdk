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
		this->pinNumber = pinNumber;
		this->board = board;
		this->spiModule = &board->spi;
		_mode = PinMode::Reserved;
		referenceLevel = AdcReferenceLevel::VREF_3V3;
	}
	void Pin::makePushPullOutput()
	{
		mode(PinMode::PushPullOutput);
	}

	void Pin::makeDigitalInput()
	{
		mode(PinMode::DigitalInput);
	}

	void Pin::makeAnalogInput()
	{
		mode(PinMode::AnalogInput);
	}

	void Pin::digitalValue(bool value)
	{
		if (_mode != PinMode::PushPullOutput && _mode != PinMode::OpenDrainOutput)
			makePushPullOutput();

		DigitalOut::digitalValue(value);
	}

	bool Pin::digitalValue()
	{
		if (_mode == PinMode::DigitalInput) {
			return DigitalIn::_digitalValue;
		} else {
			return DigitalOut::_digitalValue;
		}
	}
	void Pin::toggleOutput()
	{
		digitalValue(!digitalValue());
	}

	AdcReferenceLevel Pin::getReferenceLevel()
	{
		return referenceLevel;
	}

	void Pin::setReferenceLevel(AdcReferenceLevel value)
	{
		if (referenceLevel == value) return;

		referenceLevel = value;

		if (_mode == PinMode::AnalogInput)
		{
			uint8_t cmd[2];
			cmd[0] = (uint8_t)PinConfigCommands::MakeAnalogInput;
			cmd[1] = 0;
			SendCommand(cmd, 2);
		}
	}

	void Pin::updateValue(uint8_t high, uint8_t low)
	{
		uint16_t newVal = (((uint16_t)high) << 8) | low;
		if (_mode == PinMode::DigitalInput)
		{
			DigitalIn::update(newVal > 0);
		}
		else if (_mode == PinMode::AnalogInput)
		{
			double voltage = 0;

			switch (referenceLevel)
			{
			case AdcReferenceLevel::VREF_3V3:
				voltage = (double)newVal * 3.3 / 4092;
				break;

			case AdcReferenceLevel::VREF_1V65:
				voltage = (double)newVal * 1.65 / 4092;
				break;

			case AdcReferenceLevel::VREF_1V8:
				voltage = (double)newVal * 1.8 / 4092;
				break;

			case AdcReferenceLevel::VREF_2V4:
				voltage = (double)newVal * 2.4 / 4092;
				break;

			case AdcReferenceLevel::VREF_3V6:
				voltage = (double)newVal * 3.6 / 4092;
				break;

			case AdcReferenceLevel::VREF_3V3_DERIVED:
				voltage = (double)newVal * 3.3 / 4092;
				break;
			}


			if (_adcValue != newVal) // compare the actual ADC values, not the floating-point conversions.
			{
				_adcValue = newVal;
				_analogVoltage = voltage;
				if (AnalogValueChanged != NULL)
				{
					AnalogValueChanged(_adcValue);
				}
				if (AnalogVoltageChanged != NULL)
				{
					AnalogVoltageChanged(_analogVoltage);
				}
			}
		}
	}

	double Pin::analogValue()
	{
		return _analogValue;
	}

	double Pin::analogVoltage()
	{
		return _analogVoltage;
	}

	int Pin::adcValue()
	{
		return _adcValue;
	}

	void Pin::mode(PinMode value)
	{
		if (value == _mode) return;

		_mode = value;

		uint8_t cmd[2];
		switch (_mode)
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

	PinMode Pin::mode()
	{
		return _mode;
	}

	void Pin::writeOutputValue()
	{
		uint8_t cmd[] = { (uint8_t)PinConfigCommands::SetDigitalValue, DigitalOut::_digitalValue };
		SendCommand(cmd, 2);
	}

	void Pin::SendCommand(uint8_t* cmd, size_t len)
	{
		uint8_t data[6];
		data[0] = pinNumber;
		memcpy(&data[1], cmd, len);
		board->sendPinConfigPacket(data, 6);
	}
}