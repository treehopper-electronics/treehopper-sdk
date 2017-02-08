#pragma once

#include "Property.h"
#include <stdint.h>

#ifdef TREEHOPPER_EXPORTS
#define EXPORT __declspec(dllexport)
#else
#define EXPORT __declspec(dllimport)
#endif

using namespace std;

class TreehopperBoard;
enum PinState;

enum PinState
{
	PinStateReservedPin,
	PinStateDigitalInput,
	PinStateDigitalOutput,
	PinStateAnalogInput,
	PinStateAnalogOutput,
	PinStatePWM
};

class EXPORT Pin
{
	friend class TreehopperBoard;
	friend class AnalogIn;
public:
	 Pin(uint8_t pinNumber, TreehopperBoard* board);
	 void MakeDigitalOutput();
	 void MakeDigitalInput();
	 void MakeAnalogInput();
	 void SetDigitalValue(bool val);
	 bool GetDigitalValue();
	 void ToggleOutput();

	 PinState State;

	 // Digital stuff
	function<void(bool)> DigitalValueChanged;
	Property<bool> DigitalValue;

	// analog stuff
	int AnalogValue;
	double AnalogVoltage;
	function<void(int)> AnalogValueChanged;
	function<void(double)> AnalogVoltageChanged;

	void SendCommand(uint8_t* data, int length);

protected:
	TreehopperBoard* Board;
	uint8_t PinNumber;
	virtual void UpdateValue(uint8_t high, uint8_t low);
	bool digitalValue;
	
};
