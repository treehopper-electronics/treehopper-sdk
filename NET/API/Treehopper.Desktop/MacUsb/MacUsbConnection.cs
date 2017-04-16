using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Treehopper.Desktop.MacUsb.IOKit;

namespace Treehopper.Desktop.MacUsb
{
	public class MacUsbConnection : IConnection
	{
		IOUSBDeviceInterface320 deviceInterface;

		IOUSBInterfaceInterface197 interfaceInterface;

		IntPtr kIOCFPlugInInterfaceID 			= NativeMethods.CFUUIDGetConstantUUIDWithBytes(IntPtr.Zero, 0xC2, 0x44, 0xE8, 0x58, 0x10, 0x9C, 0x11, 0xD4, 0x91, 0xD4, 0x00, 0x50, 0xE4, 0xC6, 0x42, 0x6F);
		IntPtr kIOUSBDeviceUserClientTypeID 	= NativeMethods.CFUUIDGetConstantUUIDWithBytes(IntPtr.Zero, 0x9d, 0xc7, 0xb7, 0x80, 0x9e, 0xc0, 0x11, 0xD4, 0xa5, 0x4f, 0x00, 0x0a, 0x27, 0x05, 0x28, 0x61);
		IntPtr kIOUSBInterfaceUserClientTypeID	= NativeMethods.CFUUIDGetConstantUUIDWithBytes(IntPtr.Zero, 0x2d, 0x97, 0x86, 0xc6, 0x9e, 0xf3, 0x11, 0xD4, 0xad, 0x51, 0x00, 0x0a, 0x27, 0x05, 0x28, 0x61);

		CFUUIDBytes kIOUSBDeviceInterfaceID320 	= new CFUUIDBytes(0x01, 0xA2, 0xD0, 0xE9, 0x42, 0xF6, 0x4A, 0x87, 0x8B, 0x8B, 0x77, 0x05, 0x7C, 0x8C, 0xE0, 0xCE);
		CFUUIDBytes kIOUSBInterfaceInterfaceID 	= new CFUUIDBytes(0x73, 0xc9, 0x7a, 0xe8, 0x9e, 0xf3, 0x11, 0xD4, 0xb1, 0xd0, 0x00, 0x0a, 0x27, 0x05, 0x28, 0x61);

		public MacUsbConnection(IOObject usbService, string name, string serialNumber)
		{
			if (usbService.Handle == IntPtr.Zero)
			{
				throw new NullReferenceException("USB Service is null");
			}

			int plugInSize = Marshal.SizeOf(typeof(IOCFPlugInInterface));
			IntPtr pluginInterfacePtrPtr = IntPtr.Zero;
			var device = IntPtr.Zero;

			int score = 0;

			var status = NativeMethods.IOCreatePlugInInterfaceForService(usbService.Handle, kIOUSBDeviceUserClientTypeID, kIOCFPlugInInterfaceID, out pluginInterfacePtrPtr, out score);
			if (status != IOKitFramework.kIOReturnSuccess)
			{
				Debug.WriteLine("Couldn't create plug-in interface. Error = {0}", status);
				return;
			}

			IOCFPlugInInterface pluginInterface = IOCFPlugInInterface.GetPlugInInterfaceFromPtrPtr(pluginInterfacePtrPtr);

			IntPtr deviceInterfacePtrPtr = IntPtr.Zero;
			var kr = pluginInterface.QueryInterface(pluginInterfacePtrPtr, kIOUSBDeviceInterfaceID320, out deviceInterfacePtrPtr);
			if (kr != IOKitFramework.kIOReturnSuccess)
			{
				Debug.WriteLine("Couldn't query interface. Error = {0}", kr);
				return;
			}

			IntPtr deviceInterfacePtr = new IntPtr(Marshal.ReadInt32(deviceInterfacePtrPtr));
			if (deviceInterfacePtr != IntPtr.Zero)
			{
				this.deviceInterface = (IOUSBDeviceInterface320)Marshal.PtrToStructure(deviceInterfacePtr, typeof(IOUSBDeviceInterface320));
				this.deviceInterface.Handle = deviceInterfacePtrPtr;
			}
		}

		public string DevicePath { get; private set; }

		public string Name { get; private set; }

		public string Serial { get; private set; }

		public int UpdateRate { get; set; }

		public short Version { get; set; }

		public event PinEventData PinEventDataReceived;
		public event PropertyChangedEventHandler PropertyChanged;

		public void Close()
		{
			interfaceInterface.USBInterfaceClose(interfaceInterface.Handle);
		}

		public void Dispose()
		{
			
		}

		public async Task<bool> OpenAsync()
		{
			deviceInterface.GetDeviceAsyncEventSource(deviceInterface.Handle);
			var kr = deviceInterface.USBDeviceOpen(deviceInterface.Handle);
			if (kr != IOKitFramework.kIOReturnSuccess)
			{
				Debug.WriteLine("Couldn’t open device (err = {x})", kr);
				return false;
			}

			byte numConfigs = 0;
			deviceInterface.GetNumberOfConfigurations(deviceInterface.Handle, out numConfigs);
			if (numConfigs == 0)
				return false;

			IOUSBConfigurationDescriptor configDesc = new IOUSBConfigurationDescriptor();
			kr = deviceInterface.GetConfigurationDescriptorPtr(deviceInterface.Handle, 0, out configDesc);
			if (kr != IOKitFramework.kIOReturnSuccess)
			{
				Debug.WriteLine("Couldn’t get configuration descriptor for index (err = {x})", kr);
				return false;
			}

			kr = deviceInterface.SetConfiguration(deviceInterface.Handle, configDesc.bConfigurationValue);
			if (kr != IOKitFramework.kIOReturnSuccess)
			{
				Debug.WriteLine("Couldn’t set configuration to value (err = {x})", kr);
				return false;
			}

			IOUSBFindInterfaceRequest interfaceRequest = new IOUSBFindInterfaceRequest();
			IntPtr interfaceIteratorPtr = IntPtr.Zero;
			kr = deviceInterface.CreateInterfaceIterator(deviceInterface.Handle, interfaceRequest, out interfaceIteratorPtr);

			if (kr != IOKitFramework.kIOReturnSuccess)
			{
				Debug.WriteLine("Failed to create interface iterator (err = {x})", kr);
				return false;
			}

			IOIterator interfaceIterator = new IOIterator(interfaceIteratorPtr);

			var usbInterface = interfaceIterator.Next();
			IntPtr pluginInterfacePtrPtr = IntPtr.Zero;
			int score = 0;

			if (NativeMethods.IOCreatePlugInInterfaceForService(usbInterface.Handle, kIOUSBInterfaceUserClientTypeID, kIOCFPlugInInterfaceID, out pluginInterfacePtrPtr, out score) != IOKitFramework.kIOReturnSuccess)
			{
				Debug.WriteLine("Failed to create CF Plug-In interface (err = {x})", kr);
				return false;
			}

			IOCFPlugInInterface pluginInterface = IOCFPlugInInterface.GetPlugInInterfaceFromPtrPtr(pluginInterfacePtrPtr);

			usbInterface.Dispose();
			interfaceIterator.Dispose();


			IntPtr interfaceInterfacePtrPtr = IntPtr.Zero;
			if (pluginInterface.QueryInterface(pluginInterfacePtrPtr, kIOUSBInterfaceInterfaceID, out interfaceInterfacePtrPtr) != IOKitFramework.kIOReturnSuccess)
			{
				Debug.WriteLine("Could not query plugin interface to retrieve interface interface");
				return false;
			}

			IntPtr interfaceInterfacePtr = new IntPtr(Marshal.ReadInt32(interfaceInterfacePtrPtr));
			if (interfaceInterfacePtr == IntPtr.Zero)
			{
				return false;
			}

			this.interfaceInterface = (IOUSBInterfaceInterface197)Marshal.PtrToStructure(interfaceInterfacePtr, typeof(IOUSBInterfaceInterface197));
			this.interfaceInterface.Handle = interfaceInterfacePtrPtr;

			byte intNumber = 0;

			interfaceInterface.GetNumEndpoints(interfaceInterface.Handle, out intNumber);
			interfaceInterface.GetInterfaceClass(interfaceInterface.Handle, out intNumber);

			kr = interfaceInterface.GetInterfaceNumber(interfaceInterface.Handle, out intNumber);
			if (kr != IOKitFramework.kIOReturnSuccess)
			{
				Debug.WriteLine("Couldn't get interface number");
				return false;
			}

			kr = interfaceInterface.USBInterfaceOpen(interfaceInterface.Handle);

			if (kr != IOKitFramework.kIOReturnSuccess)
			{
				Debug.WriteLine("Couldn't open interface");
				return false;
			}
			else
			{
				return true;
			}

			return false;
		}

		public async Task<byte[]> ReadPeripheralResponsePacket(uint numBytesToRead)
		{
			byte[] dataToRead = new byte[numBytesToRead];

			var status = interfaceInterface.ReadPipeTO(interfaceInterface.Handle, 1, dataToRead, out numBytesToRead, 1000, 1000);
			if (status != IOKitFramework.kIOReturnSuccess)
			{
				Debug.WriteLine("Write to peripheral endpoint failed: {0}", status);
				interfaceInterface.ClearPipeStallBothEnds(interfaceInterface.Handle, 1);
			}
			return dataToRead;
		}

		public async Task SendDataPeripheralChannel(byte[] data)
		{
			var status = interfaceInterface.WritePipeTO(interfaceInterface.Handle, 4, data, (uint)data.Length, 1000, 1000);
			if (status != IOKitFramework.kIOReturnSuccess)
			{
				Debug.WriteLine("Write to peripheral endpoint failed: {0:x8}", status);
				interfaceInterface.ClearPipeStallBothEnds(interfaceInterface.Handle, 4);
			}
		}

		public async Task SendDataPinConfigChannel(byte[] data)
		{
			var status = interfaceInterface.WritePipeTO(interfaceInterface.Handle, 3, data, (uint)data.Length, 1000, 1000);
			if (status != IOKitFramework.kIOReturnSuccess)
			{
				Debug.WriteLine("Write to peripheral endpoint failed: {0:x8}", status);
				interfaceInterface.ClearPipeStallBothEnds(interfaceInterface.Handle, 3);
			}			
		}
	}
}
