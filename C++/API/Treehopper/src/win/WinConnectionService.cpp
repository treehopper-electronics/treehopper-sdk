#include "stdafx.h"
#include "ConnectionService.h"
#include <SetupAPI.h>
#include <string>
#include "WinUsbConnection.h"
#include <memory>

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

	void ConnectionService::scan()
	{
		BOOL                             bResult = FALSE;
		HDEVINFO                         deviceInfo;
		SP_DEVICE_INTERFACE_DATA         interfaceData;
		PSP_DEVICE_INTERFACE_DETAIL_DATA detailData = NULL;
		ULONG                            length;
		ULONG                            requiredLength = 0;
		HRESULT                          hr;

		// Enumerate all devices exposing the interface
		deviceInfo = SetupDiGetClassDevs(&GUID_DEVINTERFACE_testWinUsb,
			NULL,
			NULL,
			DIGCF_PRESENT | DIGCF_DEVICEINTERFACE);

		if (deviceInfo == INVALID_HANDLE_VALUE) {

			hr = HRESULT_FROM_WIN32(GetLastError());
			return;
		}

		interfaceData.cbSize = sizeof(SP_DEVICE_INTERFACE_DATA);


		int deviceIndex = 0;

		while (SetupDiEnumDeviceInterfaces(deviceInfo, NULL, &GUID_DEVINTERFACE_testWinUsb, deviceIndex++, &interfaceData))
		{
			//
			// Get the size of the path string
			// We expect to get a failure with insufficient buffer
			//
			bResult = SetupDiGetDeviceInterfaceDetail(deviceInfo,
				&interfaceData,
				NULL,
				0,
				&requiredLength,
				NULL);

			if (FALSE == bResult && ERROR_INSUFFICIENT_BUFFER != GetLastError()) {

				hr = HRESULT_FROM_WIN32(GetLastError());
				SetupDiDestroyDeviceInfoList(deviceInfo);
				return;
			}

			// Allocate temporary space for SetupDi structure
			detailData = (PSP_DEVICE_INTERFACE_DETAIL_DATA)
				LocalAlloc(LMEM_FIXED, requiredLength);

			if (NULL == detailData)
			{
				hr = E_OUTOFMEMORY;
				SetupDiDestroyDeviceInfoList(deviceInfo);
				return;
			}

			detailData->cbSize = sizeof(SP_DEVICE_INTERFACE_DETAIL_DATA);
			length = requiredLength;

			// Get the interface's path string
			bResult = SetupDiGetDeviceInterfaceDetail(deviceInfo,
				&interfaceData,
				detailData,
				length,
				&requiredLength,
				NULL);

			if (FALSE == bResult)
			{
				hr = HRESULT_FROM_WIN32(GetLastError());
				LocalFree(detailData);
				SetupDiDestroyDeviceInfoList(deviceInfo);
				return;
			}

			wstring devicePath = wstring(detailData->DevicePath);
			int vidIdx = devicePath.find(L"vid_") + 4;
			int pidIdx = devicePath.find(L"pid_") + 4;

			wstring vid = devicePath.substr(vidIdx, 4);
			wstring pid = devicePath.substr(pidIdx, 4);

			if (vid.compare(L"10c4") == 0 && pid.compare(L"8a7e") == 0)
			{
				// this is a Treehopper!
				//TreehopperUsb* board = new TreehopperUsb(new WinUsbConnection(devicePath));
				boards.emplace_back(*(new WinUsbConnection(devicePath)));
			}

			LocalFree(detailData);
		}

		SetupDiDestroyDeviceInfoList(deviceInfo);
	}
}