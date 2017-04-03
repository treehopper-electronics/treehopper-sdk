using System;
using System.Runtime.InteropServices;

namespace Treehopper.Desktop.MacUsb.IOKit
{
	[StructLayout(LayoutKind.Sequential)]
	public class IUnknownCGuts
	{
		public IntPtr reserved;
		public QueryInterfaceDelegate QueryInterface;
		public AddRefDelegate AddRef;
		public ReleaseDelegate Release;

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate ulong AddRefDelegate(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate ulong ReleaseDelegate(IntPtr self);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate ulong QueryInterfaceDelegate(IntPtr self, CFUUIDBytes iid, IntPtr ppv);
	}

	public struct CFUUIDBytes
	{
		byte byte0;
		byte byte1;
		byte byte2;
		byte byte3;
		byte byte4;
		byte byte5;
		byte byte6;
		byte byte7;
		byte byte8;
		byte byte9;
		byte byte10;
		byte byte11;
		byte byte12;
		byte byte13;
		byte byte14;
		byte byte15;
	}
		
}
