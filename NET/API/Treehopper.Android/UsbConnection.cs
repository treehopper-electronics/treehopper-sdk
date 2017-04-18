using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Hardware.Usb;

namespace Treehopper.Android
{
    public class UsbConnection : IConnection
    {
        UsbDeviceConnection connection;
        UsbDevice usbDevice;
        UsbEndpoint pinConfigEndpoint;
        UsbEndpoint pinReportEndpoint;
        UsbEndpoint peripheralConfigEndpoint;
        UsbEndpoint peripheralResponseEndpoint;
        UsbManager usbManager;

        public UsbConnection(UsbDevice device, UsbManager manager)
        {
            usbDevice = device;
            usbManager = manager;
        }

        public string DevicePath => usbDevice.Handle.ToString();

        public string Name => usbDevice.ProductName;

        public string Serial => usbDevice.SerialNumber;

        public int UpdateRate { get; set; }

        string IConnection.DevicePath
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public short Version { get; private set; }

        public event PinEventData PinEventDataReceived;
        public event PropertyChangedEventHandler PropertyChanged;

        private bool connected = false;

        public void Close()
        {
            if (!connected)
                return;

            connection.Close();
            connected = false;
        }

        public async Task<bool> OpenAsync()
        {
            if (connected)
                return false;

            usbManager.RequestPermission(usbDevice, PendingIntent.GetBroadcast(ConnectionService.Instance.Context, 0, new Intent(ConnectionService.Instance.ActionUsbPermission), PendingIntentFlags.CancelCurrent));


            UsbInterface intf = usbDevice.GetInterface(0);
            pinReportEndpoint = intf.GetEndpoint(0);
            peripheralResponseEndpoint = intf.GetEndpoint(1);
            pinConfigEndpoint = intf.GetEndpoint(2);
            peripheralConfigEndpoint = intf.GetEndpoint(3);
            connection = usbManager.OpenDevice(usbDevice);
            if (connection != null)
            {
                bool intfClaimed = connection.ClaimInterface(intf, true);
                if (intfClaimed)
                {
                    connected = true;
                    return true;
                }
            }
            return false;
        }

        public Task<byte[]> ReadPeripheralResponsePacket(uint bytesToRead)
        {
            throw new NotImplementedException();
        }

        public async Task SendDataPeripheralChannel(byte[] data)
        {
            if (connection == null)
            {
                connected = false;
                return;
            }

            connection.BulkTransfer(peripheralConfigEndpoint, data, data.Length, 1000);
        }

        public async Task SendDataPinConfigChannel(byte[] data)
        {
            if (connection == null)
            {
                connected = false;
                return;
            }

            connection.BulkTransfer(pinConfigEndpoint, data, data.Length, 1000);
        }

        public void Dispose()
        {
            
        }
    }
}