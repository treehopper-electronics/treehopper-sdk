// I2cDeviceScanner.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "Treehopper/inc/ConnectionService.h"
#include "Treehopper/inc/I2cTransferException.h"
#include <iostream>
#include "Windows.h"
#include "Treehopper/inc/Settings.h"
using namespace std;
using namespace Treehopper;

int main()
{
	TreehopperUsb& board = ConnectionService::instance().boards[0];
	Settings::instance().throwExceptions = true;
	board.connect();
	board.i2c.enabled(true);
	for (int i = 1; i < 127; i++) {
		cout << "0x" << std::hex << i << ": ";
		for (int j = 0; j < 3; j++)	{
			try {
				byte_t sendData = 0;
				board.i2c.sendReceive((byte_t)i, &sendData, 1, NULL, 0);
				cout << "Device found!";
				break;
			} catch (...) {
				cout << "...";
			}

			Sleep(10);
		}
		cout << endl;
	}
	board.disconnect();
	cout << "Scan complete. Press any key to close." << endl;
	cin.get();
    return 0;
}

