#include "Pins.h"
#include "AnalogIn.h"

Pin1::Pin1(TreehopperBoard* board) : Pin(1, board), AnalogIn(this)
{
	
}

void Pin1::UpdateValue(uint8_t high, uint8_t low)
{
	switch (State)
	{
	case PinStateAnalogInput:
		AnalogIn.UpdateAnalogValue(high, low);
		break;
	case PinStateDigitalInput:
		Pin::UpdateValue(high, low);
	}
}

Pin2::Pin2(TreehopperBoard* board) : Pin(2, board)
{

}

Pin3::Pin3(TreehopperBoard* board) : Pin(3, board)
{

}

Pin4::Pin4(TreehopperBoard* board) : Pin(4, board), AnalogIn(this)
{
	
}

void Pin4::UpdateValue(uint8_t high, uint8_t low)
{
	switch (State)
	{
	case PinStateAnalogInput:
		AnalogIn.UpdateAnalogValue(high, low);
		break;
	case PinStateDigitalInput:
		Pin::UpdateValue(high, low);
	}
}

Pin5::Pin5(TreehopperBoard* board) : Pin(5, board), AnalogIn(this)
{

}

void Pin5::UpdateValue(uint8_t high, uint8_t low)
{
	switch (State)
	{
	case PinStateAnalogInput:
		AnalogIn.UpdateAnalogValue(high, low);
		break;
	case PinStateDigitalInput:
		Pin::UpdateValue(high, low);
	}
}

Pin6::Pin6(TreehopperBoard* board) : Pin(6, board), AnalogIn(this)
{

}

void Pin6::UpdateValue(uint8_t high, uint8_t low)
{
	switch (State)
	{
	case PinStateAnalogInput:
		AnalogIn.UpdateAnalogValue(high, low);
		break;
	case PinStateDigitalInput:
		Pin::UpdateValue(high, low);
	}
}

Pin7::Pin7(TreehopperBoard* board) : Pin(7, board), AnalogIn(this)
{

}

void Pin7::UpdateValue(uint8_t high, uint8_t low)
{
	switch (State)
	{
	case PinStateAnalogInput:
		AnalogIn.UpdateAnalogValue(high, low);
		break;
	case PinStateDigitalInput:
		Pin::UpdateValue(high, low);
	}
}

Pin8::Pin8(TreehopperBoard* board) : Pin(8, board)
{

}

Pin9::Pin9(TreehopperBoard* board) : Pin(9, board), AnalogIn(this)
{

}

void Pin9::UpdateValue(uint8_t high, uint8_t low)
{
	switch (State)
	{
	case PinStateAnalogInput:
		AnalogIn.UpdateAnalogValue(high, low);
		break;
	case PinStateDigitalInput:
		Pin::UpdateValue(high, low);
	}
}

Pin10::Pin10(TreehopperBoard* board) : Pin(10, board), AnalogIn(this)
{

}

void Pin10::UpdateValue(uint8_t high, uint8_t low)
{
	switch (State)
	{
	case PinStateAnalogInput:
		AnalogIn.UpdateAnalogValue(high, low);
		break;
	case PinStateDigitalInput:
		Pin::UpdateValue(high, low);
	}
}

Pin11::Pin11(TreehopperBoard* board) : Pin(11, board)
{

}

Pin12::Pin12(TreehopperBoard* board) : Pin(12, board), AnalogIn(this)
{

}

void Pin12::UpdateValue(uint8_t high, uint8_t low)
{
	switch (State)
	{
	case PinStateAnalogInput:
		AnalogIn.UpdateAnalogValue(high, low);
		break;
	case PinStateDigitalInput:
		Pin::UpdateValue(high, low);
	}
}

Pin13::Pin13(TreehopperBoard* board) : Pin(13, board), AnalogIn(this)
{

}

void Pin13::UpdateValue(uint8_t high, uint8_t low)
{
	switch (State)
	{
	case PinStateAnalogInput:
		AnalogIn.UpdateAnalogValue(high, low);
		break;
	case PinStateDigitalInput:
		Pin::UpdateValue(high, low);
	}
}

Pin14::Pin14(TreehopperBoard* board) : Pin(14, board)
{

}


