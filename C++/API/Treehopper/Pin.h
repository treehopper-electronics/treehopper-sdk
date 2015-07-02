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
	 void SetDigitalValue(bool val);
	 bool GetDigitalValue();
	 void ToggleOutput();
	function<void(bool)> ValueChanged;
	Property<bool> Value;
	PinState State;

protected:
	TreehopperBoard* Board;
	uint8_t PinNumber;
	virtual void UpdateValue(uint8_t high, uint8_t low);
	bool digitalValue;
	void SendCommand(uint8_t* data, int length);
};
