#pragma once

#include "Property.h"
#include <stdint.h>
#include "Pin.h"

#ifdef TREEHOPPER_STATIC_LINK
#define TREEHOPPER_API
#else
#ifdef TREEHOPPER_EXPORTS
#define TREEHOPPER_API __declspec(dllexport)
#else
#define TREEHOPPER_API __declspec(dllimport)
#endif
#endif

enum PwmFrequency
{
	/// <summary>
	/// 48 kHz (20.833 microseconds)
	/// </summary>
	Freq_48KHz,

	/// <summary>
	/// 12 kHz (83.333 microseconds)
	/// </summary>
	Freq_12KHz,

	/// <summary>
	/// 3 kHz (333.333 microseconds)
	/// </summary>
	Freq_3KHz,

	/// <summary>
	/// 750 Hz (1.333 milliseconds)
	/// </summary>
	Freq_750HZ
};


class TREEHOPPER_API Pwm
{
	//friend class Pin;

public:
	Pwm(Pin* pin);
	Property<PwmFrequency> Frequency;
	Property<bool> IsEnabled;
	Property<double> DutyCycle;
	Property<double> PulseWidth;

	//Pin Pin;
	//double dutyCycle;
	//double pulseWidth;
	//double period;
	//private bool isEnabled = false;
	//private PwmFrequency frequency = PwmFrequency.Freq_750HZ;
private:
	Pin* _pin;
	PwmFrequency frequency;
	double dutyCycle;
	bool isEnabled;
	double pulseWidth;
};