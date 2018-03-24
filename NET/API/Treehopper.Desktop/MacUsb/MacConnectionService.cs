using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Treehopper.Desktop.MacUsb.IOKit;
using System.Threading;

/// <summary>
/// IOKit-based Treehopper library for macOS support
/// </summary>
namespace Treehopper.Desktop.MacUsb
{
    internal delegate int DeviceAdded(IOObject usbDevice, string name, string serialNumber);
    internal delegate void DeviceRemoved(int usbDevice);

    /// <summary>
    /// 
    /// </summary>
	internal class MacUsbConnectionService : ConnectionService
	{
	    readonly Thread listener;

        DeviceWatcher watcher;
		public MacUsbConnectionService()
		{
            
            watcher = new DeviceWatcher(this.DeviceAdded, this.DeviceRemoved);

            listener = new Thread(new ThreadStart(watcher.RunScanner));
            listener.Name = "Device watcher thread";
            listener.Start();
		}

		public int DeviceAdded(IOObject usbDevice, string name, string serialNumber)
        {
			var board = new TreehopperUsb(new MacUsbConnection(usbDevice, name, serialNumber));

			Boards.Add(board);
			return Boards.IndexOf(board);
        }

		public void DeviceRemoved(int usbDevice)
        {
            Debug.WriteLine("DeviceRemoved() called");

            Boards[usbDevice].Disconnect();
            Boards.RemoveAt(usbDevice);
        }


        public override void Dispose()
        {
            watcher.StopScanner();
            listener.Abort();
        }

        private void addDeviceToCollection(TreehopperUsb board)
        {

        }
	}
}
