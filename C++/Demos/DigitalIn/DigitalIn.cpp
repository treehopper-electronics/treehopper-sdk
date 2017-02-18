// Blink.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "Treehopper/inc/ConnectionService.h"
#include "Treehopper/inc/TreehopperUsb.h"
#include <string>
#include <vector>
#include <thread>
#include <chrono>
#include <iostream>

using namespace std;
using namespace Treehopper;

void pin1inputHandler(DigitalIn* sender, PinChangedEventArgs e) {
	cout << "Pin 0 event: " << e.newValue << endl;
};

int main()
{
	ConnectionService service;
	TreehopperUsb& board = service.boards[0];
	board.connect();

	board.pins[0].mode(PinMode::DigitalInput);
	board.pins[1].mode(PinMode::DigitalInput);

	// you can hook into the pinChanged event by passing in a named function
	board.pins[0].pinChanged += pin1inputHandler;

	// ... or by defining an in-place lambda expression
	board.pins[1].pinChanged += [](DigitalIn* sender, PinChangedEventArgs e) {
		cout << "Pin 1 event: " << e.newValue << endl;
	};

	this_thread::sleep_for(5s);
	cout << "removing pin 0 event" << endl;

	// you can remove an event handler, too:
	board.pins[0].pinChanged -= pin1inputHandler;

	// make sure that your main program doesn't exit
	this_thread::sleep_for(100s);
	cout << "disconnecting and exiting program" << endl;
	board.disconnect();
    return 0;
}

