using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Desktop.WinUsb;
using Treehopper.Desktop.LibUsb;
using Treehopper.Desktop.MacUsb;

namespace Treehopper.Desktop
{
    /// <summary>
    /// This platform-specific class is used for discovering Treehopper boards booted into firmware upload mode.
    /// </summary>
    public class FirmwareConnection
    {
		private static Lazy<WinUsbFirmwareConnection> winUsbInstance = new Lazy<WinUsbFirmwareConnection>();
		private static Lazy<LibUsbFirmwareConnection> libUsbInstance = new Lazy<LibUsbFirmwareConnection>();
		private static Lazy<MacUsbFirmwareConnection> macUsbInstance = new Lazy<MacUsbFirmwareConnection>();

		public static IFirmwareConnection Instance
		{
			get {
				if(ConnectionService.IsWindows)
					return winUsbInstance.Value;

				if (ConnectionService.IsMac)
					return macUsbInstance.Value;

				if (ConnectionService.IsLinux)
					return libUsbInstance.Value;

				throw new Exception("Unsupported operating system");
			}
		}
			
    }
}
