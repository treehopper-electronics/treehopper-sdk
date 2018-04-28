#include "stdafx.h"
#include <iostream>
#include "ConnectionService.h"
#include "Libraries/Displays/Dm632.h"
using namespace Treehopper;
using namespace Treehopper::Libraries::Displays;

int main()
{
	TreehopperUsb& board = ConnectionService::instance().boards[0];
	board.connect();

	board.spi.enabled(true);

	auto driver = Dm632(board.spi, &board.pins[5]);

	while (true)
	{
		for (int i = 0; i<16; i++)
		{
			driver.leds[i].brightness(0.25);
			driver.leds[i].state(true);
			this_thread::sleep_for(100ms);
		}

		for (int i = 0; i<16; i++)
		{
			driver.leds[i].brightness(0);
			driver.leds[i].state(true);
			this_thread::sleep_for(100ms);
		}
	}
	
	
	board.disconnect();
    return 0;
}

