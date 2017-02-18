#pragma once

#include "Treehopper.h"
#include <functional>
#include <vector>
#include "Event.h"

namespace Treehopper 
{
	using namespace std;

	class DigitalIn;

	struct PinChangedEventArgs
	{
	public:
		bool newValue;
	};

	class TREEHOPPER_API DigitalIn
	{
	public:
		DigitalIn() :pinChanged(this) { }
		Event<DigitalIn, PinChangedEventArgs> pinChanged;

		virtual void makeDigitalInput() = 0;
		virtual bool digitalValue()
		{
			return _digitalValue;
		}
	protected:
		bool _digitalValue;

		virtual void update(bool newValue)
		{
			if (newValue == _digitalValue) return;

			_digitalValue = newValue;
			PinChangedEventArgs args;
			args.newValue = newValue;
			pinChanged.invoke(args);
		}
	};
}