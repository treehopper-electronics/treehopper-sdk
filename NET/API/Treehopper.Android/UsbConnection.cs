using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Java.Lang;

/// <summary>
/// Xamarin-based implementation for running Treehopper on Android
/// </summary>
namespace Treehopper.Android
{
    /// <summary>
    /// UsbConnection implementation for Xamarin.Android
    /// </summary>
    public class UsbConnection : IConnection
    {
        UsbDeviceConnection connection;
        readonly UsbManager usbManager;
        readonly UsbDevice usbDevice;

        UsbEndpoint pinConfigEndpoint;
        UsbEndpoint pinReportEndpoint;
        UsbEndpoint peripheralConfigEndpoint;
        UsbEndpoint peripheralResponseEndpoint;

        Thread pinListenerThread;
        bool pinListenerThreadRunning = false;

        public UsbConnection(UsbDevice device, Context appContext) : this(device, (UsbManager) appContext.GetSystemService("usb"))
        {

        }

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

            pinListenerThreadRunning = false;
            pinListenerThread.Join();

            connection.Close();
            connected = false;
        }

        public async Task<bool> OpenAsync()
        {
            if (connected)
                return false;

            //usbManager.RequestPermission(usbDevice, PendingIntent.GetBroadcast(ConnectionService.Instance.Context, 0, new Intent(ConnectionService.Instance.ActionUsbPermission), PendingIntentFlags.CancelCurrent));
            
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

                    pinListenerThread = new Thread(
                        () =>
                        {
                            byte[] data = new byte[64];
                            pinListenerThreadRunning = true;
                            while (pinListenerThreadRunning)
                            {
                                int res =
                                    connection.BulkTransfer(pinReportEndpoint, data, 41,
                                        100); // pin reports are 41 bytes long now
                                if (res > 0)
                                    this.PinEventDataReceived?.Invoke(data);
                            }
                        });

                    pinListenerThread.Start();
                    return true;

                }
            }
            return false;
        }

        public async Task<byte[]> ReadPeripheralResponsePacketAsync(uint bytesToRead)
        {
            byte[] data = new byte[bytesToRead];
            int res = connection.BulkTransfer(peripheralResponseEndpoint, data, (int)bytesToRead, 1000);
            return data;
        }

        public async Task SendDataPeripheralChannelAsync(byte[] data)
        {
            if (connection == null)
            {
                connected = false;
                return;
            }

            connection.BulkTransfer(peripheralConfigEndpoint, data, data.Length, 1000);
        }

        public async Task SendDataPinConfigChannelAsync(byte[] data)
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