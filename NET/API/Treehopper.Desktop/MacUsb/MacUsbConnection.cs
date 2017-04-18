﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Treehopper.Desktop.MacUsb.IOKit;

namespace Treehopper.Desktop.MacUsb
{
    public class MacUsbConnection : IConnection
    {
        private readonly IOUSBDeviceInterface320 deviceInterface;

        private readonly IntPtr kIOCFPlugInInterfaceID = NativeMethods.CFUUIDGetConstantUUIDWithBytes(IntPtr.Zero, 0xC2,
            0x44, 0xE8, 0x58, 0x10, 0x9C, 0x11, 0xD4, 0x91, 0xD4, 0x00, 0x50, 0xE4, 0xC6, 0x42, 0x6F);

        private readonly CFUUIDBytes kIOUSBDeviceInterfaceID320 = new CFUUIDBytes(0x01, 0xA2, 0xD0, 0xE9, 0x42, 0xF6,
            0x4A, 0x87, 0x8B, 0x8B, 0x77, 0x05, 0x7C, 0x8C, 0xE0, 0xCE);

        private readonly IntPtr kIOUSBDeviceUserClientTypeID = NativeMethods.CFUUIDGetConstantUUIDWithBytes(IntPtr.Zero,
            0x9d, 0xc7, 0xb7, 0x80, 0x9e, 0xc0, 0x11, 0xD4, 0xa5, 0x4f, 0x00, 0x0a, 0x27, 0x05, 0x28, 0x61);

        private readonly CFUUIDBytes kIOUSBInterfaceInterfaceID = new CFUUIDBytes(0x73, 0xc9, 0x7a, 0xe8, 0x9e, 0xf3,
            0x11, 0xD4, 0xb1, 0xd0, 0x00, 0x0a, 0x27, 0x05, 0x28, 0x61);

        private readonly IntPtr kIOUSBInterfaceUserClientTypeID = NativeMethods.CFUUIDGetConstantUUIDWithBytes(
            IntPtr.Zero, 0x2d, 0x97, 0x86, 0xc6, 0x9e, 0xf3, 0x11, 0xD4, 0xad, 0x51, 0x00, 0x0a, 0x27, 0x05, 0x28,
            0x61);

        private IOUSBInterfaceInterface197 interfaceInterface;
        private bool isConnected;

        private Thread pinReportThread;

        public MacUsbConnection(IOObject usbService, string name, string serialNumber)
        {
            if (usbService.Handle == IntPtr.Zero)
                throw new NullReferenceException("USB Service is null");

            var plugInSize = Marshal.SizeOf(typeof(IOCFPlugInInterface));
            var pluginInterfacePtrPtr = IntPtr.Zero;
            var device = IntPtr.Zero;

            var score = 0;

            var kr = NativeMethods.IOCreatePlugInInterfaceForService(usbService.Handle, kIOUSBDeviceUserClientTypeID,
                kIOCFPlugInInterfaceID, out pluginInterfacePtrPtr, out score);
            if (kr != IOKitError.Success)
            {
                Debug.WriteLine("Couldn't create plug-in interface. Error: " + kr);
                return;
            }

            var pluginInterface = IOCFPlugInInterface.GetPlugInInterfaceFromPtrPtr(pluginInterfacePtrPtr);

            var deviceInterfacePtrPtr = IntPtr.Zero;
            kr = pluginInterface.QueryInterface(pluginInterfacePtrPtr, kIOUSBDeviceInterfaceID320,
                out deviceInterfacePtrPtr);
            if (kr != IOKitError.Success)
            {
                Debug.WriteLine("Couldn't query interface. Error: " + kr);
                return;
            }

            var deviceInterfacePtr = new IntPtr(Marshal.ReadInt32(deviceInterfacePtrPtr));
            if (deviceInterfacePtr != IntPtr.Zero)
            {
                deviceInterface =
                    (IOUSBDeviceInterface320) Marshal.PtrToStructure(deviceInterfacePtr,
                        typeof(IOUSBDeviceInterface320));
                deviceInterface.Handle = deviceInterfacePtrPtr;
            }

            Name = name;
            Serial = serialNumber;
        }

        public string DevicePath { get; private set; }

        public string Name { get; }

        public string Serial { get; }

        public int UpdateRate { get; set; }

        public short Version { get; set; }

        public event PinEventData PinEventDataReceived;
        public event PropertyChangedEventHandler PropertyChanged;

        public void Close()
        {
            if (!isConnected) return;

            isConnected = false;

            if (pinReportThread.IsAlive)
                pinReportThread.Join();

            interfaceInterface.USBInterfaceClose(interfaceInterface.Handle);
            deviceInterface.USBDeviceClose(deviceInterface.Handle);
        }

        public void Dispose()
        {
            Close();
        }

        public async Task<bool> OpenAsync()
        {
            deviceInterface.GetDeviceAsyncEventSource(deviceInterface.Handle);
            var kr = deviceInterface.USBDeviceOpen(deviceInterface.Handle);
            if (kr != IOKitError.Success)
            {
                Debug.WriteLine("Couldn’t open device: " + kr);
                return false;
            }

            byte numConfigs = 0;
            deviceInterface.GetNumberOfConfigurations(deviceInterface.Handle, out numConfigs);
            if (numConfigs == 0)
                return false;

            var configDesc = new IOUSBConfigurationDescriptor();
            kr = deviceInterface.GetConfigurationDescriptorPtr(deviceInterface.Handle, 0, out configDesc);
            if (kr != IOKitError.Success)
            {
                Debug.WriteLine("Couldn’t get configuration descriptor for index: " + kr);
                return false;
            }

            kr = deviceInterface.SetConfiguration(deviceInterface.Handle, configDesc.bConfigurationValue);
            if (kr != IOKitError.Success)
            {
                Debug.WriteLine("Couldn’t set configuration to value: " + kr);
                return false;
            }

            // HACK: the LED blinks three times since we reconfigured it. Ugh, we should wait here so we don't miss commands
            await Task.Delay(500).ConfigureAwait(false);

            var interfaceRequest = new IOUSBFindInterfaceRequest();
            var interfaceIteratorPtr = IntPtr.Zero;
            kr = deviceInterface.CreateInterfaceIterator(deviceInterface.Handle, interfaceRequest,
                out interfaceIteratorPtr);

            if (kr != IOKitError.Success)
            {
                Debug.WriteLine("Failed to create interface iterator: " + kr);
                return false;
            }

            var interfaceIterator = new IOIterator(interfaceIteratorPtr);

            var usbInterface = interfaceIterator.Next();
            if (usbInterface == null)
            {
                Debug.WriteLine("Failed to find an interface.");
                return false;
            }

            var pluginInterfacePtrPtr = IntPtr.Zero;
            var score = 0;

            if (NativeMethods.IOCreatePlugInInterfaceForService(usbInterface.Handle, kIOUSBInterfaceUserClientTypeID,
                    kIOCFPlugInInterfaceID, out pluginInterfacePtrPtr, out score) != IOKitError.Success)
            {
                Debug.WriteLine("Failed to create CF Plug-In interface: " + kr);
                return false;
            }

            var pluginInterface = IOCFPlugInInterface.GetPlugInInterfaceFromPtrPtr(pluginInterfacePtrPtr);

            usbInterface.Dispose();
            interfaceIterator.Dispose();


            var interfaceInterfacePtrPtr = IntPtr.Zero;
            if (pluginInterface.QueryInterface(pluginInterfacePtrPtr, kIOUSBInterfaceInterfaceID,
                    out interfaceInterfacePtrPtr) != IOKitError.Success)
            {
                Debug.WriteLine("Could not query plugin interface to retrieve interface interface: " + kr);
                return false;
            }

            var interfaceInterfacePtr = new IntPtr(Marshal.ReadInt32(interfaceInterfacePtrPtr));
            if (interfaceInterfacePtr == IntPtr.Zero)
                return false;

            interfaceInterface =
                (IOUSBInterfaceInterface197) Marshal.PtrToStructure(interfaceInterfacePtr,
                    typeof(IOUSBInterfaceInterface197));
            interfaceInterface.Handle = interfaceInterfacePtrPtr;

            byte intNumber = 0;

            interfaceInterface.GetNumEndpoints(interfaceInterface.Handle, out intNumber);
            interfaceInterface.GetInterfaceClass(interfaceInterface.Handle, out intNumber);

            kr = interfaceInterface.GetInterfaceNumber(interfaceInterface.Handle, out intNumber);
            if (kr != IOKitError.Success)
            {
                Debug.WriteLine("Couldn't get interface number: " + kr);
                return false;
            }

            kr = interfaceInterface.USBInterfaceOpen(interfaceInterface.Handle);

            if (kr != IOKitError.Success)
            {
                Debug.WriteLine("Couldn't open interface: " + kr);
                return false;
            }

            isConnected = true;

            pinReportThread = new Thread(() =>
            {
                var pinReport = new byte[41];
                uint numBytesToRead = 41;
                while (isConnected)
                {
                    var status = interfaceInterface.ReadPipeTO(interfaceInterface.Handle, 1, pinReport,
                        out numBytesToRead, 1000, 1000);
                    if (status == IOKitError.Success)
                    {
                        PinEventDataReceived?.Invoke(pinReport);
                    }
                    else
                    {
                        if (status == IOKitError.NoDevice || status == IOKitError.Aborted) // device was unplugged
                        {
                        }
                        else
                        {
                            Debug.WriteLine("Read from pin report failed: {0}", status);

                            status = interfaceInterface.ClearPipeStallBothEnds(interfaceInterface.Handle, 1);
                            if (status != IOKitError.Success)
                                Debug.WriteLine("Can't clear pipe stall: " + status);
                        }
                    }
                }
            });

            pinReportThread.Name = "PinReportThread";
            pinReportThread.Start();

            return true;
        }

        public async Task<byte[]> ReadPeripheralResponsePacket(uint numBytesToRead)
        {
            var dataToRead = new byte[numBytesToRead];

            var status = interfaceInterface.ReadPipeTO(interfaceInterface.Handle, 2, dataToRead, out numBytesToRead,
                1000, 1000);
            if (status != IOKitError.Success)
            {
                Debug.WriteLine("Write to peripheral endpoint failed: {0}", status);
                interfaceInterface.ClearPipeStallBothEnds(interfaceInterface.Handle, 1);
            }
            return dataToRead;
        }

        public async Task SendDataPeripheralChannel(byte[] data)
        {
            var status = interfaceInterface.WritePipeTO(interfaceInterface.Handle, 4, data, (uint) data.Length, 1000,
                1000);
            if (status != IOKitError.Success)
            {
                Debug.WriteLine("Write to peripheral endpoint failed: {0:x8}", status);
                interfaceInterface.ClearPipeStallBothEnds(interfaceInterface.Handle, 4);
            }
        }

        public async Task SendDataPinConfigChannel(byte[] data)
        {
            var status = interfaceInterface.WritePipeTO(interfaceInterface.Handle, 3, data, (uint) data.Length, 1000,
                1000);
            if (status != IOKitError.Success)
            {
                Debug.WriteLine("Write to peripheral endpoint failed: {0:x8}", status);
                interfaceInterface.ClearPipeStallBothEnds(interfaceInterface.Handle, 3);
            }
        }
    }
}