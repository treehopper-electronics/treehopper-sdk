using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Treehopper.Desktop.LibUsb
{
	public class LibUsbConnection : IConnection
	{
		const byte pinReportEndpoint = 0x81;
		const byte peripheralResponseEndpoint = 0x82;
		const byte pinConfigEndpoint = 0x01;
		const byte peripheralConfigEndpoint = 0x02;

		LibUsbDeviceHandle deviceHandle;
		IntPtr deviceProfile;


		public LibUsbConnection(IntPtr deviceProfile)
		{
			this.deviceProfile = deviceProfile;
		}

		public string DevicePath { get; private set; }

		public string Name { get; private set; }

		public string Serial { get; private set; }

		public int UpdateRate { get; set; } = 1000;

		public short Version { get; private set; }

		public event PinEventData PinEventDataReceived;
		public event PropertyChangedEventHandler PropertyChanged;

		public void Close()
		{
			deviceHandle.Dispose();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public async Task<bool> OpenAsync()
		{
			IntPtr handle = new IntPtr();
			NativeMethods.Open(deviceProfile, ref handle);
			this.deviceHandle = new LibUsbDeviceHandle(handle);
			return true;
		}

		public async Task<byte[]> ReadPeripheralResponsePacket(uint numBytesToRead)
		{
			byte[] data = new byte[numBytesToRead];
			int len = 0;
			NativeMethods.BulkTransfer(deviceHandle, pinConfigEndpoint, data, (int)numBytesToRead, out len, 1000);
			return data;
		}

		public async Task SendDataPeripheralChannel(byte[] data)
		{
			int len = 0;
			NativeMethods.BulkTransfer(deviceHandle, peripheralConfigEndpoint, data, data.Length, out len, 1000);
		}

		public async Task SendDataPinConfigChannel(byte[] data)
		{
			int len = 0;
			NativeMethods.BulkTransfer(deviceHandle, pinConfigEndpoint, data, data.Length, out len, 1000);
		}
	}
}
