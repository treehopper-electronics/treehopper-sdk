#include <iostream>
#include "ConnectionService.h"
#include "Input/RotaryEncoder.h"
#include "Sensors/Inertial/Adxl345.h"

using namespace Treehopper;
using namespace Treehopper::Libraries::Sensors::Inertial;
using namespace std::chrono;

int main()
{
	TreehopperUsb& board = ConnectionService::instance().getFirstDevice();
	board.connect();

	Adxl345 imu(board.i2c);

	cout << "Sleeping main thread for 50 seconds";
	this_thread::sleep_for(seconds(50));
	cout << "Disconnecting board and closing";
	board.disconnect();
    return 0;
}
