#pragma once
#include "UsbConnection.h"
#include <memory>
#include <vector>
#include "Pin.h"
#include "HardwarePwmManager.h"
#include "HardwarePwm.h"
#include "HardwareI2c.h"
#include "HardwareSpi.h"
#include <thread>

using namespace std;

namespace Treehopper 
{
	class Pwm;
	class I2c;

	class TREEHOPPER_API TreehopperUsb
	{
		friend Pin;
		friend HardwareI2c;
		friend UsbConnection;
		friend HardwarePwmManager;
		friend HardwareSpi;
	public:
		TreehopperUsb(UsbConnection& connection);
		~TreehopperUsb();
		TreehopperUsb& operator=(const TreehopperUsb& rhs)
		{
			connection = rhs.connection;
			return *this;
		}
		TreehopperUsb(const TreehopperUsb& rhs) : TreehopperUsb(rhs.connection)
		{

		}

		bool isConnected;
		bool connect();
		void disconnect();
		wstring serialNumber();
		wstring name();
		void led(bool value);
		bool led();
        wstring toString()
        {
            wstring output = name() + L" (" + serialNumber() + L")";
			return output;
		}
		vector<Pin> pins;
		const int numberOfPins = 20;

		HardwareI2c i2c;
		HardwareSpi spi;
		HardwarePwm pwm1;
		HardwarePwm pwm2;
		HardwarePwm pwm3;
		HardwarePwmManager pwmManager;
	private:
		enum class DeviceCommands
		{
			Reserved = 0,   // Not implemented
			ConfigureDevice,    // Sent upon board connect/disconnect
			PwmConfig,
			UartConfig,
			I2cConfig,
			SpiConfig,
			I2cTransaction,
			SpiTransaction,
			UartTransaction,
			SoftPwmConfig,
			FirmwareUpdateSerial,
			FirmwareUpdateName,
			Reboot,
			EnterBootloader,
			LedConfig,
			ParallelConfig,
			ParallelTransaction
		};
		UsbConnection& connection;
		thread pinListenerThread;
		void pinStateListener();
		uint8_t buffer[41];
		bool _led;
	protected:
		void sendPinConfigPacket(uint8_t* data, size_t len);
		void sendPeripheralConfigPacket(uint8_t* data, size_t len);
		void receivePeripheralConfigPacket(uint8_t* data, size_t numBytesToRead);
	};
}