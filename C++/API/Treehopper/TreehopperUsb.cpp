#include "stdafx.h"
#include "TreehopperUsb.h"

TreehopperUsb::TreehopperUsb(unique_ptr<UsbConnection> connection)
{
	this->connection = move(connection);

	for (int i = 0; i < numberOfPins; i++)
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

	isConnected = true;

	pinListenerThread = thread(&TreehopperUsb::pinStateListener, this);
	pinListenerThread.detach();
	return true;
}

void TreehopperUsb::disconnect()
{
	isConnected = false;
	//pinListenerThread.join(); // block this thread until the listener exits
	connection->close();
}


wstring TreehopperUsb::getSerialNumber()
{
	return connection->getSerialNumber();
}


wstring TreehopperUsb::getName()
{
	return connection->getName();
}

void TreehopperUsb::setLed(bool value)
{
	uint8_t data[2];
	data[0] = DevCmdLedConfig;
	data[1] = value;
	connection->sendDataPeripheralChannel(data, 2);
}

void TreehopperUsb::sendPinConfigPacket(uint8_t* data, uint8_t len)
{
	connection->sendDataPinConfigChannel(data, len);
}

void TreehopperUsb::pinStateListener()
{
	uint8_t buffer[41];
	while (isConnected)
	{
		connection->receivePinReportPacket(buffer);

		for (int i = 0; i < numberOfPins; i++)
			pins[i].updateValue(buffer[i * 2 + 1], buffer[i * 2 + 2]);
	}
}
