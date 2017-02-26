#pragma once
#include "Spi.h"
#include "DigitalOut.h"
namespace Treehopper {
	class TREEHOPPER_API SpiChipSelectPin : public DigitalOut
	{
	public:
		int pinNumber;
		Spi* spiModule;
	};
}