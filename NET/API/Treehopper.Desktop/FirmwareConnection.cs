using LibUsbDotNet;
using LibUsbDotNet.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;

namespace Treehopper
{
    public class FirmwareConnection : IFirmwareConnection
    {
        UsbDevice dev;
        private UsbEndpointReader reader;
        private UsbEndpointWriter writer;

        public FirmwareConnection()
        {

        }

        public void Close()
        {
            dev.Close();
        }

        public async Task<bool> OpenAsync()
        {
            dev = UsbDevice.OpenUsbDevice(new UsbDeviceFinder(0x10C4, 0xEAC9));
            if (dev == null)
            {
                dev = UsbDevice.OpenUsbDevice(new UsbDeviceFinder(0x10C4, 0xEACA));
                if (dev == null)
                    return false;
            }

            IUsbDevice wholeUsbDevice = dev as IUsbDevice;
            if (!ReferenceEquals(wholeUsbDevice, null))
            {
                // This is a "whole" USB device. Before it can be used, 
                // the desired configuration and interface must be selected.

                // Select config #1
                wholeUsbDevice.SetConfiguration(1);

                // Claim interface #0.
                wholeUsbDevice.ClaimInterface(0);
            }


            writer = dev.OpenEndpointWriter(WriteEndpointID.Ep01, EndpointType.Interrupt);
            reader = dev.OpenEndpointReader(ReadEndpointID.Ep01, 64, EndpointType.Interrupt);

            return true;
        }

        public async Task<byte[]> Read(int numBytes)
        {
            var retVal = new byte[numBytes];
            if (ConnectionService.IsWindows) // Windows HID API uses the first byte as the HID report.
                retVal = new byte[numBytes+1];

            int actaulBytesRead = 0;
            reader.Read(retVal, 1000, out actaulBytesRead);
            if (ConnectionService.IsWindows) // Windows HID API uses the first byte as the HID report.
            {
                var temp = new byte[numBytes];
                Array.Copy(retVal, 1, temp, 0, numBytes);
                retVal = temp;
            }
            return retVal;
        }

        public async Task<bool> Write(byte[] data)
        {
            int transferedBytes = 0;

            if (ConnectionService.IsWindows) // Windows HID API uses the first byte as the HID report.
                data = new byte[] { 0x00 }.Concat(data).ToArray();

            var result = writer.Write(data, 1000, out transferedBytes);
            if (transferedBytes == data.Length)
                return true;
            return false;
        }
    }
}
