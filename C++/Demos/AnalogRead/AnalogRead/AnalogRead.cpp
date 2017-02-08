// AnalogRead.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "ConnectionService.h"
#include "TreehopperUsb.h"
#include <string>
#include <vector>
#include <iostream>

using namespace std;

int main()
{
	ConnectionService service;
	TreehopperUsb& board = *service.boards[0];
	board.connect();

	wcout << "Connected to " << board << endl;

	board.pins[5].setMode(PinModeAnalogInput);

	for (int i = 0; i<50; i++)
	{
		wcout << "Analog value: " << board.pins[5].AnalogVoltage << endl;
		Sleep(100);
	}
	board.disconnect();
	return 0;
}
