using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LibUsbDotNet.Main;
using LibUsbDotNet;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Treehopper
{
    public class UsbConnection : IConnection
    {
        private bool isOpen = false;
        public UsbConnection(string devicePath, string name)
        {
            UpdateRate = 1000;
            DevicePath = devicePath;
        }

        UsbDevice device;

        public UsbConnection(UsbDevice device)
        {
            this.device = device;
            string serialNumber = "";
            string deviceName = "";
            device.GetString(out serialNumber, 0x0409, 3);
            if (serialNumber != null)
            {
                serialNumber = Regex.Replace(serialNumber, @"[^\u0000-\u007F]", string.Empty);
                serialNumber = serialNumber.Replace("\0", String.Empty);
                SerialNumber = serialNumber;
            }

            device.GetString(out deviceName, 0x0409, 4);
            if (deviceName != null)
            {
                deviceName = deviceName.Replace("\0", String.Empty);
                Name = deviceName;
            }
                

        }

        byte[] pinEventData = new byte[64];
        byte[] peripheralResponseData = new byte[64];

        public event PinEventData PinEventDataReceived;
        public event PropertyChangedEventHandler PropertyChanged;

        public string SerialNumber { get; set; }

        public string Name { get; set; }

        public string DevicePath { get; set; }

        private int updateRate;
        private int updateDelay;
        private UsbEndpointWriter pinConfig;
        private UsbEndpointReader pinState;
        private UsbEndpointWriter commsConfig;
        private UsbEndpointReader commsReceive;

        public int UpdateRate
        {
            get
            {
                return updateRate;
            }

            set
            {
                if (updateRate == value)
                    return;
                updateRate = value;
                updateDelay = (int)Math.Round(1000.0 / updateRate);
            }
        }

        public void Close()
        {
            if (!isOpen)
                return;
            Disconnect();

            isOpen = false;
        }

        public async Task<bool> Open()
        {
            if (isOpen)
                return true;

            if (device.Open())
            {
                // If this is a "whole" usb device (libusb-win32, linux libusb)
                // it will have an IUsbDevice interface. If not (WinUSB) the 
                // variable will be null indicating this is an interface of a 
                // device.
                IUsbDevice wholeUsbDevice = device as IUsbDevice;
                if (!ReferenceEquals(wholeUsbDevice, null))
                {
                    // This is a "whole" USB device. Before it can be used, 
                    // the desired configuration and interface must be selected.

                    // Select config #1
                    wholeUsbDevice.SetConfiguration(1);

                    // Claim interface #0.
                    wholeUsbDevice.ClaimInterface(0);
                }


                pinConfig = device.OpenEndpointWriter(WriteEndpointID.Ep01);
                pinState = device.OpenEndpointReader(ReadEndpointID.Ep01);
                commsConfig = device.OpenEndpointWriter(WriteEndpointID.Ep02);
                commsReceive = device.OpenEndpointReader(ReadEndpointID.Ep02);

                pinState.DataReceived += pinState_DataReceived;
                pinState.DataReceivedEnabled = true;

                isOpen = true;
                return true;

            } else
            {
                return false;
            }
        }

        private async void pinState_DataReceived(object sender, EndpointDataEventArgs e)
        {
            int transferLength;
            pinState.Read(pinEventData, 1000, out transferLength);
            PinEventDataReceived(pinEventData);
            await Task.Delay(updateDelay);
        }

        public void SendDataPeripheralChannel(byte[] data)
        {
            if (!isOpen)
                return;

            int transferLength;
            ErrorCode error = pinConfig.Write(data, 1000, out transferLength);
            if (error != ErrorCode.None && error != ErrorCode.IoCancelled)
            {
                Debug.WriteLine(error);
            }
        }

        public void SendDataPinConfigChannel(byte[] data)
        {
            if (!isOpen)
                return;
            int transferLength;
            ErrorCode error = pinConfig.Write(data, 1000, out transferLength);
            if (error != ErrorCode.None && error != ErrorCode.IoCancelled)
            {
                Debug.WriteLine(error);
            }
        }

        public async Task<byte[]> ReadPeripheralResponsePacket(uint bytesToRead)
        {
            byte[] retVal = new byte[bytesToRead];
            if (!isOpen)
                return new byte[0];
            byte[] returnVal = new byte[64];
            int transferLength;
            if (commsReceive != null)
                commsReceive.Read(returnVal, 1000, out transferLength);
            return returnVal;
        }

        internal void Disconnect()
        {
            try
            {
                if (pinConfig != null)
                {
                    pinConfig.Abort();
                    pinConfig.Dispose();
                    pinConfig = null;
                }
                if (pinState != null)
                {
                    pinState.DataReceivedEnabled = false;
                    pinState.DataReceived -= pinState_DataReceived; // TODO: sometimes pinState is null when we get here. 
                    pinState.Dispose();
                    pinState = null;
                }
                if (commsConfig != null)
                {
                    commsConfig.Abort();
                    commsConfig.Dispose();
                    commsConfig = null;
                }
                if (commsReceive != null)
                {
                    commsReceive.Dispose();
                    commsReceive = null;
                }
            }
            catch (Exception e)
            {

            }

            if (device != null)
            {
                if (device.IsOpen)
                {
                    IUsbDevice wholeUsbDevice = device as IUsbDevice;
                    if (!ReferenceEquals(wholeUsbDevice, null))
                    {
                        wholeUsbDevice.ReleaseInterface(0);
                    }
                    device.Close();
                }
            }
        }

        public void Dispose()
        {
            Disconnect();
            UsbDevice.Exit();
        }
    }
}
