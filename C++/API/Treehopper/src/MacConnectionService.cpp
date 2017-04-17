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

typedef struct MyPrivateData {
    io_object_t             notification;
    IOUSBDeviceInterface    **deviceInterface;
    CFStringRef             deviceName;
    UInt32                  locationID;
} MyPrivateData;

namespace Treehopper {
    
    IONotificationPortRef ConnectionService::gNotifyPort;
    std::mutex ConnectionService::boardCollectionMutex;
    std::condition_variable ConnectionService::boardCollectionCondition;
    std::thread ConnectionService::deviceListenerThread;
    
    
    void ConnectionService::DeviceRemoved(void *refCon, io_service_t service, natural_t messageType, void *messageArgument)
    {
        kern_return_t   kr;
        MyPrivateData   *privateDataRef = (MyPrivateData *) refCon;
        
        if (messageType == kIOMessageServiceIsTerminated) {
            fprintf(stderr, "Device removed.\n");
            
            // Dump our private data to stderr just to see what it looks like.
            fprintf(stderr, "privateDataRef->deviceName: ");
            CFShow(privateDataRef->deviceName);
            fprintf(stderr, "privateDataRef->locationID: 0x%lx.\n\n", privateDataRef->locationID);
            
            // Free the data we're no longer using now that the device is going away
            CFRelease(privateDataRef->deviceName);
            
            if (privateDataRef->deviceInterface) {
                kr = (*privateDataRef->deviceInterface)->Release(privateDataRef->deviceInterface);
            }
            
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
            CFStringRef     deviceNameAsCFString;
            MyPrivateData   *privateDataRef = NULL;
            UInt32          locationID;
            
            // Add some app-specific information about this device.
            // Create a buffer to hold the data.
            privateDataRef = (MyPrivateData*)malloc(sizeof(MyPrivateData));
            bzero(privateDataRef, sizeof(MyPrivateData));
            
            // Get the USB device's name.
            kr = IORegistryEntryGetName(usbDevice, deviceName);
            if (KERN_SUCCESS != kr) {
                deviceName[0] = '\0';
            }
            
            deviceNameAsCFString = CFStringCreateWithCString(kCFAllocatorDefault, deviceName,
                                                             kCFStringEncodingASCII);
            
            // Dump our data to stderr just to see what it looks like.
            fprintf(stderr, "deviceName: ");
            CFShow(deviceNameAsCFString);
            
            // Save the device's name to our private data.
            privateDataRef->deviceName = deviceNameAsCFString;

            kr = IOCreatePlugInInterfaceForService(usbDevice, kIOUSBDeviceUserClientTypeID, kIOCFPlugInInterfaceID, &plugInInterface, &score);
            
            if ((kIOReturnSuccess != kr) || !plugInInterface) {
                fprintf(stderr, "IOCreatePlugInInterfaceForService returned 0x%08x.\n", kr);
                continue;
            }
            
            // Use the plugin interface to retrieve the device interface.
            res = (*plugInInterface)->QueryInterface(plugInInterface, CFUUIDGetUUIDBytes(kIOUSBDeviceInterfaceID),
                                                     (LPVOID*) &privateDataRef->deviceInterface);
            
            // Now done with the plugin interface.
            (*plugInInterface)->Release(plugInInterface);
            
            if (res || privateDataRef->deviceInterface == NULL) {
                fprintf(stderr, "QueryInterface returned %d.\n", (int) res);
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
           
            service->boards.emplace_back(*(new MacUsbConnection(usbDevice, name, serial)));
            
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
            
            
            
            fprintf(stderr, "Looking for devices matching vendor ID=%ld and product ID=%ld.\n", usbVendor, usbProduct);
            
           
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
            fprintf(stderr, "Starting run loop.\n\n");
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