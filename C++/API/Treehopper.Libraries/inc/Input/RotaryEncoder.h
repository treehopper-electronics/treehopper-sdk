#pragma once

#include "Treehopper.Libraries.h"
#include "Event.h"
#include "DigitalIn.h"

namespace Treehopper { namespace Libraries { namespace Input {

	using namespace std;
	using namespace Treehopper;

	class RotaryEncoder;

	struct PositionChangedEventArgs
	{
	public:
		long newPosition;
	};

	class LIBRARIES_API RotaryEncoder
	{
	public:
		RotaryEncoder(DigitalIn& channelA, DigitalIn& channelB, int stepsPerTick = 4);
		~RotaryEncoder();
		Event<RotaryEncoder, PositionChangedEventArgs> positionChanged;
		long position();
		void position(long value);
	protected:
		DigitalIn& a;
		DigitalIn& b;
		long _position;
		long oldPosition;
		int stepsPerTick;
		void updatePosition();
	};

} } }
