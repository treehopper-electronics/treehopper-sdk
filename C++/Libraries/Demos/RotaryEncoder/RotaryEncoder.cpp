#include "stdafx.h"
#include <iostream>
#include "Treehopper/inc/ConnectionService.h"
#include "Treehopper.Libraries/inc/Input/RotaryEncoder.h"

using namespace Treehopper;
using namespace Treehopper::Libraries::Input;

int main()
{
	TreehopperUsb& board = ConnectionService::instance().boards[0];
	board.connect();

	RotaryEncoder encoder(board.pins[0], board.pins[1], 4);
	encoder.positionChanged += [](RotaryEncoder& sender, PositionChangedEventArgs e)
	{
		cout << "New position: " << e.newPosition << endl;
	};

	cout << "Sleeping main thread for 50 seconds";
	this_thread::sleep_for(50s);
	cout << "Disconnecting board and closing";
	board.disconnect();
    return 0;
}
