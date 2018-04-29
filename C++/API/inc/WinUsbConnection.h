#pragma once

#include "Treehopper.h"
#include "UsbConnection.h"
#include <string>

#define WIN32_LEAN_AND_MEAN

#include <windows.h>
#include <SDKDDKVer.h>
#include <winusb.h>

using namespace std;

namespace Treehopper {
    class TREEHOPPER_API  WinUsbConnection : public UsbConnection {
    public:
        WinUsbConnection(wstring devicePath, wstring friendlyName, wstring serialNumber, int rev);

        ~WinUsbConnection() override;

        bool open() override;

        void close() override;

        void sendDataPinConfigChannel(uint8_t *data, size_t len) override;

        void sendDataPeripheralChannel(uint8_t *data, size_t len) override;

        wstring serialNumber() override;

        wstring name() override;

        wstring devicePath() override;

        bool receivePinReportPacket(uint8_t *data) override;

        bool receiveDataPeripheralChannel(uint8_t *data, size_t len) override;

        static void debugPrintLastError();

    private:
        typedef struct _DEVICE_DATA {

            BOOL HandlesOpen;
            WINUSB_INTERFACE_HANDLE WinusbHandle;
            HANDLE DeviceHandle;
            TCHAR DevicePath[MAX_PATH];

        } DEVICE_DATA, *PDEVICE_DATA;
        DEVICE_DATA deviceData;
    };
}