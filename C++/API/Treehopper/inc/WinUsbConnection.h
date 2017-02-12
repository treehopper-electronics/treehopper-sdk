#pragma once
#include "Treehopper.h"
#include "UsbConnection.h"
#include <string>
#include <winusb.h>

using namespace std;

namespace Treehopper 
{
	typedef struct _DEVICE_DATA {

		BOOL                    HandlesOpen;
		WINUSB_INTERFACE_HANDLE WinusbHandle;
		HANDLE                  DeviceHandle;
		TCHAR                   DevicePath[MAX_PATH];

	} DEVICE_DATA, *PDEVICE_DATA;

	class TREEHOPPER_API  WinUsbConnection : public UsbConnection
	{
	public:
		WinUsbConnection(wstring devicePath);
		~WinUsbConnection();
		bool open();
		void close();
		void sendDataPinConfigChannel(uint8_t* data, size_t len);
		void sendDataPeripheralChannel(uint8_t* data, size_t len);
		wstring serialNumber();
		wstring name();
		wstring devicePath();
		bool receivePinReportPacket(uint8_t* data);
		bool receiveDataPeripheralChannel(uint8_t* data, size_t len);
	private:
		DEVICE_DATA deviceData;
	};
}