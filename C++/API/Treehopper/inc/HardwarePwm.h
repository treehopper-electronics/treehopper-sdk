#pragma once

#include "Treehopper.h"
#include "Pwm.h"

namespace Treehopper 
{
	class Pin;
	class TreehopperUsb;
	/** A hardware PWM pin */
	class TREEHOPPER_API HardwarePwm : public Pwm
	{
		friend class HardwarePwmManager;
	public:
		HardwarePwm(TreehopperUsb* board, int pinNumber);
		~HardwarePwm();
		/** Gets whether this hardware PWM pin is enabled */
		virtual bool enabled();

		/** Sets whether this hardware PWM pin is enabled */
		virtual void enabled(bool value);
	protected:
		void updateDutyCycle();
		bool _enabled;
		int pinNumber;
		TreehopperUsb* board;
	};
}

