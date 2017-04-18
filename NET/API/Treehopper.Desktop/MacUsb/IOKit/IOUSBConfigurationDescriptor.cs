using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Treehopper.Desktop
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class IOUSBConfigurationDescriptor
    {
        public byte bConfigurationValue;
        public byte bDescriptorType;
        public byte bLength;
        public byte bmAttributes;
        public byte bNumInterfaces;
        public byte iConfiguration;
        public byte MaxPower;
        public ushort wTotalLength;
    }
}