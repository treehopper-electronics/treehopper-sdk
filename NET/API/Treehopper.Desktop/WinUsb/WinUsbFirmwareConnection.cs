using System;
using System.Threading.Tasks;

namespace Treehopper.Desktop.WinUsb
{
	public class WinUsbFirmwareConnection : IFirmwareConnection
	{
		public WinUsbFirmwareConnection ()
		{
		}

		public Task<bool> OpenAsync ()
		{
			throw new NotImplementedException ();
		}

		public void Close ()
		{
			throw new NotImplementedException ();
		}

		public Task<bool> Write (byte[] data)
		{
			throw new NotImplementedException ();
		}

		public Task<byte[]> Read (int numBytes)
		{
			throw new NotImplementedException ();
		}
	}
}

