#pragma once
#include "UsbConnection.h"
#include <memory>
#include <vector>
#include "Pin.h"
#include "HardwarePwmManager.h"
#include "HardwarePwm.h"
#include "hardwareI2c.h"
#include <thread>

using namespace std;

namespace Treehopper 
{
	class Pwm;
	class I2c;

	enum class DeviceCommands
	{
		Reserved = 0,   // Not implemented
		ConfigureDevice,    // Sent upon device connect/disconnect
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

	class TREEHOPPER_API TreehopperUsb
	{
		friend class Pin;
		friend class HardwareI2c;
		friend class UsbConnection;
		friend class HardwarePwmManager;
	public:
		TreehopperUsb(UsbConnection* connection);
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
		friend wostream& operator<<(wostream& wos, TreehopperUsb& board)
		{
			wos << board.name() << " (" << board.serialNumber() << ")";
			return wos;
		}
		vector<Pin> pins;
		const int numberOfPins = 20;

		HardwareI2c i2c;
		HardwarePwm pwm1;
		HardwarePwm pwm2;
		HardwarePwm pwm3;
		HardwarePwmManager pwmManager;
	private:
		UsbConnection* connection;
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