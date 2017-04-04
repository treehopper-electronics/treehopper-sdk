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
		public delegate int CreateInterfaceAsyncEventSourceDelegate(IntPtr self, IntPtr source);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate IntPtr GetInterfaceAsyncEventSourceDelegate (IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int CreateInterfaceAsyncPortDelegate(IntPtr self, IntPtr port);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int GetInterfaceAsyncPortDelegate (IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int USBInterfaceOpenDelegate(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int USBInterfaceCloseDelegate(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int GetInterfaceClassDelegate(IntPtr self, out byte intfClass);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int GetInterfaceSubClassDelegate(IntPtr self, out byte intfSubClass);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int GetInterfaceProtocolDelegate(IntPtr self, out byte intfProtocol);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int GetDeviceVendorDelegate(IntPtr self, out ushort devVendor);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int GetDeviceProductDelegate(IntPtr self, out ushort devProduct);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int GetDeviceReleaseNumberDelegate(IntPtr self, out ushort devRelNum);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int GetConfigurationValueDelegate(IntPtr self, out byte configVal);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int GetInterfaceNumberDelegate(IntPtr self, out byte intfNumber);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int GetAlternateSettingDelegate(IntPtr self, out byte intfAltSetting);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int GetNumEndpointsDelegate(IntPtr self, out byte intfNumEndpoints);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int GetLocationIDDelegate(IntPtr self, out UInt32 locationID);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int GetDeviceDelegate(IntPtr self, IntPtr device);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int SetAlternateInterfaceDelegate(IntPtr self, byte alternateSetting);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int GetBusFrameNumberDelegate(IntPtr self, out UInt64 frame, out long atTime);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int ControlRequestDelegate(IntPtr self, byte pipeRef, IntPtr req);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int ControlRequestAsyncDelegate(IntPtr self, byte pipeRef, IntPtr req, IntPtr callback, IntPtr refCon);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int GetPipePropertiesDelegate(IntPtr self, byte pipeRef, out byte direction, out byte number, out byte transferType, out UInt16 maxPacketSize, out byte interval);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int GetPipeStatusDelegate(IntPtr self, byte pipeRef);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int AbortPipeDelegate(IntPtr self, byte pipeRef);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int ResetPipeDelegate(IntPtr self, byte pipeRef);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int ClearPipeStallDelegate(IntPtr self, byte pipeRef);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int ReadPipeDelegate(IntPtr self, byte pipeRef, IntPtr buf, out UInt32 size);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int WritePipeDelegate(IntPtr self, byte pipeRef, IntPtr buf, UInt32 size);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int ReadPipeAsyncDelegate(IntPtr self, byte pipeRef, IntPtr buf, UInt32 size, IntPtr callback, IntPtr refcon);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int WritePipeAsyncDelegate(IntPtr self, byte pipeRef, IntPtr buf, UInt32 size, IntPtr callback, IntPtr refcon);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int ReadIsochPipeAsyncDelegate(IntPtr self, byte pipeRef, IntPtr buf, UInt64 frameStart, UInt32 numFrames, IntPtr frameList, IntPtr callback, IntPtr refcon);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int WriteIsochPipeAsyncDelegate(IntPtr self, byte pipeRef, IntPtr buf, UInt64 frameStart, UInt32 numFrames, IntPtr frameList, IntPtr callback, IntPtr refcon);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int ControlRequestTODelegate(IntPtr self, byte pipeRef, IntPtr req);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int ControlRequestAsyncTODelegate(IntPtr self, byte pipeRef, IntPtr req, IntPtr callback, IntPtr refCon);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int ReadPipeTODelegate(IntPtr self, byte pipeRef, byte[] buf, out UInt32 size, UInt32 noDataTimeout, UInt32 completionTimeout);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int WritePipeTODelegate(IntPtr self, byte pipeRef, byte[] buf, UInt32 size, UInt32 noDataTimeout, UInt32 completionTimeout);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int ReadPipeAsyncTODelegate(IntPtr self, byte pipeRef, IntPtr buf, UInt32 size, UInt32 noDataTimeout, UInt32 completionTimeout, IntPtr callback, IntPtr refcon);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int WritePipeAsyncTODelegate(IntPtr self, byte pipeRef, IntPtr buf, UInt32 size, UInt32 noDataTimeout, UInt32 completionTimeout, IntPtr callback, IntPtr refcon);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int USBInterfaceGetStringIndexDelegate(IntPtr self, out byte si);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int USBInterfaceOpenSeizeDelegate(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int ClearPipeStallBothEndsDelegate(IntPtr self, byte pipeRef);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int SetPipePolicyDelegate(IntPtr self, byte pipeRef, UInt16 maxPacketSize, byte maxInterval);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int GetBandwidthAvailableDelegate(IntPtr self, out UInt32 bandwidth);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int GetEndpointPropertiesDelegate(IntPtr self, byte alternateSetting, byte endpointNumber, byte direction, out byte transferType, out UInt16 maxPacketSize, out byte interval);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int LowLatencyReadIsochPipeAsyncDelegate(IntPtr self, byte pipeRef, IntPtr buf, UInt64 frameStart, UInt32 numFrames, UInt32 updateFrequency, IntPtr frameList, IntPtr callback, IntPtr refcon);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int LowLatencyWriteIsochPipeAsyncDelegate(IntPtr self, byte pipeRef, IntPtr buf, UInt64 frameStart, UInt32 numFrames, UInt32 updateFrequency, IntPtr frameList, IntPtr callback, IntPtr refcon);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int LowLatencyCreateBufferDelegate(IntPtr self, IntPtr buffer, int size, UInt32 bufferType);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int LowLatencyDestroyBufferDelegate(IntPtr self, IntPtr buffer );

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int GetBusMicroFrameNumberDelegate(IntPtr self, out UInt64 microFrame, out long atTime);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int GetFrameListTimeDelegate(IntPtr self, out UInt32 microsecondsInFrame);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate int GetIOUSBLibVersionDelegate(IntPtr self, IntPtr ioUSBLibVersion, IntPtr usbFamilyVersion);

	}
}
