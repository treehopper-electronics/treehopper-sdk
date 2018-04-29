//
// Created by jay on 4/18/17.
//

#pragma once

#include "Treehopper.h"

#include <libusb-1.0/libusb.h>
#include "UsbConnection.h"

namespace Treehopper {
    class TREEHOPPER_API LibUsbConnection : public UsbConnection {
    public:
        LibUsbConnection(libusb_device *device);

        ~LibUsbConnection();

        bool open();

        void close();

        std::wstring serialNumber();

        wstring name();

        wstring devicePath();

        void sendDataPinConfigChannel(uint8_t *data, size_t len);

        void sendDataPeripheralChannel(uint8_t *data, size_t len);

        bool receiveDataPeripheralChannel(uint8_t *data, size_t len);

        bool receivePinReportPacket(uint8_t *data);

    protected:
        wstring _serialNumber;
        wstring _name;
        wstring _devicePath;
        bool _isOpen = false;
        uint8_t pinReportEndpoint = 0x81;
        uint8_t peripheralResponseEndpoint = 0x82;
        uint8_t pinConfigEndpoint = 0x01;
        uint8_t peripheralConfigEndpoint = 0x02;

        libusb_device *deviceProfile;
        libusb_device_handle *deviceHandle = NULL;
    };

}
