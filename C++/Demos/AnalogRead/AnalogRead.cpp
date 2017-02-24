// AnalogRead.cpp : Defines the entry point for the console application.
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
	TreehopperUsb& board = ConnectionService::instance().boards[0];
	board.connect();

	wcout << "Connected to " << board << endl;

	board.pins[5].mode(PinMode::AnalogInput);

	for (int i = 0; i<50; i++)
	{
		wcout << "Analog value: " << board.pins[5].analogVoltage() << endl;
		Sleep(100);
	}
	board.disconnect();
	return 0;
}
