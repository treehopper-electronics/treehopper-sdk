using System.Runtime.InteropServices;

namespace Treehopper.Desktop.MacUsb
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal class TreehopperData
	{
		public string SerialNumber;
	}
}
