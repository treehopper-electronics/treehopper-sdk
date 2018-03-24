using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Desktop.LibUsb
{
    internal class LibUsbConnection : IConnection
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
            var desc = new LibUsbDeviceDescriptor();
            NativeMethods.GetDeviceDescriptor(deviceProfile, desc);
            this.deviceProfile = deviceProfile;

            Task.Run(OpenAsync).Wait();
            var sb = new StringBuilder();
            NativeMethods.GetStringDescriptor(deviceHandle, 2, sb, sb.Capacity + 1);
            Name = sb.ToString();
            NativeMethods.GetStringDescriptor(deviceHandle, 3, sb, sb.Capacity + 1);
            Serial = sb.ToString();
            Close();
            var hex = desc.bcdDevice; // 0x0111 = 1.11
            Version = (ushort)((hex & 0x0f) + (10 * ((hex >> 4) & 0x0f)) + (100 * ((hex >> 8) & 0x0f)) + (1000 * ((hex >> 12) & 0x0f))); // convert 0x1234 to 1234
            DevicePath = deviceProfile.ToString();
        }

        public string DevicePath { get; }

        public string Name { get; }

        public string Serial { get; }

        public int UpdateRate { get; set; } = 1000;

        public ushort Version { get; private set; }

        public event PinEventData PinEventDataReceived;
        public event PropertyChangedEventHandler PropertyChanged;

        public void Close()
        {
            if (!isOpen) return; // already closed
            
            deviceHandle.Dispose();
            isOpen = false;
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

                        switch(res)
                        {
                            case LibUsbError.Success:
                                PinEventDataReceived?.Invoke(buffer);
                                break;

                            case LibUsbError.ErrorNoDevice:
                            case LibUsbError.ErrorPipe:
                            case LibUsbError.ErrorOverflow:
                                this.Close();
                                // HACK ALERT: device removal notifications don't seem to work right, so if we get ErrorNoDevice,
                                // request that ConnectionService remove the board that matches our device path.
                                ((LibUsbConnectionService)ConnectionService.Instance).RemoveDevice(this.DevicePath);
                                break;

                            case LibUsbError.ErrorTimeout:
                            case LibUsbError.ErrorIO:
                                break; // normal behavior

                            default:
                                Debug.WriteLine("Pin Data Read Failure: " + res);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Exception: " + ex.Message);
                    }

                    int rate = (int)Math.Round(1000.0 / UpdateRate);

                    if (rate > 1)
                        await Task.Delay(rate).ConfigureAwait(false);
                }
            });

            pinListenerTask.Start();

            return true;
        }

        public async Task<byte[]> ReadPeripheralResponsePacketAsync(uint numBytesToRead)
        {
            if (!isOpen)
                return new byte[numBytesToRead];
            
            var data = new byte[numBytesToRead];
            var len = 0;
            NativeMethods.BulkTransfer(deviceHandle, peripheralResponseEndpoint, data, (int) numBytesToRead, out len,
                1000);
            return data;
        }

        public async Task SendDataPeripheralChannelAsync(byte[] data)
        {
            if (!isOpen)
                return;
            
            var len = 0;
            NativeMethods.BulkTransfer(deviceHandle, peripheralConfigEndpoint, data, data.Length, out len, 1000);
        }

        public async Task SendDataPinConfigChannelAsync(byte[] data)
        {
            if (!isOpen)
                return;
            
            var len = 0;
            NativeMethods.BulkTransfer(deviceHandle, pinConfigEndpoint, data, data.Length, out len, 1000);
        }
    }
}