#include "stdafx.h"
#include "TreehopperUsb.h"

TreehopperUsb::TreehopperUsb(unique_ptr<UsbConnection> connection)
{
	this->connection = move(connection);

	for (int i = 0; i<numberOfPins; i++)
	{
		pins.emplace_back(this, i);
	}
}

TreehopperUsb::~TreehopperUsb()
{

}

bool TreehopperUsb::connect()
{
	if (!connection->open())
	{
		return false;
	}

	return true;
}

void TreehopperUsb::disconnect()
{
	connection->close();
}


wstring TreehopperUsb::getSerialNumber()
{
	return wstring();
}


wstring TreehopperUsb::getName()
{
	return wstring();
}

void TreehopperUsb::setLed(bool value)
{
	uint8_t data[2];
	data[0] = 14;
	data[1] = value;
	connection->sendDataPeripheralChannel(data, 2);
}

void TreehopperUsb::sendPinConfigPacket(uint8_t* data, uint8_t len)
{
	connection->sendDataPinConfigChannel(data, len);
}
