#include "Property.h"
#include <functional>
#include "Pin.h"
#pragma once
#include <stdint.h>
class AnalogIn
{
public:
	AnalogIn(Pin* Pin);
	void UpdateAnalogValue(uint8_t highByte, uint8_t lowByte);
	function<void(int)> ValueChanged;
	function<void(double)> VoltageChanged;
	Pin* pin;
	bool isEnabled;
	Property<bool> IsEnabled;
	int Value;
	double Voltage;
};