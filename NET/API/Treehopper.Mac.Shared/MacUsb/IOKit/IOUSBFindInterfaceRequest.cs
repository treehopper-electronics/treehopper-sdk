using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Treehopper.Desktop.MacUsb.IOKit
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class IOUSBFindInterfaceRequest
	{
		public ushort bInterfaceClass;     // requested class
		public ushort bInterfaceSubClass;      // requested subclass
		public ushort bInterfaceProtocol;      // requested protocol
		public ushort bAlternateSetting;       // requested alt setting

		public const ushort kIOUSBFindInterfaceDontCare = 0xFFFF;

		public IOUSBFindInterfaceRequest()
		{
			bInterfaceClass = kIOUSBFindInterfaceDontCare;
			bInterfaceSubClass = kIOUSBFindInterfaceDontCare;
			bInterfaceProtocol = kIOUSBFindInterfaceDontCare;
			bAlternateSetting = kIOUSBFindInterfaceDontCare;
		}
	}
}
