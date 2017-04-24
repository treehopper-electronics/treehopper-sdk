#include "Input/RotaryEncoder.h"

namespace Treehopper { namespace Libraries { namespace Input {

	RotaryEncoder::RotaryEncoder(DigitalIn& channelA, DigitalIn& channelB, int stepsPerTick) : positionChanged(*this), a(channelA), b(channelB), stepsPerTick(stepsPerTick), _position(0), oldPosition(0)
	{
		a.makeDigitalInput();
		b.makeDigitalInput();

		a.pinChanged += [&](DigitalIn& object, PinChangedEventArgs e) {
			if (b.digitalValue() && a.digitalValue())
			{
				_position++;
			}
			else if (b.digitalValue() && !a.digitalValue())
			{
				_position--;
			}
			else if (!b.digitalValue() && a.digitalValue())
			{
				_position--;
			}
			else if (!b.digitalValue() && !a.digitalValue())
			{
				_position++;
			}

			updatePosition();
		};

		b.pinChanged += [&](DigitalIn& object, PinChangedEventArgs e) {
			if (b.digitalValue() && a.digitalValue())
			{
				_position--;
			}
			else if (b.digitalValue() && !a.digitalValue())
			{
				_position++;
			}
			else if (!b.digitalValue() && a.digitalValue())
			{
				_position++;
			}
			else if (!b.digitalValue() && !a.digitalValue())
			{
				_position--;
			}

			updatePosition();
		};
	}

	void RotaryEncoder::updatePosition()
	{
		if (_position % stepsPerTick == 0)
		{
			if (position() == oldPosition) return;

			PositionChangedEventArgs args;
			args.newPosition = position();
			positionChanged.invoke(args);
			
			oldPosition = position();
		}
	}

	long RotaryEncoder::position()
	{
		return _position / stepsPerTick;
	}

	void RotaryEncoder::position(long value)
	{
		_position = value * stepsPerTick;
	}

	RotaryEncoder::~RotaryEncoder()
	{
	}

}}}