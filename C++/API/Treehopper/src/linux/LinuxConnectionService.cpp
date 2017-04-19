//
// Created by jay on 4/18/17.
//

#include "ConnectionService.h"
#include "libusb-1.0/libusb.h"

namespace Treehopper {
    ConnectionService::ConnectionService() {
        libusb_init(NULL);
    }

    ConnectionService::~ConnectionService() {

    }

    TreehopperUsb& ConnectionService::getFirstDevice() {

    }

    void ConnectionService::scan() {

    }
}