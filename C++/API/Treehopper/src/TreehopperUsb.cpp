#include "stdafx.h"
#include "TreehopperUsb.h"
#include "HardwarePwm.h"
#include "hardwareI2c.h"

namespace Treehopper 
{
	TreehopperUsb::TreehopperUsb(UsbConnection* connection) :
		connection(connection),
		pwmManager(this),
		i2c(*this),
		spi(*this),
		pwm1(this, 7),
		pwm2(this, 8),
		pwm3(this, 9)
	{
		for (int i = 0; i < numberOfPins; i++)
			pins.emplace_back(this, i);
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
		_led = value;
		uint8_t data[2];
		data[0] = (uint8_t)DeviceCommands::LedConfig;
		data[1] = _led;
		connection->sendDataPeripheralChannel(data, 2);
	}

	bool TreehopperUsb::led()
	{
		return _led;
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