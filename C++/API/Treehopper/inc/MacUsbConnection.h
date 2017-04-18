#pragma once
#include "Treehopper.h"
#include "UsbConnection.h"
#include <string>
#include "IOKit/IOKitLib.h"
#include "IOKit/IOTypes.h"
#include "IOKit/usb/IOUSBLib.h"
#include "IOKit/IOCFPlugin.h"
#include "IOKit/usb/USBSpec.h"
#include "CoreFoundation/CoreFoundation.h"

using namespace std;

namespace Treehopper
{
    class TREEHOPPER_API  MacUsbConnection : public UsbConnection
    {
    public:
        MacUsbConnection(io_service_t deviceService, string name, string serial);
        ~MacUsbConnection();
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
        wstring _name;
        wstring _serialNumber;
        int _currentConfiguration;
        int _currentInterface = 0;
        uint8_t _currentAlternateInterface = 0;
        IOCFPlugInInterface** _currentPlugInInterface = NULL;
        IOUSBDeviceInterface320** deviceInterface = NULL;
        IOUSBInterfaceInterface197** _currentInterfaceInterface = NULL;
    };
}
