#pragma once

#include <stdint.h>
#include <libusb.h>
#include <string>
#include <thread>
#include "Pins.h"


#ifdef TREEHOPPER_STATIC_LINK
	#define TREEHOPPER_API
#else
	#ifdef TREEHOPPER_EXPORTS
		#define TREEHOPPER_API __declspec(dllexport)
	#else
		#define TREEHOPPER_API __declspec(dllimport)
	#endif
#endif

using namespace std;

enum PinConfigCommand
{
	CmdMakeDigitalInput = 0x00,
	CmdMakeDigitalOutput,
	CmdMakeAnalogInput,
	CmdMakeAnalogOutput,
	CmdMakePWMPin,
	CmdSetDigitalValue,
	CmdGetDigitalValue,
	CmdGetAnalogValue,
	CmdSetAnalogValue,
	CmdSetPWMValue
};

enum DeviceCommands
{
	DevCmdReserved = 0,
	DevCmdGetDeviceInfo,
	DevCmdPinConfig,
	DevCmdComparatorConfig,
	DevCmdDACConfig,
	DevCmdUARTConfig,
	DevCmdI2CConfig,
	DevCmdSPIConfig,
	DevCmdI2CTransaction,
	DevCmdSPITransaction,
	DevCmdSoftPwmConfig,
	DevCmdSoftPwmUpdateDc,
	DevCmdFirmwareUpdateSerial,
	DevCmdFirmwareUpdateName,
	DevCmdReboot,
	DevCmdEnterBootloader
};

class TREEHOPPER_API TreehopperBoard
{
public:
	//thread pinStateThread;
	static const int vid = 0x04d8;
	static const int pid = 0xF426;
	string Name;
	string SerialNumber;
	void pinStateListener();

	//EXPORT TreehopperBoard(libusb_device* device);
	TreehopperBoard::TreehopperBoard(libusb_device* device);
	TreehopperBoard(string serialNumber = "");
	void UpdateStrings();
	void Open();
	void Close();
	int SendPinConfigCommand(uint8_t pinNumber, uint8_t* data, uint8_t len);
	bool IsOpen;
	void callback(struct libusb_transfer *transfer);
	Pin1 Pin1;
	Pin2 Pin2;
	Pin3 Pin3;
	Pin4 Pin4;
	Pin5 Pin5;
	Pin6 Pin6;
	Pin7 Pin7;
	Pin8 Pin8;
	Pin9 Pin9;
	Pin10 Pin10;
	Pin11 Pin11;
	Pin12 Pin12;
	Pin13 Pin13;
	Pin14 Pin14;

private:

	uint8_t PinConfigBuffer[64];
	uint8_t PinStateBuffer[64];
	uint8_t AuxConfigBuffer[64];
	uint8_t AuxResponseBuffer[64];

	libusb_device_handle* Handle;
	libusb_device* Device;

	unsigned char PinConfigEndpoint = 0x01;
	unsigned char PinStateEndpoint = 0x81;
	unsigned char CommsConfigEndpoint = 0x02;
	unsigned char CommsReceiveEndpoint = 0x82;
	libusb_transfer* transfer;

};