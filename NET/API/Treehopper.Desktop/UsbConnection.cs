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

        public UsbConnection(UsbRegistry regDevice)
        {
            this.DevicePath = regDevice.SymbolicName;
            device = regDevice.Device;


            SerialNumber = device.Info.SerialString;
            Name = device.Info.ProductString;
            Version = device.Info.Descriptor.BcdDevice;
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
        private UsbEndpointWriter peripheralConfig;
        private UsbEndpointReader peripheralReceive;
        private UsbRegistry regDevice;

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

        public short Version { get; set; }

        public void Close()
        {
            if (!isOpen)
                return;
            Disconnect();

            isOpen = false;
        }

        public async Task<bool> OpenAsync()
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
                peripheralConfig = device.OpenEndpointWriter(WriteEndpointID.Ep02);
                peripheralReceive = device.OpenEndpointReader(ReadEndpointID.Ep02);

                isOpen = true;

                pinListenerTask = new Task(() =>
                {
                    while (isOpen)
                    {
                        byte[] buffer = new byte[41];
                        int len = 0;
                        try
                        {
                            pinState.Read(buffer, 1000, out len);
                            PinEventDataReceived?.Invoke(buffer);
                        }
                        catch (Exception ex)
                        {

                        }
                        if (updateDelay > 1) Task.Delay(updateDelay).Wait();
                    }

                });

                pinListenerTask.Start();

                return true;

            } else
            {
                return false;
            }
        }



        private Task pinListenerTask;
        public void SendDataPeripheralChannel(byte[] data)
        {
            if (!isOpen)
                return;

            int transferLength;
            ErrorCode error = peripheralConfig.Write(data, 1000, out transferLength);
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
            if (peripheralReceive != null)
                peripheralReceive.Read(returnVal, 1000, out transferLength);
            return returnVal;
        }

        internal void Disconnect()
        {
            if (!isOpen) return; // already disconnected

            isOpen = false;
            pinListenerTask.Wait(); // wait for the task to return
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
                    pinState.Dispose();
                    pinState = null;
                }
                if (peripheralConfig != null)
                {
                    peripheralConfig.Abort();
                    peripheralConfig.Dispose();
                    peripheralConfig = null;
                }
                if (peripheralReceive != null)
                {
                    peripheralReceive.Dispose();
                    peripheralReceive = null;
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

        ~UsbConnection()
        {
            Dispose();
        }
    }
}
