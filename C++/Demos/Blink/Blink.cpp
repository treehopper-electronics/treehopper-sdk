// Blink.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "Treehopper/inc/ConnectionService.h"
#include "Treehopper/inc/TreehopperUsb.h"
#include <string>
#include <vector>

using namespace std;
using namespace Treehopper;

int main()
{
	ConnectionService service;
	TreehopperUsb& board = service.boards[0];
	board.connect();

	for(int i=0;i<20;i++)
	{
		board.led(!board.led());
		board.pins[1].toggleOutput();
		Sleep(100);
	}
	board.disconnect();
    return 0;
}

