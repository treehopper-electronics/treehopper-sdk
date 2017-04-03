using System;
using System.Diagnostics;
using Treehopper.Desktop.MacUsb.IOKit;

namespace Treehopper.Desktop.MacUsb
{
	public class MacUsbConnectionService : ConnectionService
	{
		public MacUsbConnectionService()
		{
			Scan();
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

							var productID = usbDeviceService.GetCFPropertyInt(IOKitFramework.kUSBProductID);
							var vendorID = usbDeviceService.GetCFPropertyInt(IOKitFramework.kUSBVendorID);

							if ((productID == TreehopperUsb.Settings.Pid) && (vendorID == TreehopperUsb.Settings.Vid))
							{
								var vendorString = usbDeviceService.GetCFPropertyString(IOKitFramework.kUSBVendorString);
								var productString = usbDeviceService.GetCFPropertyString(IOKitFramework.kUSBProductString);
								var serialNumber = usbDeviceService.GetCFPropertyString(IOKitFramework.kUSBSerialNumberString);

								Debug.WriteLine("Found Device:");
								Debug.WriteLine("Vendor: " + vendorString);
								Debug.WriteLine("Product: " + productString);
								Debug.WriteLine("Product ID: " + productID);
								Debug.WriteLine("Vendor ID: " + vendorID);
								Debug.WriteLine("Serial Number: " + serialNumber);
								Debug.WriteLine("");
								IOKitFramework.GetUsbDevice(usbDeviceService);
							}
						}

						usbDeviceService = usbDeviceIterator.Next();
					}
				}
			}
		}
	}
}
