// Blink.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "ConnectionService.h"
#include "TreehopperUsb.h"
#include <string>
#include <vector>

using namespace std;

int main()
{
	ConnectionService service;
	TreehopperUsb& board = *service.boards.at(0);
	board.connect();

	for(int i=0;i<20;i++)
	{
		board.setLed(true);
		board.pins[1].setDigitalValue(true);
		Sleep(100);
		board.setLed(false);
		board.pins[1].setDigitalValue(false);
		Sleep(100);
	}
	board.disconnect();
    return 0;
}

