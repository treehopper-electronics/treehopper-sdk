//
// Created by jay on 4/18/17.
//

#include "LibUsbConnection.h"

namespace Treehopper {
    LibUsbConnection::LibUsbConnection(libusb_device *device) {
        this->deviceProfile = device;

        open();

        unsigned char buffer[128];

        libusb_get_string_descriptor_ascii(deviceHandle, 2, &buffer[0], 128);
        auto tmp = string((char*)buffer);
        _name = wstring(tmp.begin(), tmp.end());

        libusb_get_string_descriptor_ascii(deviceHandle, 3, &buffer[0], 128);
        tmp = string((char*)buffer);
        _serialNumber = wstring(tmp.begin(), tmp.end());


        close();

    }

    bool LibUsbConnection::open() {
        if(_isOpen) return _isOpen;
        if(libusb_open(this->deviceProfile, &deviceHandle) != LIBUSB_SUCCESS)
            return false;

        if(libusb_claim_interface(deviceHandle, 0) != LIBUSB_SUCCESS)
            return false;

        _isOpen = true;

        return _isOpen;
    }

    void LibUsbConnection::close() {
        if(!_isOpen) return;

        libusb_close(deviceHandle);
        deviceHandle = NULL;

        _isOpen = false;
    }

    wstring LibUsbConnection::devicePath() {

    }

    wstring LibUsbConnection::name() {

    }

    std::wstring LibUsbConnection::serialNumber() {

    }

    bool LibUsbConnection::receiveDataPeripheralChannel(uint8_t *data, size_t len) {
        int lenTransfered = 0;
        libusb_bulk_transfer(deviceHandle, peripheralResponseEndpoint, data, len, &lenTransfered, 1000);
    }

    void LibUsbConnection::sendDataPeripheralChannel(uint8_t *data, size_t len) {
        int lenTransfered = 0;
        libusb_bulk_transfer(deviceHandle, peripheralConfigEndpoint, data, len, &lenTransfered, 1000);
    }

    bool LibUsbConnection::receivePinReportPacket(uint8_t *data) {
        int count = 41;
        int lenTransfered = 0;
        libusb_bulk_transfer(deviceHandle, pinReportEndpoint, data, count, &lenTransfered, 1000);
        return lenTransfered == count;
    }

    void LibUsbConnection::sendDataPinConfigChannel(uint8_t *data, size_t len) {
        int lenTransfered = 0;
        libusb_bulk_transfer(deviceHandle, pinConfigEndpoint, data, len, &lenTransfered, 1000);
    }

    LibUsbConnection::~LibUsbConnection() {

    }
}