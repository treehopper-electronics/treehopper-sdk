#include "TreehopperUsb.h"
#include "HardwarePwm.h"
#include "HardwareI2c.h"

namespace Treehopper 
{
	TreehopperUsb::TreehopperUsb(UsbConnection& connection) :
		connection(connection),
		pwmManager(*this),
		i2c(*this),
		spi(*this),
		pwm1(*this, 7),
		pwm2(*this, 8),
		pwm3(*this, 9)
	{
		for (int i = 0; i < numberOfPins; i++)
			pins.emplace_back(this, i);
	}

	TreehopperUsb::~TreehopperUsb()
	{
		if (_isDestroyed)
			return;

		cout << "Destructor" << endl;
        disconnect();
        //delete &connection;
		_isDestroyed = true;
	}

	bool TreehopperUsb::connect()
	{
		if (!connection.open())
		{
			return false;
		}

		isConnected = true;
		pinListenerThread = thread(&TreehopperUsb::pinStateListener, this);

		reinitialize();
		return true;
	}

	void TreehopperUsb::disconnect()
	{
		if (!isConnected) return;

		reinitialize(); // leave the board where we found it

		isConnected = false;
        if(pinListenerThread.joinable())
            pinListenerThread.join(); // block this thread until the listener exits
		connection.close();
	}

	void TreehopperUsb::reinitialize()
	{
		uint8_t data[2];
		data[0] = (uint8_t)DeviceCommands::ConfigureDevice;
		sendPeripheralConfigPacket(data, 2);
	}

	wstring TreehopperUsb::serialNumber()
	{
		return connection.serialNumber();
	}

	wstring TreehopperUsb::name()
	{
		return connection.name();
	}

	void TreehopperUsb::led(bool value)
	{
        if(!isConnected) return;
        
        _led = value;
		uint8_t data[2];
		data[0] = (uint8_t)DeviceCommands::LedConfig;
		data[1] = _led;
		connection.sendDataPeripheralChannel(data, 2);
	}

	bool TreehopperUsb::led()
	{
		return _led;
	}

	void TreehopperUsb::sendPinConfigPacket(uint8_t* data, size_t len)
	{
        if(!isConnected) return;
		connection.sendDataPinConfigChannel(data, len);
	}

	void TreehopperUsb::sendPeripheralConfigPacket(uint8_t* data, size_t len)
	{
        if(!isConnected) return;
		connection.sendDataPeripheralChannel(data, len);
	}

	void TreehopperUsb::receivePeripheralConfigPacket(uint8_t * data, size_t numBytesToRead)
	{
        if(!isConnected) return;
		connection.receiveDataPeripheralChannel(data, numBytesToRead);
	}

	void TreehopperUsb::pinStateListener()
	{
		while (isConnected)
		{
                if(!connection.receivePinReportPacket(buffer)) continue;
                
                for (int i = 0; i < numberOfPins; i++)
                    pins[i].updateValue(buffer[i * 2 + 1], buffer[i * 2 + 2]);
        }
	}
}