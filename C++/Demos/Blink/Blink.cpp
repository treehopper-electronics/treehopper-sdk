// Blink.cpp : Defines the entry point for the console application.

#include "ConnectionService.h"
#include "TreehopperUsb.h"
#include <string>
#include <vector>
#include <iostream>

using namespace Treehopper;

int main()
{
	auto service = ConnectionService::instance();
    TreehopperUsb& board = service.getFirstDevice();
    
	board.connect();

    board.pins[2].makeAnalogInput();
    
    for(int i=0;i<20;i++)
	{
		board.led(!board.led());
		//board.pins[1].toggleOutput();
        cout << board.pins[2].adcValue() << endl;
        this_thread::sleep_for(chrono::milliseconds(100));
	}
	board.disconnect();

    return 0;
}

