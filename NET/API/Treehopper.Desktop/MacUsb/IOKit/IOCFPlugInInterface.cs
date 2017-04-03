using System;
using System.Runtime.InteropServices;

namespace Treehopper.Desktop.MacUsb.IOKit
{
	[StructLayout(LayoutKind.Sequential)]
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
	}
}
