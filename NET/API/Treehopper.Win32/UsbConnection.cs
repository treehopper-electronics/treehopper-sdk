using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Treehopper
{
    public class UsbConnection : IConnection
    {
        WinUsbCommunications winUsb = new WinUsbCommunications();

        private bool isOpen = false;
        public UsbConnection(string devicePath, string name)
        {
            DevicePath = devicePath;
            SerialNumber = devicePath.Split('#')[2];
            Name = name;
        }

        byte[] pinEventData = new byte[64];
        byte[] peripheralResponseData = new byte[64];

        async Task pinEventListerner(CancellationToken ct)
        {
            if (!isOpen)
                return;
            uint bytesRead = 0;
            while (true)
            {
                if (ct.IsCancellationRequested)
                    return;
                try
                {
                    bool success = false;
                    winUsb.ReceiveDataViaBulkTransfer(0, 64, ref pinEventData, ref bytesRead, ref success);

                    if (bytesRead == 64)
                    {
                        if (this.PinEventDataReceived != null)
                        {
                            PinEventDataReceived(pinEventData);
                        }
                    }
                }
                catch
                {
                    return;
                }
                await Task.Delay(1);
            }
        }
        CancellationTokenSource cts;

        public event PinEventData PinEventDataReceived;
        public event PropertyChangedEventHandler PropertyChanged;

        public string SerialNumber { get; set; }

        public string Name { get; set; }

        public string DevicePath { get; set; }

        public void Close()
        {
            if (!isOpen)
                return;
            cts.Cancel();
            winUsb.Close();
            isOpen = false;
        }

        public async Task<bool> Open()
        {
            if (isOpen)
                return true;
            winUsb.Open(DevicePath, 500);
            isOpen = true;
            cts = new CancellationTokenSource();
            pinEventListerner(cts.Token);
            return true;
        }

        public void SendDataPeripheralChannel(byte[] data)
        {
            if (!isOpen)
                return;
            uint bytesWritten = 0;
            bool success = false;
            winUsb.SendDataViaBulkTransfer(1, (uint)data.Length, data, ref bytesWritten, ref success);
            if(!success)
              Close();
        }

        public void SendDataPinConfigChannel(byte[] data)
        {
            if (!isOpen)
                return;
            uint bytesWritten = 0;
            bool success = false;
            winUsb.SendDataViaBulkTransfer(0, (uint)data.Length, data, ref bytesWritten, ref success);
        }

        public async Task<byte[]> ReadPeripheralResponsePacket(uint bytesToRead)
        {
            byte[] retVal = new byte[bytesToRead];
            if (!isOpen)
                return new byte[0];
            uint bytesRead = 0;
            bool success = false;
            winUsb.ReceiveDataViaBulkTransfer(1, bytesToRead, ref retVal, ref bytesRead, ref success);
            return retVal;
        }
    }
}
