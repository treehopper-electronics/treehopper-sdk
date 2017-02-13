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
	ConnectionService service;
	TreehopperUsb& board = service.boards[0];
	board.connect();

	wcout << "Connected to " << board << endl;

	board.pins[5].setMode(PinMode::AnalogInput);

	for (int i = 0; i<50; i++)
	{
		wcout << "Analog value: " << board.pins[5].AnalogVoltage << endl;
		Sleep(100);
	}
	board.disconnect();
	return 0;
}
