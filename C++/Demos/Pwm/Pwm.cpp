// Blink.cpp : Defines the entry point for the console application.
//

#include "ConnectionService.h"
#include "TreehopperUsb.h"
#include "Pwm.h"
#include <string>
#include <vector>

using namespace std;
using namespace Treehopper;

int main()
{
	TreehopperUsb& board = ConnectionService::instance().boards[0];
	board.connect();
	board.pwm1.enabled(true);
		for (int i = 0; i <= 20; i++)
		{
			board.pwm1.dutyCycle(0.05 * i);
			this_thread::sleep_for(chrono::milliseconds(10));
		}
		for (int i = 0; i <= 20; i++)
		{
			board.pwm1.dutyCycle(1 - 0.05 * i);
			this_thread::sleep_for(chrono::milliseconds(10));
		}
	
	
	board.disconnect();
    return 0;
}

