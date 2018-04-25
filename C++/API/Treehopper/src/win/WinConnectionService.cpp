#include "ConnectionService.h"

#define WIN32_LEAN_AND_MEAN  
#include <windows.h>
#include <SDKDDKVer.h>

#include <SetupAPI.h>
#include <string>
#include "WinUsbConnection.h"
#include <memory>

#include <wchar.h>

#define DEVPROPKEY(name, l, w1, w2, b1, b2, b3, b4, b5, b6, b7, b8, pid) const DEVPROPKEY name = { { l, w1, w2, { b1, b2,  b3,  b4,  b5,  b6,  b7,  b8 } }, pid }

using namespace std;

namespace Treehopper 
{
	DEFINE_GUID(GUID_DEVINTERFACE_testWinUsb,
		0xa5dcbf10, 0x6530, 0x11d2, 0x90, 0x1f, 0x00, 0xc0, 0x4f, 0xb9, 0x51, 0xed);

	ConnectionService::ConnectionService()
	{
		scan();
	}


	ConnectionService::~ConnectionService()
	{
	}

	TreehopperUsb& ConnectionService::getFirstDevice()
	{
		while(boards.size() == 0)
		{
			this_thread::sleep_for(chrono::milliseconds(100));
		}
		return boards[0];
	}


	void ConnectionService::scan()
	{
		DEVPROPKEY(FriendlyName, 0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 14);    // DEVPROP_TYPE_STRING
		DEVPROPKEY(HardwareIds,  0xa45c254e, 0xdf1c, 0x4efd, 0x80, 0x20, 0x67, 0xd1, 0x46, 0xa8, 0x50, 0xe0, 3);    // DEVPROP_TYPE_STRING

		BOOL bResult = FALSE;
		HDEVINFO devInfo;
		SP_DEVINFO_DATA devInfoData;

		SP_DEVICE_INTERFACE_DATA devInterfaceData;
		PSP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData = NULL;
		ULONG length;
		ULONG requiredLength = 0;
		HRESULT hr;

		DEVPROPTYPE ulPropertyType;

		uint8_t propBuffer[255];

		// Enumerate all devices exposing the interface
		devInfo = SetupDiGetClassDevs(&GUID_DEVINTERFACE_testWinUsb,
			NULL,
			NULL,
			DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);

		if (devInfo == INVALID_HANDLE_VALUE) {

			hr = HRESULT_FROM_WIN32(GetLastError());
			return;
		}

		devInfoData.cbSize = sizeof(SP_DEVINFO_DATA);
		devInterfaceData.cbSize = sizeof(SP_DEVICE_INTERFACE_DATA);


		// Allocate temporary space for SetupDi structure
		deviceInterfaceDetailData = (PSP_DEVICE_INTERFACE_DETAIL_DATA)
			LocalAlloc(LMEM_FIXED, requiredLength);

		if (NULL == deviceInterfaceDetailData)
		{
			hr = E_OUTOFMEMORY;
			SetupDiDestroyDeviceInfoList(devInfo);
			return;
		}

		deviceInterfaceDetailData->cbSize = sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA);

		int deviceIndex = 0;
		while (SetupDiEnumDeviceInterfaces(devInfo, NULL, &GUID_DEVINTERFACE_testWinUsb, deviceIndex++, &devInterfaceData))
		{
			//
			// Get the size of the path string
			// We expect to get a failure with insufficient buffer
			//
			bResult = SetupDiGetDeviceInterfaceDetail(devInfo,
				&devInterfaceData,
				deviceInterfaceDetailData,
				255,
				&requiredLength,
				&devInfoData);

			if (FALSE == bResult && ERROR_INSUFFICIENT_BUFFER != GetLastError()) {
				WinUsbConnection::debugPrintLastError();
				SetupDiDestroyDeviceInfoList(devInfo);
				return;
			}

			length = requiredLength;

			wstring devicePath = wstring(deviceInterfaceDetailData->DevicePath);
			int vidIdx = devicePath.find(L"vid_") + 4;
			int pidIdx = devicePath.find(L"pid_") + 4;

			wstring vid = devicePath.substr(vidIdx, 4);
			wstring pid = devicePath.substr(pidIdx, 4);

			if (vid.compare(L"10c4") == 0 && pid.compare(L"8a7e") == 0)
			{
				// this is a Treehopper!

				// get the serial from the path 
				// Path looks something like 
				// L"\\\\?\\usb#vid_10c4&pid_8a7e#asd123#{a5dcbf10-6530-11d2-901f-00c04fb951ed}"

				auto serialOffsetStart = Utility::nthOccurrence(devicePath, L"#", 2);
				auto serialOffsetEnd = Utility::nthOccurrence(devicePath, L"#", 3);
				wstring serial = devicePath.substr(serialOffsetStart, serialOffsetEnd - serialOffsetStart);

				// Get the FriendlyName
				bResult = SetupDiGetDevicePropertyW(devInfo,
					&devInfoData,
					&FriendlyName,
					&ulPropertyType,
					propBuffer,
					255,
					&length,
					NULL);

				if (FALSE == bResult)
				{
					WinUsbConnection::debugPrintLastError();
					SetupDiDestroyDeviceInfoList(devInfo);
					return;
				}

				wstring friendlyName = wstring((WCHAR*)propBuffer);

				// Get the HardwareIds
				bResult = SetupDiGetDevicePropertyW(devInfo,
					&devInfoData,
					&HardwareIds,
					&ulPropertyType,
					propBuffer,
					255,
					&length,
					NULL);

				if (FALSE == bResult)
				{
					WinUsbConnection::debugPrintLastError();
					SetupDiDestroyDeviceInfoList(devInfo);
					return;
				}

				wstring hardwareIds = wstring((WCHAR*)propBuffer);
				auto revOffset = hardwareIds.find(L"REV_") + 4;
				auto rev = wcstol(hardwareIds.substr(revOffset, 4).c_str(), NULL, 10);


				boards.emplace_back(*(new WinUsbConnection(devicePath, friendlyName, serial, rev)));
			}

			//LocalFree(deviceInterfaceDetailData);
		}

		SetupDiDestroyDeviceInfoList(devInfo);
	}
}