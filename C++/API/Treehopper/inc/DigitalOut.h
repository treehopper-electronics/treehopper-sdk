#pragma once

#include "Treehopper.h"
#include "Event.h"

namespace Treehopper
{
	using namespace std;

	class TREEHOPPER_API DigitalOut
	{
	public:
		virtual void makePushPullOutput() = 0;
		virtual bool digitalValue()
		{
			return _digitalValue;
		}

		virtual void digitalValue(bool value)
		{
			if (_digitalValue == value) return;

			_digitalValue = value;
			writeOutputValue();
		}
	protected:
		bool _digitalValue;
		virtual void writeOutputValue() = 0;
	};
}