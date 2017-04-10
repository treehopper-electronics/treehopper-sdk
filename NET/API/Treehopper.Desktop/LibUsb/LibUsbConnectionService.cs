using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Treehopper.Desktop.LibUsb
{
    public class LibUsbConnectionService : ConnectionService
    {
		IntPtr context = new IntPtr();
		public LibUsbConnectionService()
		{
			NativeMethods.Init(ref context);
			Refresh();
		}

		private void Refresh()
		{
			IntPtr deviceProfilePtrPtr = new IntPtr();
			int ret = NativeMethods.GetDeviceList(context, out deviceProfilePtrPtr);
			if (ret > 0 || deviceProfilePtrPtr == IntPtr.Zero)
			{
				for (int i = 0; i < ret; i++)
				{
					// calculate the offset pointer
					IntPtr deviceProfilePtr = Marshal.ReadIntPtr (new IntPtr (deviceProfilePtrPtr.ToInt64 () + i * IntPtr.Size));

					LibUsbDeviceDescriptor desc = new LibUsbDeviceDescriptor();
					NativeMethods.GetDeviceDescriptor(deviceProfilePtr, desc);

					if (desc.idVendor == TreehopperUsb.Settings.Vid && desc.idProduct == TreehopperUsb.Settings.Pid)
					{
						Boards.Add (new TreehopperUsb(new LibUsbConnection (deviceProfilePtr)));
					}
				}
			}
		}

		public override void Dispose()
		{
			base.Dispose();
			NativeMethods.Exit(context);
		}
    }
}
