#include "MacUsbConnection.h"

namespace Treehopper
{
    MacUsbConnection::MacUsbConnection(io_service_t deviceService, string name, string serial)
    {
        io_name_t deviceName;
        SInt32 score;
        int kernelStatus;
        IOCFPlugInInterface** plugInInterface;
                
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
        
        deviceError = (*deviceInterface)->CreateInterfaceIterator(deviceInterface, &interfaceRequest, &interfaceIterator);
        assert(deviceError == kIOReturnSuccess && "Failed to create interface iterator");
        
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
            status = (*_currentInterfaceInterface)->ClearPipeStallBothEnds(_currentInterfaceInterface, 3);
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
            status = (*_currentInterfaceInterface)->ClearPipeStallBothEnds(_currentInterfaceInterface, 4);
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
            status = (*_currentInterfaceInterface)->ClearPipeStallBothEnds(_currentInterfaceInterface, 1);
            assert(status == kIOReturnSuccess);
            return false;
        }
        return true;
    }
}