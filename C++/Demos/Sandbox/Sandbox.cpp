#include <iostream>
#include "ConnectionService.h"
#include "Sensors/Pressure/Bmp280.h"
#include <chrono>
#include <thread>

using namespace Treehopper;
using namespace Treehopper::Libraries::Sensors::Pressure;

int main()
{
	TreehopperUsb& board = ConnectionService::instance().boards[0];
	board.connect();

	auto sensor = Bmp280(board.i2c, false);
	while (true) {
		cout << sensor.altitude() << "\r\n";
		std::this_thread::sleep_for(std::chrono::milliseconds(500));
	}
	
	board.disconnect();
	return 0;
}
