using System;

namespace Treehopper.Desktop.LibUsb
{
    [Flags]
    internal enum HotplugEvent
    {
        DeviceArrived = 0x01,
        DeviceLeft = 0x02
    }
}