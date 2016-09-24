using LibUsbDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibUsbDotNet.Internal;
using System.Runtime.InteropServices;
using LibUsbDotNet.WinUsb.Internal;
using Microsoft.Win32.SafeHandles;
using LibUsbDotNet.Internal.WinUsb;
using LibUsbDotNet.Main;

namespace LibUsbDotNet.WinHid
{
    public class WinHidDevice : UsbDevice, IUsbInterface
    {
        private string mDevicePath;
        private SafeFileHandle mSafeDevHandle;
        internal WinHidDevice(UsbApiBase usbApi,
                              SafeFileHandle usbHandle,
                              string devicePath) 
            : base(usbApi, usbHandle)
        {
            mDevicePath = devicePath;
            mSafeDevHandle = usbHandle;
        }

        public override DriverModeType DriverMode
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool Close()
        {
            throw new NotImplementedException();
        }

        public override bool Open()
        {
            throw new NotImplementedException();
        }

        internal static bool Open(string devicePath, out WinHidDevice usbDevice)
        {
            usbDevice = null;

            SafeFileHandle sfhDev;

            bool bSuccess = WinHidApi.OpenDevice(out sfhDev, devicePath);
            if (bSuccess)
            {
                usbDevice = new WinHidDevice(WinHidApi, sfhDev, devicePath);
            }
            else
                UsbError.Error(ErrorCode.Win32Error, Marshal.GetLastWin32Error(), "Open", typeof(UsbDevice));

            return bSuccess;
        }
    }
}
