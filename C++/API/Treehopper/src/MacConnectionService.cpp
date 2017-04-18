//
//  MacUsbConnectionService.cpp
//  Treehopper
//
//  Created by Jay Carlson on 4/2/17.
//  Copyright © 2017 Treehopper. All rights reserved.
//

#include "ConnectionService.h"
#include <IOKit/IOMessage.h>
#include <IOKit/IOKitLib.h>
#include <IOKit/IOTypes.h>
#include <IOKit/usb/IOUSBLib.h>
#include <IOKit/IOCFPlugin.h>
#include <IOKit/usb/USBSpec.h>
#include <CoreFoundation/CoreFoundation.h>
#include "assert.h"
#include "Settings.h"
#include "MacUsbConnection.h"
#include <thread>
#include <iostream>
namespace Treehopper {

    typedef struct BoardConnectionData {
        io_object_t             notification;
        ConnectionService       *connectionService;
        UInt32                  boardId;
    } MyPrivateData;
    
    IONotificationPortRef ConnectionService::gNotifyPort;
    std::mutex ConnectionService::boardCollectionMutex;
    std::condition_variable ConnectionService::boardCollectionCondition;
    std::thread ConnectionService::deviceListenerThread;
    
    void ConnectionService::DeviceRemoved(void *refCon, io_service_t service, natural_t messageType, void *messageArgument)
    {
        if (messageType == kIOMessageServiceIsTerminated) {

            kern_return_t   kr;
            BoardConnectionData *privateDataRef = (BoardConnectionData *) refCon;
            
            int boardId = privateDataRef->boardId;
            

            std::vector<TreehopperUsb> &boards = privateDataRef->connectionService->boards;
            
            std::wcerr << L"Removing: " << boards[boardId].toString() << std::endl;
            
            boards[boardId].disconnect();
            boards.erase(boards.begin() + boardId);
            
            kr = IOObjectRelease(privateDataRef->notification);
            
            free(privateDataRef);
        }
    }
    
    void ConnectionService::DeviceAdded(void *refCon, io_iterator_t iterator)
    {
        kern_return_t       kr;
        io_service_t        usbDevice;
        IOCFPlugInInterface **plugInInterface = NULL;
        SInt32              score;
        HRESULT             res;
        
        ConnectionService* service = (ConnectionService*)refCon;
        
        while ((usbDevice = IOIteratorNext(iterator))) {
            io_name_t       deviceName;
            MyPrivateData   *privateDataRef = NULL;
            
            // Add some app-specific information about this device.
            // Create a buffer to hold the data.
            privateDataRef = (BoardConnectionData*)malloc(sizeof(BoardConnectionData));
            bzero(privateDataRef, sizeof(MyPrivateData));
            
            
            privateDataRef->connectionService = service;
            
            
            // Get the USB device's name.
            kr = IORegistryEntryGetName(usbDevice, deviceName);
            if (KERN_SUCCESS != kr) {
                deviceName[0] = '\0';
            }
            
            kr = IOCreatePlugInInterfaceForService(usbDevice, kIOUSBDeviceUserClientTypeID, kIOCFPlugInInterfaceID, &plugInInterface, &score);
            
            if ((kIOReturnSuccess != kr) || !plugInInterface) {
                fprintf(stderr, "IOCreatePlugInInterfaceForService returned 0x%08x.\n", kr);
                continue;
            }
            
           
            // Register for an interest notification of this device being removed. Use a reference to our
            // private data as the refCon which will be passed to the notification callback.
            kr = IOServiceAddInterestNotification(gNotifyPort,                      // notifyPort
                                                  usbDevice,                        // service
                                                  kIOGeneralInterest,               // interestType
                                                  DeviceRemoved,                    // callback
                                                  privateDataRef,                   // refCon
                                                  &(privateDataRef->notification)   // notification
                                                  );
            
            
            char buffer[128];
            string name = string(deviceName);
            string serial;
            
            auto path = IORegistryEntrySearchCFProperty(usbDevice, kIOServicePlane, CFSTR(kUSBSerialNumberString), kCFAllocatorDefault, kIORegistryIterateRecursively);
            if(path)
            {
                CFStringGetCString((CFStringRef)path, buffer, 128, kCFStringEncodingUTF8);
                serial = string(buffer);
                CFRelease(path);
            }
            
            std::unique_lock<std::mutex> lock(boardCollectionMutex);
            privateDataRef->boardId = service->boards.size();
            service->boards.emplace_back(*(new MacUsbConnection(usbDevice, name, serial)));
            std::wcerr << "Adding: " << service->boards[privateDataRef->boardId].toString() << std::endl;
            
            lock.unlock();
            
            boardCollectionCondition.notify_one();
            
            if (KERN_SUCCESS != kr) {
                printf("IOServiceAddInterestNotification returned 0x%08x.\n", kr);
            }
        }
    }

    ConnectionService::ConnectionService()
    {
        deviceListenerThread = std::thread([&]() {
            io_iterator_t           gAddedIter;
            CFMutableDictionaryRef  matchingDict;
            CFRunLoopSourceRef      runLoopSource;
            CFNumberRef             numberRef;
            kern_return_t           kr;
            long                    usbVendor = Settings::instance().vid;
            long                    usbProduct = Settings::instance().pid;
            sig_t                   oldHandler;
            
            matchingDict = IOServiceMatching(kIOUSBDeviceClassName);    // Interested in instances of class
            // IOUSBDevice and its subclasses
            if (matchingDict == NULL) {
                fprintf(stderr, "IOServiceMatching returned NULL.\n");
                return;
            }
            
            // We are interested in all USB devices (as opposed to USB interfaces).  The Common Class Specification
            // tells us that we need to specify the idVendor, idProduct, and bcdDevice fields, or, if we're not interested
            // in particular bcdDevices, just the idVendor and idProduct.  Note that if we were trying to match an
            // IOUSBInterface, we would need to set more values in the matching dictionary (e.g. idVendor, idProduct,
            // bInterfaceNumber and bConfigurationValue.
            
            // Create a CFNumber for the idVendor and set the value in the dictionary
            numberRef = CFNumberCreate(kCFAllocatorDefault, kCFNumberSInt32Type, &usbVendor);
            CFDictionarySetValue(matchingDict,
                                 CFSTR(kUSBVendorID),
                                 numberRef);
            CFRelease(numberRef);
            
            // Create a CFNumber for the idProduct and set the value in the dictionary
            numberRef = CFNumberCreate(kCFAllocatorDefault, kCFNumberSInt32Type, &usbProduct);
            CFDictionarySetValue(matchingDict,
                                 CFSTR(kUSBProductID),
                                 numberRef);
            CFRelease(numberRef);
            numberRef = NULL;
            
            // Create a notification port and add its run loop event source to our run loop
            // This is how async notifications get set up.
            
            gNotifyPort = IONotificationPortCreate(kIOMasterPortDefault);
            runLoopSource = IONotificationPortGetRunLoopSource(gNotifyPort);
            
            gRunLoop = CFRunLoopGetCurrent();
            CFRunLoopAddSource(gRunLoop, runLoopSource, kCFRunLoopDefaultMode);
            
            // Now set up a notification to be called when a device is first matched by I/O Kit.
            kr = IOServiceAddMatchingNotification(gNotifyPort,                  // notifyPort
                                                  kIOFirstMatchNotification,    // notificationType
                                                  matchingDict,                 // matching
                                                  static_cast<IOServiceMatchingCallback>(ConnectionService::DeviceAdded),                  // callback
                                                  this,                         // refCon
                                                  &gAddedIter                   // notification
                                                  );
            
            // Iterate once to get already-present devices and arm the notification
            DeviceAdded(this, gAddedIter);
            
            // Start the run loop. Now we'll receive notifications.
            CFRunLoopRun();
        });

    }
    
    TreehopperUsb& ConnectionService::getFirstDevice()
    {
        std::unique_lock<std::mutex> lock(boardCollectionMutex);
        while(boards.empty())
        {
            boardCollectionCondition.wait(lock);
        }
        return boards[0];
    }
    
    ConnectionService::~ConnectionService()
    {
        CFRunLoopStop(gRunLoop);
    }
}