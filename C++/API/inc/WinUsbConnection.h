#pragma once
#include "Treehopper.h"
#include "UsbConnection.h"
#include <string>
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <SDKDDKVer.h>
#include <winusb.h>

using namespace std;

namespace Treehopper 
{
	class TREEHOPPER_API  WinUsbConnection : public UsbConnection
	{
	public:
		WinUsbConnection(wstring devicePath, wstring friendlyName, wstring serialNumber, int rev);
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
		static void debugPrintLastError();
	private:
		typedef struct _DEVICE_DATA {

			BOOL                    HandlesOpen;
			WINUSB_INTERFACE_HANDLE WinusbHandle;
			HANDLE                  DeviceHandle;
			TCHAR                   DevicePath[MAX_PATH];

		} DEVICE_DATA, *PDEVICE_DATA;
		DEVICE_DATA deviceData;
	};
}