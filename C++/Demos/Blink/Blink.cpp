// Blink.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "Treehopper/inc/ConnectionService.h"
#include "Treehopper/inc/TreehopperUsb.h"
#include <string>
#include <vector>
#include <iostream>

using namespace std;
using namespace Treehopper;

int main()
{
	ConnectionService service;
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
    
    while(true)
    {
        std:this_thread::sleep_for(chrono::seconds(5));
    }
    
    return 0;
}

