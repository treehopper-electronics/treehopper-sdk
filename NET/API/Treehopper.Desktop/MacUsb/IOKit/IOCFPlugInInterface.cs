using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Treehopper.Desktop.MacUsb.IOKit
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public class IOCFPlugInInterface : IUnknownCGuts
	{
		public ushort Version;
		public ushort Revision;
		public ProbeDelegate Probe;
		public StartDelegate Start;
		public StopDelegate Stop;

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate ulong ProbeDelegate(IntPtr self, IntPtr propertyTable, IOObject ioService, IntPtr order);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate ulong StartDelegate(IntPtr self, IntPtr propertyTable, IOObject ioService);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate ulong StopDelegate(IntPtr self);

		public static IOCFPlugInInterface GetPlugInInterfaceFromPtrPtr(IntPtr PtrPtr)
		{
			IntPtr pluginInterfacePtr = new IntPtr(Marshal.ReadInt32(PtrPtr));
			IOCFPlugInInterface pluginInterface = (IOCFPlugInInterface)Marshal.PtrToStructure(pluginInterfacePtr, typeof(IOCFPlugInInterface));
			return pluginInterface;
		}
	}
}
