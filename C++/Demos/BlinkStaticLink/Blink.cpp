// Blink.cpp
// This example blinks an LED hooked up to the Treehopper's pin 1.

#define TREEHOPPER_STATIC_LINK // must be put above the Treehopper.h include line

#include "stdafx.h"
#include <Treehopper.h>
#include <chrono>
#include <thread>

using namespace std;

int _tmain(int argc, _TCHAR* argv[])
{
	chrono::milliseconds duration(1000);
	TreehopperBoard &board = *(new TreehopperBoard());

	board.Open();
	while (true)
	{
		board.Pin1.Value = !board.Pin1.Value;
		std::this_thread::sleep_for(duration);
	}
	
	board.Close();
	return 0;
}

