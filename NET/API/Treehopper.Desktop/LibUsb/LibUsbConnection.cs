using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Desktop.LibUsb
{
    public class LibUsbConnection : IConnection
    {
        private const byte pinReportEndpoint = 0x81;
        private const byte peripheralResponseEndpoint = 0x82;
        private const byte pinConfigEndpoint = 0x01;
        private const byte peripheralConfigEndpoint = 0x02;
        private readonly IntPtr deviceProfile;

        private LibUsbDeviceHandle deviceHandle;

        private bool isOpen;

        private Task pinListenerTask;

        public LibUsbConnection(IntPtr deviceProfile)
        {
            this.deviceProfile = deviceProfile;

            OpenAsync().Wait();
            var sb = new StringBuilder();
            NativeMethods.GetStringDescriptor(deviceHandle, 2, sb, sb.Capacity + 1);
            Name = sb.ToString();
            NativeMethods.GetStringDescriptor(deviceHandle, 3, sb, sb.Capacity + 1);
            Serial = sb.ToString();
            Close();
            DevicePath = deviceProfile.ToString();
        }

        public string DevicePath { get; }

        public string Name { get; }

        public string Serial { get; }

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
            Close();
        }

        public async Task<bool> OpenAsync()
        {
            var handle = new IntPtr();
            NativeMethods.Open(deviceProfile, ref handle);
            deviceHandle = new LibUsbDeviceHandle(handle);
            NativeMethods.ClaimInterface(deviceHandle, 0);

            isOpen = true;

            pinListenerTask = new Task(async () =>
            {
                while (isOpen)
                {
                    var buffer = new byte[41];
                    var len = 0;
                    try
                    {
                        var res = NativeMethods.BulkTransfer(deviceHandle, pinReportEndpoint, buffer, buffer.Length,
                            out len, 1000);

                        if (res == LibUsbError.Success)
                            PinEventDataReceived?.Invoke(buffer);
                        else if (res != LibUsbError.ErrorTimeout)
                            Debug.WriteLine("Pin Data Read Failure: " + res);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Exception: " + ex.Message);
                    }

                    if (1000f / UpdateRate > 1)
                        await Task.Delay((int) Math.Round(1000f / UpdateRate));
                }
            });

            pinListenerTask.Start();

            return true;
        }

        public async Task<byte[]> ReadPeripheralResponsePacket(uint numBytesToRead)
        {
            var data = new byte[numBytesToRead];
            var len = 0;
            NativeMethods.BulkTransfer(deviceHandle, peripheralResponseEndpoint, data, (int) numBytesToRead, out len,
                1000);
            return data;
        }

        public async Task SendDataPeripheralChannel(byte[] data)
        {
            var len = 0;
            NativeMethods.BulkTransfer(deviceHandle, peripheralConfigEndpoint, data, data.Length, out len, 1000);
        }

        public async Task SendDataPinConfigChannel(byte[] data)
        {
            var len = 0;
            NativeMethods.BulkTransfer(deviceHandle, pinConfigEndpoint, data, data.Length, out len, 1000);
        }
    }
}