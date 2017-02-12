#include "stdafx.h"
#include <iostream>
#include "Treehopper/inc/ConnectionService.h"
#include "Treehopper.Libraries/inc/Sensors/Temperature/Lm75.h"
using namespace Treehopper;
using namespace Treehopper::Libraries::Sensors::Temperature;

int main()
{
	ConnectionService service;
	TreehopperUsb& board = *service.boards[0];
	board.connect();

	Lm75 temp(board.i2c, true, true, true);

	for (int i = 0; i<20; i++)
	{
		cout << "Temperature: " << temp.getFahrenheit() << endl;
		Sleep(1000);
	}
	board.disconnect();
    return 0;
}

