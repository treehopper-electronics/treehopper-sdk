#include "stdafx.h"
#include <iostream>
#include "Treehopper/inc/ConnectionService.h"
#include "Treehopper.Libraries/inc/Displays/Dm632.h"
using namespace Treehopper;
using namespace Treehopper::Libraries::Displays;

uint8_t dataToSend[32];

int main()
{
	TreehopperUsb& board = ConnectionService::instance().boards[0];
	board.connect();

	board.spi.enabled(true);

	auto driver = Dm632(board.spi, &board.pins[5]);

	dataToSend[0] = 255;
	dataToSend[1] = 255;
	dataToSend[30] = 255;
	dataToSend[31] = 255;

	for(int i=0;i<16;i++)
	{
		driver.leds[i].brightness(0.5);
		driver.leds[i].state(true);
		this_thread::sleep_for(100ms);
	}

	for (int i = 0; i<16; i++)
	{
		driver.leds[i].brightness(0);
		driver.leds[i].state(true);
		this_thread::sleep_for(100ms);
	}
	
	board.disconnect();
    return 0;
}

