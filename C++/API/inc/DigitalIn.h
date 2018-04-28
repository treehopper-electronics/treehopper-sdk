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
	
	/*!	\brief digital input pin abstract class.

	This abstract class provides digital input support used by Pin, and can also be extended by GPIO expanders and other peripherals that provide DigitalIn -like functionality.
	*/
	class TREEHOPPER_API DigitalIn
	{
	public:
		DigitalIn() :pinChanged(*this) { }

		/** Fires whenever the digital input changes. */
		Event<DigitalIn, PinChangedEventArgs> pinChanged;

		/** Make the pin a digital input.
		*/
		virtual void makeDigitalInput() = 0;

		/** Get the digital value
		*/
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