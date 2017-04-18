using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Treehopper.Desktop.MacUsb.IOKit
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class IOUSBDeviceInterface320 : IUnknownCGuts
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError CreateDeviceAsyncEventSourceDelegate(IntPtr self, IntPtr source);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError CreateDeviceAsyncPortDelegate(IntPtr self, IntPtr port);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError CreateInterfaceIteratorDelegate(IntPtr self, IOUSBFindInterfaceRequest req,
            out IntPtr iter);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError DeviceRequestAsyncDelegate(IntPtr self, IntPtr req, IntPtr callback, IntPtr refCon);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError DeviceRequestAsyncTODelegate(IntPtr self, IntPtr req, IntPtr callback,
            IntPtr refCon);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError DeviceRequestDelegate(IntPtr self, IntPtr req);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError DeviceRequestTODelegate(IntPtr self, IntPtr req);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetBusFrameNumberDelegate(IntPtr self, out ulong frame, out long atTime);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetBusFrameNumberWithTimeDelegate(IntPtr self, out ulong frame, out long atTime);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetBusMicroFrameNumberDelegate(IntPtr self, out ulong microFrame, out long atTime);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetConfigurationDelegate(IntPtr self, out byte configNum);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetConfigurationDescriptorPtrDelegate(IntPtr self, byte configIndex,
            out IOUSBConfigurationDescriptor desc); // IOUSBConfigurationDescriptorPtr

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetDeviceAddressDelegate(IntPtr self, out short addr);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetDeviceAsyncEventSourceDelegate(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetDeviceAsyncPortDelegate(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetDeviceBusPowerAvailableDelegate(IntPtr self, out uint powerAvailable);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetDeviceClassDelegate(IntPtr self, out byte devClass);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetDeviceProductDelegate(IntPtr self, out short devProduct);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetDeviceProtocolDelegate(IntPtr self, out byte devProtocol);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetDeviceReleaseNumberDelegate(IntPtr self, out short devRelNum);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetDeviceSpeedDelegate(IntPtr self, out byte devSpeed);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetDeviceSubClassDelegate(IntPtr self, out byte devSubClass);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetDeviceVendorDelegate(IntPtr self, out short devVendor);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetExtraPowerAllocatedDelegate(IntPtr self, uint type, out uint powerAllocated);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetIOUSBLibVersionDelegate(IntPtr self, IntPtr ioUSBLibVersion,
            IntPtr usbFamilyVersion);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetLocationIDDelegate(IntPtr self, out uint locationID);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetNumberOfConfigurationsDelegate(IntPtr self, out byte numConfig);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetUSBDeviceInformationDelegate(IntPtr self, out uint info);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError RequestExtraPowerDelegate(IntPtr self, uint type, uint requestedPower,
            out uint powerAvailable);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError ResetDeviceDelegate(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError ReturnExtraPowerDelegate(IntPtr self, uint type, uint powerReturned);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError SetConfigurationDelegate(IntPtr self, byte configNum);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError USBDeviceAbortPipeZeroDelegate(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError USBDeviceCloseDelegate(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError USBDeviceOpenDelegate(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError USBDeviceOpenSeizeDelegate(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError USBDeviceReEnumerateDelegate(IntPtr self, uint options);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError USBDeviceSuspendDelegate(IntPtr self, bool suspend);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError USBGetManufacturerStringIndexDelegate(IntPtr self, out byte msi);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError USBGetProductStringIndexDelegate(IntPtr self, out byte psi);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError USBGetSerialNumberStringIndexDelegate(IntPtr self, out byte snsi);

        public CreateDeviceAsyncEventSourceDelegate CreateDeviceAsyncEventSource;
        public CreateDeviceAsyncPortDelegate CreateDeviceAsyncPort;
        public CreateInterfaceIteratorDelegate CreateInterfaceIterator;
        public DeviceRequestDelegate DeviceRequest;
        public DeviceRequestAsyncDelegate DeviceRequestAsync;
        public DeviceRequestAsyncTODelegate DeviceRequestAsyncTO;
        public DeviceRequestTODelegate DeviceRequestTO;
        public GetBusFrameNumberDelegate GetBusFrameNumber;
        public GetBusFrameNumberWithTimeDelegate GetBusFrameNumberWithTime;
        public GetBusMicroFrameNumberDelegate GetBusMicroFrameNumber;
        public GetConfigurationDelegate GetConfiguration;
        public GetConfigurationDescriptorPtrDelegate GetConfigurationDescriptorPtr;
        public GetDeviceAddressDelegate GetDeviceAddress;
        public GetDeviceAsyncEventSourceDelegate GetDeviceAsyncEventSource;
        public GetDeviceAsyncPortDelegate GetDeviceAsyncPort;
        public GetDeviceBusPowerAvailableDelegate GetDeviceBusPowerAvailable;
        public GetDeviceClassDelegate GetDeviceClass;
        public GetDeviceProductDelegate GetDeviceProduct;
        public GetDeviceProtocolDelegate GetDeviceProtocol;
        public GetDeviceReleaseNumberDelegate GetDeviceReleaseNumber;
        public GetDeviceSpeedDelegate GetDeviceSpeed;
        public GetDeviceSubClassDelegate GetDeviceSubClass;
        public GetDeviceVendorDelegate GetDeviceVendor;
        public GetExtraPowerAllocatedDelegate GetExtraPowerAllocated;
        public GetIOUSBLibVersionDelegate GetIOUSBLibVersion;
        public GetLocationIDDelegate GetLocationID;
        public GetNumberOfConfigurationsDelegate GetNumberOfConfigurations;
        public GetUSBDeviceInformationDelegate GetUSBDeviceInformation;
        public RequestExtraPowerDelegate RequestExtraPower;
        public ResetDeviceDelegate ResetDevice;
        public ReturnExtraPowerDelegate ReturnExtraPower;
        public SetConfigurationDelegate SetConfiguration;
        public USBDeviceAbortPipeZeroDelegate USBDeviceAbortPipeZero;
        public USBDeviceCloseDelegate USBDeviceClose;
        public USBDeviceOpenDelegate USBDeviceOpen;
        public USBDeviceOpenSeizeDelegate USBDeviceOpenSieze;
        public USBDeviceReEnumerateDelegate USBDeviceReEnumerate;
        public USBDeviceSuspendDelegate USBDeviceSuspend;
        public USBGetManufacturerStringIndexDelegate USBGetManufacturerStringIndex;
        public USBGetProductStringIndexDelegate USBGetProductStringIndex;
        public USBGetSerialNumberStringIndexDelegate USBGetSerialNumberStringIndex;
        public IntPtr Handle { get; set; }
    }
}