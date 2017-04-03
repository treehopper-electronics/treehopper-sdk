#include "MacUsbConnection.h"

namespace Treehopper
{
    MacUsbConnection::MacUsbConnection(io_service_t deviceService)
    {
        io_name_t deviceName;
        string name;
        string serial;
        SInt32 score;
        int kernelStatus;
        IOCFPlugInInterface** plugInInterface;
        
        char buffer[128];
        CFTypeRef path = IORegistryEntrySearchCFProperty(deviceService, kIOServicePlane, CFSTR(kUSBProductString), kCFAllocatorDefault, kIORegistryIterateRecursively);
        if(path)
        {
            CFStringGetCString((CFStringRef)path, buffer, 128, kCFStringEncodingUTF8);
            name = string(buffer);
            CFRelease(path);
        }
        
        
        path = IORegistryEntrySearchCFProperty(deviceService, kIOServicePlane, CFSTR(kUSBSerialNumberString), kCFAllocatorDefault, kIORegistryIterateRecursively);
        if(path)
        {
            CFStringGetCString((CFStringRef)path, buffer, 128, kCFStringEncodingUTF8);
            serial = string(buffer);
            CFRelease(path);
        }
        
        // Create CF plugin for device, this'll be used to get information.
        kernelStatus = IOCreatePlugInInterfaceForService(deviceService, kIOUSBDeviceUserClientTypeID, kIOCFPlugInInterfaceID, &plugInInterface, &score);
        assert(kernelStatus == KERN_SUCCESS && "Failed to get device");
        
        // Release objects
        kernelStatus = IOObjectRelease(deviceService);
        assert(kernelStatus == KERN_SUCCESS && "Failed to release kernel objects");
        
        // Verify object exists.
        if(plugInInterface) {
            uint16_t idVendor, idProduct;
            HRESULT resultingStatus;
            //                NSMutableDictionary* usbDictionary = [[NSMutableDictionary alloc] initWithCapacity:10];
            
            // Get device interface
            resultingStatus = (*plugInInterface)->QueryInterface(
                                                                 plugInInterface,
                                                                 CFUUIDGetUUIDBytes(kIOUSBDeviceInterfaceID320),
                                                                 (LPVOID*)&deviceInterface
                                                                 );
            assert(resultingStatus == kIOReturnSuccess && "Failed to create plug-in interface for device");
            (*plugInInterface)->Release(plugInInterface);
        }
    }
    
    MacUsbConnection::~MacUsbConnection()
    {
        
    }
    
    bool MacUsbConnection::open()
    {
        if((*deviceInterface)->USBDeviceOpenSeize(deviceInterface) != kIOReturnSuccess)
        {
            return false;
        }
        
        UInt8                           numConfig;
        IOReturn                        kr;
        IOUSBConfigurationDescriptorPtr configDesc;
        
        //Get the number of configurations. The sample code always chooses
        //the first configuration (at index 0) but your code may need a
        //different one
        kr = (*deviceInterface)->GetNumberOfConfigurations(deviceInterface, &numConfig);
        if (!numConfig)
            return false;
        
        //Get the configuration descriptor for index 0
        kr = (*deviceInterface)->GetConfigurationDescriptorPtr(deviceInterface, 0, &configDesc);
        if (kr)
        {
            printf("Couldn’t get configuration descriptor for index %d (err = %08x)\n", 0, kr);
            return false;
        }
        
        //Set the device’s configuration. The configuration value is found in
        //the bConfigurationValue field of the configuration descriptor
        kr = (*deviceInterface)->SetConfiguration(deviceInterface, configDesc->bConfigurationValue);
        if (kr)
        {
            printf("Couldn’t set configuration to value %d (err = %08x)\n", 0,
                   kr);
            return false;
        }
        
        IOReturn deviceError;
        SInt32 score;
        IOUSBFindInterfaceRequest interfaceRequest;
        io_iterator_t interfaceIterator;
        io_service_t usbInterface;
        
        // Construct request
        interfaceRequest.bInterfaceClass = kIOUSBFindInterfaceDontCare;
        interfaceRequest.bInterfaceSubClass = kIOUSBFindInterfaceDontCare;
        interfaceRequest.bInterfaceProtocol = kIOUSBFindInterfaceDontCare;
        interfaceRequest.bAlternateSetting = kIOUSBFindInterfaceDontCare;
        
        assert(deviceInterface != nil && "Device interface nonexistent; did you open the device?");
        
        deviceError = (*deviceInterface)->CreateInterfaceIterator(deviceInterface, &interfaceRequest, &interfaceIterator);
        assert(deviceError == kIOReturnSuccess && "Failed to create interface iterator");
        
        // Release current interface if it exists
        if(_currentInterfaceInterface != nil) {
            (*_currentInterfaceInterface)->USBInterfaceClose(_currentInterfaceInterface);
            (*_currentInterfaceInterface)->Release(_currentInterfaceInterface);
        }
        
        // Go through iterators
        usbInterface = IOIteratorNext(interfaceIterator);
        IOCFPlugInInterface **plugInInterface;
        IOUSBInterfaceInterface197 **__interface;
        UInt8 num;
        
        // Create CF plugin interface
        deviceError = IOCreatePlugInInterfaceForService(usbInterface, kIOUSBInterfaceUserClientTypeID, kIOCFPlugInInterfaceID, &plugInInterface, &score);
        assert(deviceError == kIOReturnSuccess && "Failed to create CF Plug-In interface");
        
        // Release objects.
        IOObjectRelease(usbInterface);
        
        assert(plugInInterface != NULL);
        
        // Get interface.
        deviceError = (*plugInInterface)->QueryInterface(plugInInterface, CFUUIDGetUUIDBytes(kIOUSBInterfaceInterfaceID), (LPVOID*)&__interface);
        assert(deviceError == kIOReturnSuccess && "Failed to query interface");
        
        // Check to make sure interface is valid.
        assert(__interface != NULL);
        
        // Release objects.
        (*plugInInterface)->Release(plugInInterface);
        
        // Get interface number and match it.
        deviceError = (*__interface)->GetInterfaceNumber(__interface, &num);
        assert(deviceError == kIOReturnSuccess && "Failed to get interface number");
        
        if(__interface && 0 == num) {
            deviceError = (*__interface)->USBInterfaceOpenSeize(__interface);
            assert(deviceError == kIOReturnSuccess && "Failed to open device interface");
            _currentInterfaceInterface = __interface;
        }
        
        IOUSBFindInterfaceRequest   request;
        io_iterator_t               iterator;
        HRESULT                     result;
        UInt8                       interfaceClass;
        UInt8                       interfaceSubClass;
        UInt8                       interfaceNumEndpoints;
        int                         pipeRef;
        //Get the number of endpoints associated with this interface
        kr = (*_currentInterfaceInterface)->GetNumEndpoints(_currentInterfaceInterface,
                                                            &interfaceNumEndpoints);
        if (kr != kIOReturnSuccess)
        {
            printf("Unable to get number of endpoints (%08x)\n", kr);
            (void) (*_currentInterfaceInterface)->USBInterfaceClose(_currentInterfaceInterface);
            (void) (*_currentInterfaceInterface)->Release(_currentInterfaceInterface);
            return false;
        }
        
        printf("Interface has %d endpoints\n", interfaceNumEndpoints);
        //Access each pipe in turn, starting with the pipe at index 1
        //The pipe at index 0 is the default control pipe and should be
        //accessed using (*usbDevice)->DeviceRequest() instead
        for (pipeRef = 1; pipeRef <= interfaceNumEndpoints; pipeRef++)
        {
            IOReturn        kr2;
            UInt8           direction;
            UInt8           number;
            UInt8           transferType;
            UInt16          maxPacketSize;
            UInt8           interval;
            char            *message;
            
            kr2 = (*_currentInterfaceInterface)->GetPipeProperties(_currentInterfaceInterface,
                                                                   pipeRef, &direction,
                                                                   &number, &transferType,
                                                                   &maxPacketSize, &interval);
            if (kr2 != kIOReturnSuccess)
                printf("Unable to get properties of pipe %d (%08x)\n",
                       pipeRef, kr2);
            else
            {
                printf("PipeRef %d: ", pipeRef);
                
                printf("Number %d (%08x)", number);
                
                switch (direction)
                {
                    case kUSBOut:
                        message = "out";
                        break;
                    case kUSBIn:
                        message = "in";
                        break;
                    case kUSBNone:
                        message = "none";
                        break;
                    case kUSBAnyDirn:
                        message = "any";
                        break;
                    default:
                        message = "???";
                }
                printf("direction %s, ", message);
                
                switch (transferType)
                {
                    case kUSBControl:
                        message = "control";
                        break;
                    case kUSBIsoc:
                        message = "isoc";
                        break;
                    case kUSBBulk:
                        message = "bulk";
                        break;
                    case kUSBInterrupt:
                        message = "interrupt";
                        break;
                    case kUSBAnyType:
                        message = "any";
                        break;
                    default:
                        message = "???";
                }
                printf("transfer type %s, maxPacketSize %d\n", message,
                       maxPacketSize);
            }
        }
        
        
        
        // Set alternate interface
        //            deviceError = (*_currentInterfaceInterface)->SetAlternateInterface(_currentInterfaceInterface, (uint8_t)0);
        //            if(deviceError == kIOUSBPipeStalled) {
        //                deviceError = (*_currentInterfaceInterface)->ClearPipeStallBothEnds(_currentInterfaceInterface, 0);
        //            }
        
        return true;
    }
    
    
    void MacUsbConnection::close()
    {
        
    }
    
    void MacUsbConnection::sendDataPinConfigChannel(uint8_t* data, size_t len)
    {
        IOReturn status;
        assert(_currentInterfaceInterface != nil && "Interface interface nonexistent; did you set a configuration?");
        status = (*_currentInterfaceInterface)->WritePipeTO(_currentInterfaceInterface, 3, data, (uint32_t)len, 1000, 1000);
        if(status != kIOReturnSuccess) {
            status = (*_currentInterfaceInterface)->ClearPipeStallBothEnds(_currentInterfaceInterface, peripheralConfigEndpoint);
            assert(status == kIOReturnSuccess);
        }
    }
    
    void MacUsbConnection::sendDataPeripheralChannel(uint8_t* data, size_t len)
    {
        IOReturn status;
        assert(_currentInterfaceInterface != nil && "Interface interface nonexistent; did you set a configuration?");
        status = (*_currentInterfaceInterface)->WritePipeTO(_currentInterfaceInterface, 4, data, (uint32_t)len, 100, 100);
        if(status != kIOReturnSuccess) {
            printf("WriteToDevice run returned err 0x%x\n", status);
            status = (*_currentInterfaceInterface)->ClearPipeStallBothEnds(_currentInterfaceInterface, 2);
            assert(status == kIOReturnSuccess);
        }
    }
    
    wstring MacUsbConnection::serialNumber()
    {
        return wstring();
    }
    
    wstring MacUsbConnection::name()
    {
        return wstring();
    }
    
    wstring MacUsbConnection::devicePath()
    {
        return wstring();
    }
    
    bool MacUsbConnection::receivePinReportPacket(uint8_t* data)
    {
        IOReturn status;
        assert(_currentInterfaceInterface != nil && "Interface interface nonexistent; did you set a configuration?");
        uint32_t lengthTransfered = 41;
        status = (*_currentInterfaceInterface)->ReadPipeTO(_currentInterfaceInterface, 1, data, &lengthTransfered, 100, 100);
        if(status != kIOReturnSuccess) {
            status = (*_currentInterfaceInterface)->ClearPipeStallBothEnds(_currentInterfaceInterface, 1);
            assert(status == kIOReturnSuccess);
            return false;
        }
        return true;
    }
    
    bool MacUsbConnection::receiveDataPeripheralChannel(uint8_t* data, size_t len)
    {
        IOReturn status;
        assert(_currentInterfaceInterface != nil && "Interface interface nonexistent; did you set a configuration?");
        uint32_t lengthTransfered = len;
        status = (*_currentInterfaceInterface)->ReadPipeTO(_currentInterfaceInterface, 1, data, &lengthTransfered, 100, 100);
        if(status != kIOReturnSuccess) {
            status = (*_currentInterfaceInterface)->ClearPipeStallBothEnds(_currentInterfaceInterface, peripheralResponseEndpoint);
            assert(status == kIOReturnSuccess);
            return false;
        }
        return true;
    }
}