// I2cDeviceScanner.cpp : Defines the entry point for the console application.
//

#include "ConnectionService.h"
#include "I2cTransferException.h"
#include <iostream>
#include "Settings.h"
#include <vector>
#include <stdint.h>

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
				board.i2c.sendReceive((uint8_t)i, std::vector<uint8_t>(1), 0);
				cout << "Device found!";
				break;
			} catch (...) {
				cout << "...";
			}

			this_thread::sleep_for(chrono::milliseconds(10));
		}
		cout << endl;
	}
	board.disconnect();
	cout << "Scan complete. Press any key to close." << endl;
	cin.get();
    return 0;
}

