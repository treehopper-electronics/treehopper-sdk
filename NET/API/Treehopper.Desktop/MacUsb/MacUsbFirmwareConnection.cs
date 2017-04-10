using System;
using System.Threading.Tasks;

namespace Treehopper.Desktop.MacUsb
{
	public class MacUsbFirmwareConnection : IFirmwareConnection
	{
		public MacUsbFirmwareConnection ()
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

