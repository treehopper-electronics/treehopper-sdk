#pragma once
#include "UsbConnection.h"
#include <memory>
#include <vector>
#include "Pin.h"
#include "HardwareI2c.h"
#include <thread>
using namespace std;
namespace Treehopper 
{
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
	public:
		TreehopperUsb(unique_ptr<UsbConnection> connection);
		~TreehopperUsb();
		TreehopperUsb(const TreehopperUsb& rhs) = delete;
		TreehopperUsb& operator= (const TreehopperUsb& rhs) = delete;
		bool isConnected;
		bool connect();
		void disconnect();
		wstring getSerialNumber();
		wstring getName();
		void setLed(bool value);
		friend wostream& operator<<(wostream& wos, TreehopperUsb& board)
		{
			wos << board.getName() << " (" << board.getSerialNumber() << ")";
			return wos;
		}
		vector<Pin> pins;
		int numberOfPins = 20;

		I2c* i2c;

	private:
		unique_ptr<UsbConnection> connection;
		thread pinListenerThread;
		void pinStateListener();
	protected:
		void sendPinConfigPacket(uint8_t* data, size_t len);
		void sendPeripheralConfigPacket(uint8_t* data, size_t len);
		void receivePeripheralConfigPacket(uint8_t* data, size_t numBytesToRead);
	};
}