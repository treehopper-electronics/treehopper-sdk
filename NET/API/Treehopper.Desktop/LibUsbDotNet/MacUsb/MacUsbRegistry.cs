using System;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace Treehopper.Desktop.LibUsbDotNet
{
	public class MacUsbRegistry : UsbRegistry
	{
		public MacUsbRegistry()
		{
		}

		public override UsbDevice Device
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override Guid[] DeviceInterfaceGuids
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override bool IsAlive
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override bool Open(out UsbDevice usbDevice)
		{
			usbDevice = null;
			MacUsbDevice macUsbDevice;
			bool bSuccess = Open(out macUsbDevice);
			if (bSuccess)
				usbDevice = macUsbDevice;
			return bSuccess;
		}

		public bool Open(out MacUsbDevice usbDevice)
		{
			usbDevice = null;

			if (String.IsNullOrEmpty(SymbolicName)) return false;
			if (MacUsbDevice.Open(SymbolicName, out usbDevice))
			{
				usbDevice.mUsbRegistry = this;
				return true;
			}
			return false;
		}
	}
}

