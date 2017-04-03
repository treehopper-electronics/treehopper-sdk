//
//  MacUsbConnectionService.cpp
//  Treehopper
//
//  Created by Jay Carlson on 4/2/17.
//  Copyright © 2017 Treehopper. All rights reserved.
//

#include "ConnectionService.h"
#include "IOKit/IOKitLib.h"
#include "IOKit/IOTypes.h"
#include "IOKit/usb/IOUSBLib.h"
#include "IOKit/IOCFPlugin.h"
#include "IOKit/usb/USBSpec.h"
#include "CoreFoundation/CoreFoundation.h"
#include "assert.h"
#include "Settings.h"
#include "MacUsbConnection.h"

namespace Treehopper {
    ConnectionService::ConnectionService()
    {
        scan();
    }
    
    ConnectionService::~ConnectionService()
    {
        
    }
    
    void ConnectionService::scan()
    {
        mach_port_t iokitPort = kIOMasterPortDefault;
        int kernelStatus;
        io_iterator_t deviceIterator;
        io_service_t deviceService;
        
        kernelStatus = IOServiceGetMatchingServices(iokitPort, IOServiceMatching(kIOUSBDeviceClassName), &deviceIterator);
        assert(kernelStatus == KERN_SUCCESS && "Failed to get matching service");
        
        while((deviceService = IOIteratorNext(deviceIterator))) {
            CFTypeRef path = IORegistryEntrySearchCFProperty(deviceService, kIOServicePlane, CFSTR(kUSBProductID), kCFAllocatorDefault, kIORegistryIterateRecursively);
            
            int productID;
            int vendorID;
            
            if(path)
            {
                CFNumberGetValue((CFNumberRef)path, kCFNumberIntType, &productID);
            }
            
            path = IORegistryEntrySearchCFProperty(deviceService, kIOServicePlane, CFSTR(kUSBVendorID), kCFAllocatorDefault, kIORegistryIterateRecursively);
            if(path)
            {
                CFNumberGetValue((CFNumberRef)path, kCFNumberIntType, &vendorID);
            }
            
            
            if(Settings::instance().vid == vendorID && Settings::instance().pid == productID)
            {
                boards.emplace_back(*(new MacUsbConnection(deviceService)));
            }
        }
    }
}