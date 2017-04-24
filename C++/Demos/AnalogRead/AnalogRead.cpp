// AnalogRead.cpp : Defines the entry point for the console application.
//

#include "ConnectionService.h"
#include "TreehopperUsb.h"
#include <string>
#include <vector>
#include <iostream>

using namespace std;
using namespace Treehopper;

int main()
{
	TreehopperUsb& board = ConnectionService::instance().boards[0];
	board.connect();

	wcout << "Connected to " << board.toString() << endl;

	board.pins[5].mode(PinMode::AnalogInput);

	for (int i = 0; i<50; i++)
	{
		wcout << "Analog value: " << board.pins[5].analogVoltage() << endl;
		this_thread::sleep_for(chrono::milliseconds(100));
	}
	board.disconnect();
	return 0;
}
