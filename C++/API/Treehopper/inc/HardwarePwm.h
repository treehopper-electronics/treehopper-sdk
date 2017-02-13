#pragma once

#include "Treehopper.h"
#include "Pwm.h"

namespace Treehopper 
{
	class Pin;
	class TreehopperUsb;

	class TREEHOPPER_API HardwarePwm : public Pwm
	{
		friend class HardwarePwmManager;
	public:
		HardwarePwm(TreehopperUsb* board, int pinNumber);
		~HardwarePwm();
		virtual bool enabled();
		virtual void enabled(bool value);
	protected:
		void updateDutyCycle();
		bool _enabled;
		int pinNumber;
		TreehopperUsb* board;
	};
}

