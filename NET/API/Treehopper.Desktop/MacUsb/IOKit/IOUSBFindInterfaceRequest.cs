using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Treehopper.Desktop
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class IOUSBFindInterfaceRequest
    {
        public const ushort kIOUSBFindInterfaceDontCare = 0xFFFF;
        public ushort bAlternateSetting; // requested alt setting
        public ushort bInterfaceClass; // requested class
        public ushort bInterfaceProtocol; // requested protocol
        public ushort bInterfaceSubClass; // requested subclass

        public IOUSBFindInterfaceRequest()
        {
            bInterfaceClass = kIOUSBFindInterfaceDontCare;
            bInterfaceSubClass = kIOUSBFindInterfaceDontCare;
            bInterfaceProtocol = kIOUSBFindInterfaceDontCare;
            bAlternateSetting = kIOUSBFindInterfaceDontCare;
        }
    }
}