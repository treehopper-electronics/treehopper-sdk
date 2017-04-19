//
// Created by jay on 4/18/17.
//

#include <Settings.h>
#include <LibUsbConnection.h>
#include "ConnectionService.h"
namespace Treehopper {
    ConnectionService::ConnectionService() {
        libusb_init(&context);
        scan();
    }

    ConnectionService::~ConnectionService() {
        libusb_exit(context);
    }

    TreehopperUsb& ConnectionService::getFirstDevice() {
        return boards[0];
    }

    void ConnectionService::scan() {
        libusb_device** deviceList;

        ssize_t count = libusb_get_device_list(context, &deviceList);
        ssize_t i = 0;

        if(count < 0)
            return;

        for(i = 0; i < count; i++)
        {
            libusb_device *device = deviceList[i];
            libusb_device_descriptor descriptor;
            libusb_get_device_descriptor(device, &descriptor);

            if(descriptor.idVendor == Settings::instance().vid && descriptor.idProduct == Settings::instance().pid)
            {
                boards.emplace_back(*new LibUsbConnection(device));
            }
        }
    }
}