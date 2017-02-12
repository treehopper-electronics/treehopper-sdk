#include "stdafx.h"
#include "TreehopperUsb.h"
namespace Treehopper 
{
	TreehopperUsb::TreehopperUsb(unique_ptr<UsbConnection> connection)
	{
		this->connection = move(connection);
		i2c = new HardwareI2c(this);
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

	wstring TreehopperUsb::serialNumber()
	{
		return connection->serialNumber();
	}

	wstring TreehopperUsb::name()
	{
		return connection->name();
	}

	void TreehopperUsb::led(bool value)
	{
		uint8_t data[2];
		data[0] = (uint8_t)DeviceCommands::LedConfig;
		data[1] = value;
		connection->sendDataPeripheralChannel(data, 2);
	}

	void TreehopperUsb::sendPinConfigPacket(uint8_t* data, size_t len)
	{
		connection->sendDataPinConfigChannel(data, len);
	}

	void TreehopperUsb::sendPeripheralConfigPacket(uint8_t* data, size_t len)
	{
		connection->sendDataPeripheralChannel(data, len);
	}

	void TreehopperUsb::receivePeripheralConfigPacket(uint8_t * data, size_t numBytesToRead)
	{
		connection->receiveDataPeripheralChannel(data, numBytesToRead);
	}

	void TreehopperUsb::pinStateListener()
	{
		while (isConnected)
		{
			connection->receivePinReportPacket(buffer);

			for (int i = 0; i < numberOfPins; i++)
				pins[i].updateValue(buffer[i * 2 + 1], buffer[i * 2 + 2]);
		}
	}
}