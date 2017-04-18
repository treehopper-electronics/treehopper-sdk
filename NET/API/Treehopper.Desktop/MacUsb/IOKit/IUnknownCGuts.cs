using System;
using System.Runtime.InteropServices;

namespace Treehopper.Desktop.MacUsb.IOKit
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate ulong AddRefDelegate(IntPtr self);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate ulong ReleaseDelegate(IntPtr self);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate IOKitError QueryInterfaceDelegate(IntPtr self, CFUUIDBytes iid, out IntPtr ppv);

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class IUnknownCGuts
	{
		public IntPtr reserved;
		public QueryInterfaceDelegate QueryInterface;
		public AddRefDelegate AddRef;
		public ReleaseDelegate Release;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct CFUUIDBytes
	{
	    readonly byte byte0;
	    readonly byte byte1;
	    readonly byte byte2;
	    readonly byte byte3;
	    readonly byte byte4;
	    readonly byte byte5;
	    readonly byte byte6;
	    readonly byte byte7;
	    readonly byte byte8;
	    readonly byte byte9;
	    readonly byte byte10;
	    readonly byte byte11;
	    readonly byte byte12;
	    readonly byte byte13;
	    readonly byte byte14;
	    readonly byte byte15;

		public CFUUIDBytes(byte byte0,
							byte byte1,
							byte byte2,
							byte byte3,
							byte byte4,
							byte byte5,
							byte byte6,
							byte byte7,
							byte byte8,
							byte byte9,
							byte byte10,
							byte byte11,
							byte byte12,
							byte byte13,
							byte byte14,
						   byte byte15)
		{
			this.byte0 = byte0;
			this.byte1 = byte1;
			this.byte2 = byte2;
			this.byte3 = byte3;
			this.byte4 = byte4;
			this.byte5 = byte5;
			this.byte6 = byte6;
			this.byte7 = byte7;
			this.byte8 = byte8;
			this.byte9 = byte9;
			this.byte10 = byte10;
			this.byte11 = byte11;
			this.byte12 = byte12;
			this.byte13 = byte13;
			this.byte14 = byte14;
			this.byte15 = byte15;
		}
	}
		
}
