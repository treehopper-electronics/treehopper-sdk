using System;
using System.Threading.Tasks;

namespace Treehopper.Desktop.LibUsb
{
	public class LibUsbFirmwareConnection : IFirmwareConnection
	{
		IntPtr ctx = new IntPtr();

		LibUsbDeviceHandle deviceHandle;

		public LibUsbFirmwareConnection ()
		{
			NativeMethods.Init (out ctx);

		}

		public async Task<bool> OpenAsync ()
		{
			var handle = NativeMethods.Open(ctx, (ushort)TreehopperUsb.Settings.BootloaderVid, (ushort)TreehopperUsb.Settings.BootloaderPid);
			if (handle == IntPtr.Zero)
				return false;

			this.deviceHandle = new LibUsbDeviceHandle(handle);

			var res = NativeMethods.DetachKernelDriver (deviceHandle, 0);

			res = NativeMethods.SetConfiguration (deviceHandle, 1);
			if (res != 0)
				return false;
			
			res = NativeMethods.ClaimInterface (deviceHandle, 0);
			if (res != 0)
				return false;

			res = NativeMethods.SetInterfaceAltSetting (deviceHandle, 0, 0);
			if (res != 0)
				return false;
			
			return true;
		}

		public void Close ()
		{
			this.deviceHandle.Dispose ();
		}

		public async Task<bool> Write (byte[] data)
		{
			int dataWritten = 0;
			var res = NativeMethods.InterruptTransfer (deviceHandle, 0x01, data, data.Length, out dataWritten, 1000);
			if(res == 0)
				return true;
			else
				return false;
		}

		public async Task<byte[]> Read (int numBytes)
		{
			int dataWritten = 0;
			byte[] data = new byte[numBytes];
			NativeMethods.InterruptTransfer (deviceHandle, 0x81, data, data.Length, out dataWritten, 1000);
			return data;
		}
	}
}

