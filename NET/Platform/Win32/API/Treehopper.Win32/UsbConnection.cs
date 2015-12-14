using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper
{
    public class UsbConnection : IConnection
    {
        SafeFileHandle fileHandle;
        WinUsbCommunications winUsb = new WinUsbCommunications();
        WinUsbCommunications.SafeWinUsbHandle winUsbHandle;
        WinUsbCommunications.DeviceInfo deviceInfo = new WinUsbCommunications.DeviceInfo();

        public UsbConnection(string id)
        {
            DevicePath = id;
            fileHandle = winUsb.GetDeviceHandle(DevicePath);
            if(!fileHandle.IsInvalid)
            {
                winUsb.InitializeDevice(fileHandle, 500);

            } else {
                winUsb.CloseDeviceHandle(fileHandle);
            }
            SerialNumber = winUsb.GetStringDescriptor(3);
            Name = winUsb.GetStringDescriptor(4);
        }

        public event PinEventData PinEventDataReceived;
        public event PropertyChangedEventHandler PropertyChanged;

        public string SerialNumber { get; set; }

        public string Name { get; set; }

        public string DevicePath { get; set; }

        public void Close()
        {
            
        }

        public bool Open()
        {
            return true;
        }

        public void SendDataPeripheralChannel(byte[] data)
        {

        }

        public void SendDataPinConfigChannel(byte[] data)
        {

        }
    }
}
