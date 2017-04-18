using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Treehopper.Desktop.MacUsb.IOKit
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class IOUSBInterfaceInterface197 : IUnknownCGuts
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError AbortPipeDelegate(IntPtr self, byte pipeRef);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError ClearPipeStallBothEndsDelegate(IntPtr self, byte pipeRef);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError ClearPipeStallDelegate(IntPtr self, byte pipeRef);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError ControlRequestAsyncDelegate(IntPtr self, byte pipeRef, IntPtr req, IntPtr callback,
            IntPtr refCon);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError ControlRequestAsyncTODelegate(IntPtr self, byte pipeRef, IntPtr req, IntPtr callback,
            IntPtr refCon);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError ControlRequestDelegate(IntPtr self, byte pipeRef, IntPtr req);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError ControlRequestTODelegate(IntPtr self, byte pipeRef, IntPtr req);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError CreateInterfaceAsyncEventSourceDelegate(IntPtr self, IntPtr source);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError CreateInterfaceAsyncPortDelegate(IntPtr self, IntPtr port);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetAlternateSettingDelegate(IntPtr self, out byte intfAltSetting);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetBandwidthAvailableDelegate(IntPtr self, out uint bandwidth);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetBusFrameNumberDelegate(IntPtr self, out ulong frame, out long atTime);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetBusMicroFrameNumberDelegate(IntPtr self, out ulong microFrame, out long atTime);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetConfigurationValueDelegate(IntPtr self, out byte configVal);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetDeviceDelegate(IntPtr self, IntPtr device);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetDeviceProductDelegate(IntPtr self, out ushort devProduct);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetDeviceReleaseNumberDelegate(IntPtr self, out ushort devRelNum);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetDeviceVendorDelegate(IntPtr self, out ushort devVendor);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetEndpointPropertiesDelegate(IntPtr self, byte alternateSetting,
            byte endpointNumber, byte direction, out byte transferType, out ushort maxPacketSize, out byte interval);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetFrameListTimeDelegate(IntPtr self, out uint microsecondsInFrame);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr GetInterfaceAsyncEventSourceDelegate(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetInterfaceAsyncPortDelegate(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetInterfaceClassDelegate(IntPtr self, out byte intfClass);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetInterfaceNumberDelegate(IntPtr self, out byte intfNumber);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetInterfaceProtocolDelegate(IntPtr self, out byte intfProtocol);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetInterfaceSubClassDelegate(IntPtr self, out byte intfSubClass);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetIOUSBLibVersionDelegate(IntPtr self, IntPtr ioUSBLibVersion,
            IntPtr usbFamilyVersion);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetLocationIDDelegate(IntPtr self, out uint locationID);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetNumEndpointsDelegate(IntPtr self, out byte intfNumEndpoints);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetPipePropertiesDelegate(IntPtr self, byte pipeRef, out byte direction,
            out byte number, out byte transferType, out ushort maxPacketSize, out byte interval);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError GetPipeStatusDelegate(IntPtr self, byte pipeRef);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError LowLatencyCreateBufferDelegate(IntPtr self, IntPtr buffer, int size,
            uint bufferType);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError LowLatencyDestroyBufferDelegate(IntPtr self, IntPtr buffer);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError LowLatencyReadIsochPipeAsyncDelegate(IntPtr self, byte pipeRef, IntPtr buf,
            ulong frameStart, uint numFrames, uint updateFrequency, IntPtr frameList, IntPtr callback, IntPtr refcon);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError LowLatencyWriteIsochPipeAsyncDelegate(IntPtr self, byte pipeRef, IntPtr buf,
            ulong frameStart, uint numFrames, uint updateFrequency, IntPtr frameList, IntPtr callback, IntPtr refcon);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError ReadIsochPipeAsyncDelegate(IntPtr self, byte pipeRef, IntPtr buf, ulong frameStart,
            uint numFrames, IntPtr frameList, IntPtr callback, IntPtr refcon);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError ReadPipeAsyncDelegate(IntPtr self, byte pipeRef, IntPtr buf, uint size,
            IntPtr callback, IntPtr refcon);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError ReadPipeAsyncTODelegate(IntPtr self, byte pipeRef, IntPtr buf, uint size,
            uint noDataTimeout, uint completionTimeout, IntPtr callback, IntPtr refcon);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError ReadPipeDelegate(IntPtr self, byte pipeRef, IntPtr buf, out uint size);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError ReadPipeTODelegate(IntPtr self, byte pipeRef, byte[] buf, out uint size,
            uint noDataTimeout, uint completionTimeout);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError ResetPipeDelegate(IntPtr self, byte pipeRef);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError SetAlternateInterfaceDelegate(IntPtr self, byte alternateSetting);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError SetPipePolicyDelegate(IntPtr self, byte pipeRef, ushort maxPacketSize,
            byte maxInterval);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError USBInterfaceCloseDelegate(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError USBInterfaceGetStringIndexDelegate(IntPtr self, out byte si);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError USBInterfaceOpenDelegate(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError USBInterfaceOpenSeizeDelegate(IntPtr self);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError WriteIsochPipeAsyncDelegate(IntPtr self, byte pipeRef, IntPtr buf, ulong frameStart,
            uint numFrames, IntPtr frameList, IntPtr callback, IntPtr refcon);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError WritePipeAsyncDelegate(IntPtr self, byte pipeRef, IntPtr buf, uint size,
            IntPtr callback, IntPtr refcon);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError WritePipeAsyncTODelegate(IntPtr self, byte pipeRef, IntPtr buf, uint size,
            uint noDataTimeout, uint completionTimeout, IntPtr callback, IntPtr refcon);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError WritePipeDelegate(IntPtr self, byte pipeRef, IntPtr buf, uint size);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IOKitError WritePipeTODelegate(IntPtr self, byte pipeRef, byte[] buf, uint size,
            uint noDataTimeout, uint completionTimeout);

        public AbortPipeDelegate AbortPipe;
        public ClearPipeStallDelegate ClearPipeStall;
        public ClearPipeStallBothEndsDelegate ClearPipeStallBothEnds;
        public ControlRequestDelegate ControlRequest;
        public ControlRequestAsyncDelegate ControlRequestAsync;
        public ControlRequestAsyncTODelegate ControlRequestAsyncTO;
        public ControlRequestTODelegate ControlRequestTO;
        public CreateInterfaceAsyncEventSourceDelegate CreateInterfaceAsyncEventSource;
        public CreateInterfaceAsyncPortDelegate CreateInterfaceAsyncPort;
        public GetAlternateSettingDelegate GetAlternateSetting;
        public GetBandwidthAvailableDelegate GetBandwidthAvailable;
        public GetBusFrameNumberDelegate GetBusFrameNumber;
        public GetBusMicroFrameNumberDelegate GetBusMicroFrameNumber;
        public GetConfigurationValueDelegate GetConfigurationValue;
        public GetDeviceDelegate GetDevice;
        public GetDeviceProductDelegate GetDeviceProduct;
        public GetDeviceReleaseNumberDelegate GetDeviceReleaseNumber;
        public GetDeviceVendorDelegate GetDeviceVendor;
        public GetEndpointPropertiesDelegate GetEndpointProperties;
        public GetFrameListTimeDelegate GetFrameListTime;
        public GetInterfaceAsyncEventSourceDelegate GetInterfaceAsyncEventSource;
        public GetInterfaceAsyncPortDelegate GetInterfaceAsyncPort;
        public GetInterfaceClassDelegate GetInterfaceClass;
        public GetInterfaceNumberDelegate GetInterfaceNumber;
        public GetInterfaceProtocolDelegate GetInterfaceProtocol;
        public GetInterfaceSubClassDelegate GetInterfaceSubClass;
        public GetIOUSBLibVersionDelegate GetIOUSBLibVersion;
        public GetLocationIDDelegate GetLocationID;
        public GetNumEndpointsDelegate GetNumEndpoints;
        public GetPipePropertiesDelegate GetPipeProperties;
        public GetPipeStatusDelegate GetPipeStatus;
        public LowLatencyCreateBufferDelegate LowLatencyCreateBuffer;
        public LowLatencyDestroyBufferDelegate LowLatencyDestroyBuffer;
        public LowLatencyReadIsochPipeAsyncDelegate LowLatencyReadIsochPipeAsync;
        public LowLatencyWriteIsochPipeAsyncDelegate LowLatencyWriteIsochPipeAsync;
        public ReadIsochPipeAsyncDelegate ReadIsochPipeAsync;
        public ReadPipeDelegate ReadPipe;
        public ReadPipeAsyncDelegate ReadPipeAsync;
        public ReadPipeAsyncTODelegate ReadPipeAsyncTO;
        public ReadPipeTODelegate ReadPipeTO;
        public ResetPipeDelegate ResetPipe;
        public SetAlternateInterfaceDelegate SetAlternateInterface;
        public SetPipePolicyDelegate SetPipePolicy;
        public USBInterfaceCloseDelegate USBInterfaceClose;
        public USBInterfaceGetStringIndexDelegate USBInterfaceGetStringIndex;
        public USBInterfaceOpenDelegate USBInterfaceOpen;
        public USBInterfaceOpenSeizeDelegate USBInterfaceOpenSeize;
        public WriteIsochPipeAsyncDelegate WriteIsochPipeAsync;
        public WritePipeDelegate WritePipe;
        public WritePipeAsyncDelegate WritePipeAsync;
        public WritePipeAsyncTODelegate WritePipeAsyncTO;
        public WritePipeTODelegate WritePipeTO;

        public IntPtr Handle { get; set; }
    }
}