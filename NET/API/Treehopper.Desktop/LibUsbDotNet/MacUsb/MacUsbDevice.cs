using System;
using System.Runtime.InteropServices;
using LibUsbDotNet;
using LibUsbDotNet.Internal;
using Microsoft.Win32.SafeHandles;

namespace Treehopper.Desktop.LibUsbDotNet
{
	public class MacUsbDevice : UsbDevice, IUsbInterface
	{
		string mDevicePath;

		SafeHandle mSafeDevHandle;


		internal MacUsbDevice(UsbApiBase usbApi,
							 SafeHandle handle,
							 string devicePath)
            : base(usbApi, handle)
        {
			mDevicePath = devicePath;
			mSafeDevHandle = handle;
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

		public static bool Open(string devicePath, out MacUsbDevice usbDevice)
		{
			throw new NotImplementedException();
			//usbDevice = null;

			//SafeFileHandle sfhDev;

			//bool bSuccess = WinUsbAPI.OpenDevice(out sfhDev, devicePath);
			//if (bSuccess)
			//{
			//	SafeWinUsbInterfaceHandle handle = new SafeWinUsbInterfaceHandle();
			//	bSuccess = WinUsbAPI.WinUsb_Initialize(sfhDev, ref handle);
			//	if (bSuccess)
			//	{
			//		usbDevice = new WinUsbDevice(WinUsbApi, sfhDev, handle, devicePath);
			//	}
			//	else
			//		UsbError.Error(ErrorCode.Win32Error, Marshal.GetLastWin32Error(), "Open:Initialize", typeof(UsbDevice));
			//}
			//else
			//	UsbError.Error(ErrorCode.Win32Error, Marshal.GetLastWin32Error(), "Open", typeof(UsbDevice));


			//return bSuccess;
		}
	}
}

