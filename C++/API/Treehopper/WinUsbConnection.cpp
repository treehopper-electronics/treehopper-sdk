#include "stdafx.h"
#include "WinUsbConnection.h"
#include "Utilities.h"

using namespace std;

void DebugPrintLastError();

WinUsbConnection::WinUsbConnection(wstring devPath)
{
	devicePath = wstring(devPath);
	OutputDebugString(L"Adding board: ");
	OutputDebugString(devicePath.c_str());
	OutputDebugString(L"\n");
}


WinUsbConnection::~WinUsbConnection()
{

}

bool WinUsbConnection::open()
{
	deviceData.DeviceHandle = CreateFile(devicePath.c_str(),
		GENERIC_WRITE | GENERIC_READ,
		FILE_SHARE_WRITE | FILE_SHARE_READ,
		NULL,
		OPEN_EXISTING,
		FILE_ATTRIBUTE_NORMAL | FILE_FLAG_OVERLAPPED,
		NULL);

	if (deviceData.DeviceHandle == INVALID_HANDLE_VALUE) 
	{
		DebugPrintLastError();
		return false;
	}

	if (WinUsb_Initialize(deviceData.DeviceHandle, &deviceData.WinusbHandle) == FALSE)
	{
		DebugPrintLastError();
		return false;
	}

	deviceData.HandlesOpen = true;

	// Fill the "name" property from the descriptor
	wchar_t buffer[64];
	ULONG transfered;
	WinUsb_GetDescriptor(deviceData.WinusbHandle, 0x03, 0x02, 0x0409, (PUCHAR)buffer, 64, &transfered); 	// The 0x02-index 0x03 (string) descriptor stores the name
	name.assign(&buffer[1], transfered / 2 - 1);
	OutputDebugString(L"Device Opened");

	return true;
}

void WinUsbConnection::close()
{
	if (!deviceData.HandlesOpen) 
		return;

	WinUsb_Free(deviceData.WinusbHandle);
	CloseHandle(deviceData.DeviceHandle);
	deviceData.HandlesOpen = false;
	OutputDebugString(L"Device Closed");
}

void WinUsbConnection::sendDataPinConfigChannel(uint8_t* data, int len)
{
	ULONG sent = 0;
	WinUsb_WritePipe(deviceData.WinusbHandle, pinConfigEndpoint, data, len, &sent, 0);
	if (sent != len)
		throw -1;
}

void WinUsbConnection::sendDataPeripheralChannel(uint8_t* data, int len)
{
	ULONG sent = 0;
	WinUsb_WritePipe(deviceData.WinusbHandle, peripheralConfigEndpoint, data, len, &sent, 0);
	if (sent != len)
		throw - 1;
}

wstring WinUsbConnection::getSerialNumber()
{
	int offset = 26;
	wstring result = devicePath.substr(offset, devicePath.length() - offset);
	offset = result.find('#', 0);
	return result.substr(0, offset);
}

wstring WinUsbConnection::getName()
{
	return name;
}

wstring WinUsbConnection::getDevicePath()
{
	return devicePath;
}

bool WinUsbConnection::receivePinReportPacket(uint8_t* data)
{
	ULONG transferred;
	WinUsb_ReadPipe(deviceData.WinusbHandle, pinReportEndpoint, data, 64, &transferred, 0);
	if (transferred > 0) return true;

	return false;
}


void DebugPrintLastError()
{
	wchar_t buf[256];
	FormatMessageW(FORMAT_MESSAGE_FROM_SYSTEM, NULL, GetLastError(),
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), buf, 256, NULL);
	OutputDebugString(buf);
	OutputDebugString(L"\n");
}