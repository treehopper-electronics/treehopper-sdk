using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Treehopper.Desktop.MacUsb.IOKit
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class IOUSBDeviceInterface320 : IUnknownCGuts
	{
		public CreateDeviceAsyncEventSourceDelegate CreateDeviceAsyncEventSource;
		public GetDeviceAsyncEventSourceDelegate GetDeviceAsyncEventSource;
		public CreateDeviceAsyncPortDelegate CreateDeviceAsyncPort;
		public GetDeviceAsyncPortDelegate GetDeviceAsyncPort;
		public USBDeviceOpenDelegate USBDeviceOpen;
		public USBDeviceCloseDelegate USBDeviceClose;
		public GetDeviceClassDelegate GetDeviceClass;
		public GetDeviceSubClassDelegate GetDeviceSubClass;
		public GetDeviceProtocolDelegate GetDeviceProtocol;
		public GetDeviceVendorDelegate GetDeviceVendor;
		public GetDeviceProductDelegate GetDeviceProduct;
		public GetDeviceReleaseNumberDelegate GetDeviceReleaseNumber;
		public GetDeviceAddressDelegate GetDeviceAddress;
		public GetDeviceBusPowerAvailableDelegate GetDeviceBusPowerAvailable;
		public GetDeviceSpeedDelegate GetDeviceSpeed;
		public GetNumberOfConfigurationsDelegate GetNumberOfConfigurations;
		public GetLocationIDDelegate GetLocationID;
		public GetConfigurationDescriptorPtrDelegate GetConfigurationDescriptorPtr;
		public GetConfigurationDelegate GetConfiguration;
		public SetConfigurationDelegate SetConfiguration;
		public GetBusFrameNumberDelegate GetBusFrameNumber;
		public ResetDeviceDelegate ResetDevice;
		public DeviceRequestDelegate DeviceRequest;
		public DeviceRequestAsyncDelegate DeviceRequestAsync;
		public CreateInterfaceIteratorDelegate CreateInterfaceIterator;
		public USBDeviceOpenSeizeDelegate USBDeviceOpenSieze;
		public DeviceRequestTODelegate DeviceRequestTO;
		public DeviceRequestAsyncTODelegate DeviceRequestAsyncTO;
		public USBDeviceSuspendDelegate USBDeviceSuspend;
		public USBDeviceAbortPipeZeroDelegate USBDeviceAbortPipeZero;
		public USBGetManufacturerStringIndexDelegate USBGetManufacturerStringIndex;
		public USBGetProductStringIndexDelegate USBGetProductStringIndex;
		public USBGetSerialNumberStringIndexDelegate USBGetSerialNumberStringIndex;
		public USBDeviceReEnumerateDelegate USBDeviceReEnumerate;
		public GetBusMicroFrameNumberDelegate GetBusMicroFrameNumber;
		public GetIOUSBLibVersionDelegate GetIOUSBLibVersion;
		public GetBusFrameNumberWithTimeDelegate GetBusFrameNumberWithTime;
		public GetUSBDeviceInformationDelegate GetUSBDeviceInformation;
		public RequestExtraPowerDelegate RequestExtraPower;
		public ReturnExtraPowerDelegate ReturnExtraPower;
		public GetExtraPowerAllocatedDelegate GetExtraPowerAllocated;
		public IntPtr Handle { get; set; }

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError CreateDeviceAsyncEventSourceDelegate (IntPtr self, IntPtr source);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetDeviceAsyncEventSourceDelegate (IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError CreateDeviceAsyncPortDelegate (IntPtr self, IntPtr port);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetDeviceAsyncPortDelegate (IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError USBDeviceOpenDelegate (IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError USBDeviceCloseDelegate (IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetDeviceClassDelegate (IntPtr self, out byte devClass);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetDeviceSubClassDelegate (IntPtr self, out byte devSubClass);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetDeviceProtocolDelegate (IntPtr self, out byte devProtocol);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetDeviceVendorDelegate (IntPtr self, out short devVendor);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetDeviceProductDelegate (IntPtr self, out short devProduct);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetDeviceReleaseNumberDelegate (IntPtr self, out short devRelNum);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetDeviceAddressDelegate (IntPtr self, out short addr);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetDeviceBusPowerAvailableDelegate (IntPtr self, out UInt32 powerAvailable);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetDeviceSpeedDelegate (IntPtr self, out byte devSpeed);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetNumberOfConfigurationsDelegate (IntPtr self, out byte numConfig);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetLocationIDDelegate (IntPtr self, out UInt32 locationID);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetConfigurationDescriptorPtrDelegate(IntPtr self, byte configIndex, out IOUSBConfigurationDescriptor desc); // IOUSBConfigurationDescriptorPtr

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetConfigurationDelegate (IntPtr self, out byte configNum);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError SetConfigurationDelegate (IntPtr self, byte configNum);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetBusFrameNumberDelegate (IntPtr self, out UInt64 frame, out long atTime);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError ResetDeviceDelegate (IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError DeviceRequestDelegate (IntPtr self, IntPtr req);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError DeviceRequestAsyncDelegate (IntPtr self, IntPtr req, IntPtr callback, IntPtr refCon);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError CreateInterfaceIteratorDelegate(IntPtr self, IOUSBFindInterfaceRequest req, out IntPtr iter);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError USBDeviceOpenSeizeDelegate (IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError DeviceRequestTODelegate (IntPtr self, IntPtr req);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError DeviceRequestAsyncTODelegate (IntPtr self, IntPtr req, IntPtr callback, IntPtr refCon);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError USBDeviceSuspendDelegate (IntPtr self, Boolean suspend);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError USBDeviceAbortPipeZeroDelegate (IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError USBGetManufacturerStringIndexDelegate (IntPtr self, out byte msi);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError USBGetProductStringIndexDelegate (IntPtr self, out byte psi);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError USBGetSerialNumberStringIndexDelegate (IntPtr self, out byte snsi);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError USBDeviceReEnumerateDelegate (IntPtr self, UInt32 options);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetBusMicroFrameNumberDelegate (IntPtr self, out UInt64 microFrame, out long atTime);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetIOUSBLibVersionDelegate (IntPtr self, IntPtr ioUSBLibVersion, IntPtr usbFamilyVersion);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetBusFrameNumberWithTimeDelegate(IntPtr self, out UInt64 frame, out long atTime);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetUSBDeviceInformationDelegate(IntPtr self, out UInt32 info);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError RequestExtraPowerDelegate(IntPtr self, UInt32 type, UInt32 requestedPower, out UInt32 powerAvailable);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError ReturnExtraPowerDelegate(IntPtr self, UInt32 type, UInt32 powerReturned);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetExtraPowerAllocatedDelegate(IntPtr self, UInt32 type, out UInt32 powerAllocated);
	}
}
