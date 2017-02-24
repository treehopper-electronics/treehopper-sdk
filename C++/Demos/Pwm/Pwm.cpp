// Blink.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "Treehopper/inc/ConnectionService.h"
#include "Treehopper/inc/TreehopperUsb.h"
#include "Treehopper/inc/Pwm.h"
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
			Sleep(10);
		}
		for (int i = 0; i <= 20; i++)
		{
			board.pwm1.dutyCycle(1 - 0.05 * i);
			Sleep(10);
		}
	
	
	board.disconnect();
    return 0;
}

