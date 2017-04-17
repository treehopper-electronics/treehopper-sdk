using System;
using System.Runtime.InteropServices;

namespace Treehopper.Desktop.MacUsb
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class TreehopperData
	{
		public string SerialNumber;
	}
}
