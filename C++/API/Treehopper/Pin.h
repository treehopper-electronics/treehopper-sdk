#pragma once

#include "Treehopper.h"
#include <stdint.h>
#include <functional>

using namespace std;

class TreehopperUsb;
enum PinMode;

enum AdcReferenceLevel
{
	VREF_3V3,
	VREF_1V65,
	VREF_1V8,
	VREF_2V4,
	VREF_3V3_DERIVED,
	VREF_3V6
};


enum PinMode
{
	PinModeReservedPin,
	PinModeDigitalInput,
	PinModePushPullOutput,
	PinModeOpenDrainOutput,
	PinModeAnalogInput,
	Unassigned
};

class TREEHOPPER_API Pin
{
	friend class TreehopperUsb;
	
public:
	 Pin(TreehopperUsb* board, uint8_t pinNumber);
	 void setMode(PinMode value);
	 void makePushPullOutput();
	 void makeDigitalInput();
	 void makeAnalogInput();
	 void setDigitalValue(bool val);
	 bool getDigitalValue();
	 void ToggleOutput();

	 AdcReferenceLevel getReferenceLevel();
	 void setReferenceLevel(AdcReferenceLevel value);

	 // Digital stuff
	function<void(bool)> DigitalValueChanged;

	// analog stuff
	int AnalogValue;
	double AnalogVoltage;
	function<void(int)> AnalogValueChanged;
	function<void(double)> AnalogVoltageChanged;

	void SendCommand(uint8_t* data, int length);

protected:
	TreehopperUsb* board;
	uint8_t PinNumber;
	virtual void updateValue(uint8_t high, uint8_t low);
	bool digitalValue;
	PinMode pinMode;
	AdcReferenceLevel referenceLevel;
};
