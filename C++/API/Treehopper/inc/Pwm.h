#pragma once
#include "Treehopper.h"
#include "Utility.h"
namespace Treehopper
{
	class TREEHOPPER_API Pwm
	{
	public:
		~Pwm() { };

		virtual double dutyCycle() { return _dutyCycle; }
		virtual void dutyCycle(double value) {
			if (Utility::closeTo(_dutyCycle, value))
				return;

			_dutyCycle = value;
			updateDutyCycle();
		}

		virtual bool enabled() = 0;
		virtual void enabled(bool value) = 0;

		virtual double pulseWidth() { 
			// always calculate from frequency and dc
			return _dutyCycle * 1000000.0 / _frequency;
		}
		virtual void pulseWidth(double value) {
			if (Utility::closeTo(pulseWidth(), value))
				return;

			_dutyCycle = value * _frequency / 1000000.0;
			updateDutyCycle();
		}

	protected:
		virtual void updateDutyCycle() = 0;
		double _dutyCycle;
		double _frequency;
	};
}