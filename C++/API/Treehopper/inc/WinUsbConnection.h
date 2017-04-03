#pragma once
#include "Treehopper.h"
#include "UsbConnection.h"
#include <string>
#include <winusb.h>

using namespace std;

namespace Treehopper 
{
	class TREEHOPPER_API  WinUsbConnection : public UsbConnection
	{
	public:
		WinUsbConnection(wstring devicePath);
		~WinUsbConnection();
		bool open();
		void close();
		void sendDataPinConfigChannel(byte_t* data, size_t len);
		void sendDataPeripheralChannel(byte_t* data, size_t len);
		wstring serialNumber();
		wstring name();
		wstring devicePath();
		bool receivePinReportPacket(byte_t* data);
		bool receiveDataPeripheralChannel(byte_t* data, size_t len);
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