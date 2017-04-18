using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Treehopper.Desktop.LibUsb
{
    public class LibUsbConnectionService : ConnectionService
    {
        private readonly IntPtr context;
        private IntPtr callbackHandle;
		public LibUsbConnectionService()
		{
			NativeMethods.Init(out context);

			HotplugCallbackFunction cb = new HotplugCallbackFunction (Callback);

			NativeMethods.HotplugRegisterCallback (context, HotplugEvent.DeviceArrived | HotplugEvent.DeviceLeft, 0, (int)TreehopperUsb.Settings.Vid, (int)TreehopperUsb.Settings.Pid, NativeMethods.HotplugMatchAny, cb, IntPtr.Zero, callbackHandle);
			Refresh();
			Task.Run (async() => {
				while(true)
				{
					NativeMethods.HandleEvents(context, IntPtr.Zero);
					await Task.Delay(100);
				}
			});
		}


		private int Callback(IntPtr context, IntPtr deviceProfile, HotplugEvent e, IntPtr userData)
		{
			Debug.WriteLine (e);
			Task.Run (() => {
				if (e == HotplugEvent.DeviceArrived) {
					var board = new TreehopperUsb (new LibUsbConnection (deviceProfile));
					Debug.WriteLine ("Adding " + board);
					Boards.Add (board);
				} else if(e == HotplugEvent.DeviceLeft) {
					var devicePath = deviceProfile.ToString ();
					var boardToRemove = Boards.FirstOrDefault (b => b.Connection.DevicePath == devicePath);
					if (boardToRemove != null) {
						boardToRemove.Dispose ();
						Boards.Remove (boardToRemove);
					}
				}
			});
			return 0;
		}

		private void Refresh()
		{
			IntPtr deviceProfilePtrPtr;
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
						var board = new TreehopperUsb(new LibUsbConnection(deviceProfilePtr));
						Debug.WriteLine ("Adding " + board);
						Boards.Add (board);
					}
				}
			}
		}

		public override void Dispose()
		{
			NativeMethods.Exit(context);
		}
    }
}
