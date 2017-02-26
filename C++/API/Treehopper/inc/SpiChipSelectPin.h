#pragma once
#include "Spi.h"
namespace Treehopper {
	class SpiChipSelectPin
	{
	public:
		int pinNumber;
		Spi* spiModule;
	};
}