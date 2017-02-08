#pragma once
#include "UsbConnection.h"
#include <memory>
#include <vector>
#include "Pin.h"
#include <thread>
using namespace std;

enum DeviceCommands
{
	DevCmdReserved,	// Not implemented
	DevCmdConfigureDevice,	// Sent upon device connect/disconnect
	DevCmdPwmConfig,
	DevCmdUartConfig,
	DevCmdI2cConfig,
	DevCmdSpiConfig,
	DevCmdI2cTransaction,
	DevCmdSpiTransaction,
	DevCmdUartTransaction,
	DevCmdSoftPwmConfig,
	DevCmdFirmwareUpdateSerial,
	DevCmdFirmwareUpdateName,
	DevCmdReboot,
	DevCmdEnterBootloader,
	DevCmdLedConfig,
	DevCmdParallelConfig,
	DevCmdParallelTransaction
};

class TREEHOPPER_API TreehopperUsb
{
	friend class Pin;
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
private:
	unique_ptr<UsbConnection> connection;
	thread pinListenerThread;
	void pinStateListener();
protected:
	void sendPinConfigPacket(uint8_t* data, uint8_t len);
};