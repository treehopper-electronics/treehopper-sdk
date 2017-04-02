using System;
using System.Collections.Generic;
using System.Diagnostics;
using LibUsbDotNet.Main;
// namespace Treehopper.Desktop.LibUsbDotNet
using MonoMac.IOKit;

namespace LibUsbDotNet.MacUsb
{
	public class MacUsbRegistry : UsbRegistry
	{
		MacUsbDevice usbDevice;

		public MacUsbRegistry()
		{
		}

		private static List<MacUsbRegistry> devices = new List<MacUsbRegistry>();
		public static List<MacUsbRegistry> Devices
		{
			get
			{
				Scan();
				return devices;
			}
		}

		private static void Scan()
		{

			using (var usbDeviceIterator = IOKitFramework.FindUsbDevices())
			{
				if (usbDeviceIterator != null)
				{
					var usbDeviceService = usbDeviceIterator.Next();

					while (usbDeviceService != null)
					{
						using (usbDeviceService)
						{
							var vendorString = usbDeviceService.GetCFPropertyString(IOKitFramework.kUSBVendorString);
							var productString = usbDeviceService.GetCFPropertyString(IOKitFramework.kUSBProductString);
							var productID = usbDeviceService.GetCFPropertyInt(IOKitFramework.kUSBProductID);
							var vendorID = usbDeviceService.GetCFPropertyInt(IOKitFramework.kUSBVendorID);

							if ((productID > 0) && (vendorID > 0))
							{
								Debug.WriteLine("Found Device:");
								Debug.WriteLine("Vendor: " + vendorString);
								Debug.WriteLine("Product: " + productString);
								Debug.WriteLine("Product ID: " + productID);
								Debug.WriteLine("Vendor ID: " + vendorID);
								Debug.WriteLine("");
							}
						}

						usbDeviceService = usbDeviceIterator.Next();
					}


				}
			}
		}

		public MacUsbRegistry(MacUsbDevice usbDevice)
		{
			this.usbDevice = usbDevice;
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

