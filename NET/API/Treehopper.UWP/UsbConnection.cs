using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Usb;
using Windows.Storage.Streams;

namespace Treehopper.Uwp
{
    public class UsbConnection : IConnection, INotifyPropertyChanged
    {
        private CancellationTokenSource cts;

        private UsbBulkInPipe peripheralInPipe;
        private UsbBulkOutPipe peripheralOutPipe;
        private DataReader peripheralReader;
        private DataWriter peripheralWriter;
        private UsbBulkOutPipe pinConfigPipe;
        private DataWriter pinConfigWriter;


        private UsbBulkInPipe pinEventPipe;

        private DataReader pinEventReader;
        private int updateDelay;

        private int updateRate;

        private UsbDevice usbDevice;

        public UsbConnection(DeviceInformation deviceInfo)
        {
            UpdateRate = 1000;
            DevicePath = deviceInfo.Id;
            Serial = DevicePath.Split('#')[2];
            Name = deviceInfo.Name;
        }

        public bool IsOpen { get; private set; }

        public event PinEventData PinEventDataReceived;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Serial { get; }


        public string Name { get; }

        public string DevicePath { get; set; }

        public int UpdateRate
        {
            get { return updateRate; }

            set
            {
                if (updateRate == value)
                    return;
                updateRate = value;
                updateDelay = (int) Math.Round(1000.0 / updateRate);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("UpdateRate"));
            }
        }

        public short Version { get; private set; }

        public async Task<bool> OpenAsync()
        {
            if (IsOpen)
                return true; // we're already open

            // Calling this will "open" the device, blocking any other app from using it.
            // This will return null if another program already has the device open.
            usbDevice = await UsbDevice.FromIdAsync(DevicePath);
            if (usbDevice == null)
                return false;

            cts = new CancellationTokenSource();

            // https://msdn.microsoft.com/en-us/library/windows/hardware/dn303346(v=vs.85).aspx

            Version = (short) usbDevice.DeviceDescriptor.BcdDeviceRevision;

            pinConfigPipe = usbDevice.DefaultInterface.BulkOutPipes[0];
            pinConfigPipe.WriteOptions |= UsbWriteOptions.ShortPacketTerminate;

            peripheralOutPipe = usbDevice.DefaultInterface.BulkOutPipes[1];
            peripheralOutPipe.WriteOptions |= UsbWriteOptions.ShortPacketTerminate;

            pinConfigWriter = new DataWriter(pinConfigPipe.OutputStream);
            peripheralWriter = new DataWriter(peripheralOutPipe.OutputStream);

            pinEventPipe = usbDevice.DefaultInterface.BulkInPipes[0];
            pinEventReader = new DataReader(pinEventPipe.InputStream);

            peripheralInPipe = usbDevice.DefaultInterface.BulkInPipes[1];
            peripheralReader = new DataReader(peripheralInPipe.InputStream);

            IsOpen = true;
            pinEventListerner(cts.Token);

            Debug.WriteLine("Connection opened");

            return true;
        }

        public void Close()
        {
            if (!IsOpen)
                return;
            cts.Cancel();
            usbDevice.Dispose();
            usbDevice = null;
            Debug.WriteLine("Connection closed");
            IsOpen = false;
        }

        public async Task SendDataPinConfigChannel(byte[] data)
        {
            if (!IsOpen)
                return;
            try
            {
                pinConfigWriter.WriteBytes(data);
                await pinConfigWriter.StoreAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public async Task SendDataPeripheralChannel(byte[] data)
        {
            if (!IsOpen)
                return;
            try
            {
                peripheralWriter.WriteBytes(data);
                await peripheralWriter.StoreAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public async Task<byte[]> ReadPeripheralResponsePacket(uint bytesToRead)
        {
            if (!IsOpen)
                return new byte[0];
            uint bytesRead;
            try
            {
                bytesRead = await peripheralReader.LoadAsync(bytesToRead);

                if (bytesRead == bytesToRead)
                {
                    var data = new byte[bytesToRead];
                    peripheralReader.ReadBytes(data);
                    return data;
                }
                return new byte[0];
            }
            catch
            {
                return new byte[0];
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private async void pinEventListerner(CancellationToken ct)
        {
            if (!IsOpen)
                return;
            uint bytesRead = 0;
            while (true)
            {
                if (ct.IsCancellationRequested)
                    return;
                try
                {
                    bytesRead = await pinEventReader.LoadAsync(64);

                    if (bytesRead == 64)
                    {
                        var data = new byte[64];
                        pinEventReader.ReadBytes(data);
                        if (PinEventDataReceived != null)
                        {
                            PinEventDataReceived(data);
                            if (updateDelay >= 10)
                                await Task.Delay(updateDelay);
                        }
                    }
                }
                catch
                {
                    return;
                }
            }
        }

        ~UsbConnection()
        {
            Close();
        }
    }
}