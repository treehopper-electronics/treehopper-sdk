using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Usb;
using Windows.Foundation;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Globalization;
using System.ComponentModel;
using System.Threading;

namespace Treehopper
{


    public class UsbConnection : IConnection
    {

        UsbDevice usbDevice;

        public UsbConnection(UsbDevice usbDevice, string id)
        {
            DevicePath = id;
            this.usbDevice = usbDevice;
            GetStrings();
        }
        bool isOpen;
        public bool IsOpen { get { return isOpen; } }

        private async void GetStrings()
        {
            var buffer = new Windows.Storage.Streams.Buffer(64);
            int length;
            IBuffer responseBuffer;

            UsbSetupPacket request = new UsbSetupPacket();
            request.RequestType.Direction = UsbTransferDirection.In;
            request.RequestType.Recipient = UsbControlRecipient.Device;
            request.RequestType.ControlTransferType = UsbControlTransferType.Standard;

            request.Index = 0;
            request.Request = 6;
            request.Length = 64;

            // serial number
            request.Value = 0x0303;
            responseBuffer = await usbDevice.SendControlInTransferAsync(request, buffer);
            length = responseBuffer.GetByte(0);
            try
            {
                serialNumber = System.Text.Encoding.Unicode.GetString(responseBuffer.ToArray(2, length - 2), 0, length - 2);
            }
            catch { }
            // device name
            request.Value = 0x0304;
            responseBuffer = await usbDevice.SendControlInTransferAsync(request, buffer);
            length = responseBuffer.GetByte(0);
            try
            {
                name = System.Text.Encoding.Unicode.GetString(responseBuffer.ToArray(2, length - 2), 0, length - 2);
            } catch { }
            
        }

        public event PinEventData PinEventDataReceived;

        public event PropertyChangedEventHandler PropertyChanged;

        private string serialNumber;
        public string SerialNumber
        {
            get
            {
                return serialNumber;
            }
        }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
        }

        UsbBulkInPipe pinEventPipe;
        UsbBulkInPipe peripheralInPipe;
        UsbBulkOutPipe pinConfigPipe;
        UsbBulkOutPipe peripheralOutPipe;

        DataReader pinEventReader;
        DataWriter pinConfigWriter;
        DataReader peripheralReader;
        DataWriter peripheralWriter;

        async void pinEventListerner(CancellationToken ct)
        {
            UInt32 bytesRead = 0;
            while (true)
            {
                if (ct.IsCancellationRequested)
                    return;
                try {
                    bytesRead = await pinEventReader.LoadAsync(64);

                    if (bytesRead == 64)
                    {
                        byte[] data = new byte[64];
                        pinEventReader.ReadBytes(data);
                        if (this.PinEventDataReceived != null)
                        {
                            PinEventDataReceived(data);
                        }
                    }
                } catch {
                    return;
                }
            }
        }
        CancellationTokenSource cts;
        Task pinEventListenerTask;
        public string DevicePath { get; set; }

        public bool Open()
        {
            if (IsOpen)
                return false;
            cts = new CancellationTokenSource();
            // https://msdn.microsoft.com/en-us/library/windows/hardware/dn303346(v=vs.85).aspx


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

            pinEventListerner(cts.Token);

            Debug.WriteLine("Connection opened");
            isOpen = true;
            return true;
        }

        public void Close()
        {
            if (!isOpen)
                return;
            cts.Cancel();
            Debug.WriteLine("Connection closed");
            isOpen = false;
        }

        async public void SendDataPinConfigChannel(byte[] data)
        {
            if (!IsOpen)
                return;
            try {
                pinConfigWriter.WriteBytes(data);
                await pinConfigWriter.StoreAsync();
            }catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        async public void SendDataPeripheralChannel(byte[] data)
        {
            if (!IsOpen)
                return;
            try
            {
                peripheralWriter.WriteBytes(data);
                await peripheralWriter.StoreAsync();
            } catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public async Task<byte[]> ReadPeripheralResponsePacket(uint bytesToRead)
        {
            uint bytesRead;
            try
            {
                bytesRead = await peripheralReader.LoadAsync(bytesToRead);

                if (bytesRead == bytesToRead)
                {
                    byte[] data = new byte[bytesToRead];
                    peripheralReader.ReadBytes(data);
                    return data;
                } else
                {
                    return new byte[0];
                }
            }
            catch
            {
                return new byte[0];
            }
        }

        ~UsbConnection()
        {
            Close();
        }
    }
}

