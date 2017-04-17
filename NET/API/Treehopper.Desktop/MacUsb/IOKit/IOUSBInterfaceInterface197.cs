using System;
using System.Runtime.InteropServices;

namespace Treehopper.Desktop.MacUsb.IOKit
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class IOUSBInterfaceInterface197 : IUnknownCGuts
	{
		public CreateInterfaceAsyncEventSourceDelegate CreateInterfaceAsyncEventSource;
		public GetInterfaceAsyncEventSourceDelegate GetInterfaceAsyncEventSource;
		public CreateInterfaceAsyncPortDelegate CreateInterfaceAsyncPort;
		public GetInterfaceAsyncPortDelegate GetInterfaceAsyncPort;
		public USBInterfaceOpenDelegate USBInterfaceOpen;
		public USBInterfaceCloseDelegate USBInterfaceClose;
		public GetInterfaceClassDelegate GetInterfaceClass;
		public GetInterfaceSubClassDelegate GetInterfaceSubClass;
		public GetInterfaceProtocolDelegate GetInterfaceProtocol;
		public GetDeviceVendorDelegate GetDeviceVendor;
		public GetDeviceProductDelegate GetDeviceProduct;
		public GetDeviceReleaseNumberDelegate GetDeviceReleaseNumber;
		public GetConfigurationValueDelegate GetConfigurationValue;
		public GetInterfaceNumberDelegate GetInterfaceNumber;
		public GetAlternateSettingDelegate GetAlternateSetting;
		public GetNumEndpointsDelegate GetNumEndpoints;
		public GetLocationIDDelegate GetLocationID;
		public GetDeviceDelegate GetDevice;
		public SetAlternateInterfaceDelegate SetAlternateInterface;
		public GetBusFrameNumberDelegate GetBusFrameNumber;
		public ControlRequestDelegate ControlRequest;
		public ControlRequestAsyncDelegate ControlRequestAsync;
		public GetPipePropertiesDelegate GetPipeProperties;
		public GetPipeStatusDelegate GetPipeStatus;
		public AbortPipeDelegate AbortPipe;
		public ResetPipeDelegate ResetPipe;
		public ClearPipeStallDelegate ClearPipeStall;
		public ReadPipeDelegate ReadPipe;
		public WritePipeDelegate WritePipe;
		public ReadPipeAsyncDelegate ReadPipeAsync;
		public WritePipeAsyncDelegate WritePipeAsync;
		public ReadIsochPipeAsyncDelegate ReadIsochPipeAsync;
		public WriteIsochPipeAsyncDelegate WriteIsochPipeAsync;
		public ControlRequestTODelegate ControlRequestTO;
		public ControlRequestAsyncTODelegate ControlRequestAsyncTO;
		public ReadPipeTODelegate ReadPipeTO;
		public WritePipeTODelegate WritePipeTO;
		public ReadPipeAsyncTODelegate ReadPipeAsyncTO;
		public WritePipeAsyncTODelegate WritePipeAsyncTO;
		public USBInterfaceGetStringIndexDelegate USBInterfaceGetStringIndex;
		public USBInterfaceOpenSeizeDelegate USBInterfaceOpenSeize;
		public ClearPipeStallBothEndsDelegate ClearPipeStallBothEnds;
		public SetPipePolicyDelegate SetPipePolicy;
		public GetBandwidthAvailableDelegate GetBandwidthAvailable;
		public GetEndpointPropertiesDelegate GetEndpointProperties;
		public LowLatencyReadIsochPipeAsyncDelegate LowLatencyReadIsochPipeAsync;
		public LowLatencyWriteIsochPipeAsyncDelegate LowLatencyWriteIsochPipeAsync;
		public LowLatencyCreateBufferDelegate LowLatencyCreateBuffer;
		public LowLatencyDestroyBufferDelegate LowLatencyDestroyBuffer;
		public GetBusMicroFrameNumberDelegate GetBusMicroFrameNumber;
		public GetFrameListTimeDelegate GetFrameListTime;
		public GetIOUSBLibVersionDelegate GetIOUSBLibVersion;

		public IntPtr Handle { get; set; }

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError CreateInterfaceAsyncEventSourceDelegate(IntPtr self, IntPtr source);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr GetInterfaceAsyncEventSourceDelegate (IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError CreateInterfaceAsyncPortDelegate(IntPtr self, IntPtr port);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetInterfaceAsyncPortDelegate (IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError USBInterfaceOpenDelegate(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError USBInterfaceCloseDelegate(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetInterfaceClassDelegate(IntPtr self, out byte intfClass);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetInterfaceSubClassDelegate(IntPtr self, out byte intfSubClass);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetInterfaceProtocolDelegate(IntPtr self, out byte intfProtocol);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetDeviceVendorDelegate(IntPtr self, out ushort devVendor);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetDeviceProductDelegate(IntPtr self, out ushort devProduct);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetDeviceReleaseNumberDelegate(IntPtr self, out ushort devRelNum);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetConfigurationValueDelegate(IntPtr self, out byte configVal);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetInterfaceNumberDelegate(IntPtr self, out byte intfNumber);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetAlternateSettingDelegate(IntPtr self, out byte intfAltSetting);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetNumEndpointsDelegate(IntPtr self, out byte intfNumEndpoints);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetLocationIDDelegate(IntPtr self, out UInt32 locationID);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetDeviceDelegate(IntPtr self, IntPtr device);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError SetAlternateInterfaceDelegate(IntPtr self, byte alternateSetting);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetBusFrameNumberDelegate(IntPtr self, out UInt64 frame, out long atTime);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError ControlRequestDelegate(IntPtr self, byte pipeRef, IntPtr req);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError ControlRequestAsyncDelegate(IntPtr self, byte pipeRef, IntPtr req, IntPtr callback, IntPtr refCon);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetPipePropertiesDelegate(IntPtr self, byte pipeRef, out byte direction, out byte number, out byte transferType, out UInt16 maxPacketSize, out byte interval);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetPipeStatusDelegate(IntPtr self, byte pipeRef);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError AbortPipeDelegate(IntPtr self, byte pipeRef);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError ResetPipeDelegate(IntPtr self, byte pipeRef);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError ClearPipeStallDelegate(IntPtr self, byte pipeRef);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError ReadPipeDelegate(IntPtr self, byte pipeRef, IntPtr buf, out UInt32 size);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError WritePipeDelegate(IntPtr self, byte pipeRef, IntPtr buf, UInt32 size);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError ReadPipeAsyncDelegate(IntPtr self, byte pipeRef, IntPtr buf, UInt32 size, IntPtr callback, IntPtr refcon);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError WritePipeAsyncDelegate(IntPtr self, byte pipeRef, IntPtr buf, UInt32 size, IntPtr callback, IntPtr refcon);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError ReadIsochPipeAsyncDelegate(IntPtr self, byte pipeRef, IntPtr buf, UInt64 frameStart, UInt32 numFrames, IntPtr frameList, IntPtr callback, IntPtr refcon);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError WriteIsochPipeAsyncDelegate(IntPtr self, byte pipeRef, IntPtr buf, UInt64 frameStart, UInt32 numFrames, IntPtr frameList, IntPtr callback, IntPtr refcon);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError ControlRequestTODelegate(IntPtr self, byte pipeRef, IntPtr req);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError ControlRequestAsyncTODelegate(IntPtr self, byte pipeRef, IntPtr req, IntPtr callback, IntPtr refCon);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError ReadPipeTODelegate(IntPtr self, byte pipeRef, byte[] buf, out UInt32 size, UInt32 noDataTimeout, UInt32 completionTimeout);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError WritePipeTODelegate(IntPtr self, byte pipeRef, byte[] buf, UInt32 size, UInt32 noDataTimeout, UInt32 completionTimeout);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError ReadPipeAsyncTODelegate(IntPtr self, byte pipeRef, IntPtr buf, UInt32 size, UInt32 noDataTimeout, UInt32 completionTimeout, IntPtr callback, IntPtr refcon);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError WritePipeAsyncTODelegate(IntPtr self, byte pipeRef, IntPtr buf, UInt32 size, UInt32 noDataTimeout, UInt32 completionTimeout, IntPtr callback, IntPtr refcon);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError USBInterfaceGetStringIndexDelegate(IntPtr self, out byte si);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError USBInterfaceOpenSeizeDelegate(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError ClearPipeStallBothEndsDelegate(IntPtr self, byte pipeRef);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError SetPipePolicyDelegate(IntPtr self, byte pipeRef, UInt16 maxPacketSize, byte maxInterval);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetBandwidthAvailableDelegate(IntPtr self, out UInt32 bandwidth);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetEndpointPropertiesDelegate(IntPtr self, byte alternateSetting, byte endpointNumber, byte direction, out byte transferType, out UInt16 maxPacketSize, out byte interval);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError LowLatencyReadIsochPipeAsyncDelegate(IntPtr self, byte pipeRef, IntPtr buf, UInt64 frameStart, UInt32 numFrames, UInt32 updateFrequency, IntPtr frameList, IntPtr callback, IntPtr refcon);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError LowLatencyWriteIsochPipeAsyncDelegate(IntPtr self, byte pipeRef, IntPtr buf, UInt64 frameStart, UInt32 numFrames, UInt32 updateFrequency, IntPtr frameList, IntPtr callback, IntPtr refcon);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError LowLatencyCreateBufferDelegate(IntPtr self, IntPtr buffer, int size, UInt32 bufferType);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError LowLatencyDestroyBufferDelegate(IntPtr self, IntPtr buffer );

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetBusMicroFrameNumberDelegate(IntPtr self, out UInt64 microFrame, out long atTime);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetFrameListTimeDelegate(IntPtr self, out UInt32 microsecondsInFrame);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IOKitError GetIOUSBLibVersionDelegate(IntPtr self, IntPtr ioUSBLibVersion, IntPtr usbFamilyVersion);

	}
}
