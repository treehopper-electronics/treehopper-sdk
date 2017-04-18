using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Treehopper.Desktop.MacUsb.IOKit
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class IOCFPlugInInterface : IUnknownCGuts
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate ulong ProbeDelegate(IntPtr self, IntPtr propertyTable, IOObject ioService, IntPtr order);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate ulong StartDelegate(IntPtr self, IntPtr propertyTable, IOObject ioService);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate ulong StopDelegate(IntPtr self);

        public ProbeDelegate Probe;
        public ushort Revision;
        public StartDelegate Start;
        public StopDelegate Stop;
        public ushort Version;

        public static IOCFPlugInInterface GetPlugInInterfaceFromPtrPtr(IntPtr PtrPtr)
        {
            var pluginInterfacePtr = new IntPtr(Marshal.ReadInt32(PtrPtr));
            var pluginInterface =
                (IOCFPlugInInterface) Marshal.PtrToStructure(pluginInterfacePtr, typeof(IOCFPlugInInterface));
            return pluginInterface;
        }
    }
}