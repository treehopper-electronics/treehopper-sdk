using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Treehopper.Desktop.MacUsb.IOKit;

namespace Treehopper.Desktop.MacUsb.IOKit
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void IOServiceMatchingCallback(IntPtr refCon, IntPtr iterator);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void IOServiceInterestCallback(int refCon, IntPtr service, uint messageType, IntPtr messageArgument);

	public static class IOKitFramework
	{
		// Service Matching That is the 'IOProviderClass'.
		public const string kIOSerialBSDServiceValue = "IOSerialBSDClient";


		public const string kIOUSBInterfaceClassName = "IOUSBInterface";
		public const string kIOGeneralInterest = "IOGeneralInterest";
		public const string kIOFirstMatchNotification = "IOServiceFirstMatch";

		// Matching keys.
		public const string kIOSerialBSDTypeKey = "IOSerialBSDClientType";

		// Currently possible kIOSerialBSDTypeKey values. 
		public const string kIOSerialBSDAllTypes = "IOSerialStream";
		public const string kIOSerialBSDModemType = "IOModemSerialStream";
		public const string kIOSerialBSDRS232Type = "IORS232SerialStream";


		// Properties that resolve to a /dev device node to open for a particular service
		public const string kIOTTYDeviceKey = "IOTTYDevice";
		public const string kIOTTYBaseNameKey = "IOTTYBaseName";
		public const string kIOTTYSuffixKey = "IOTTYSuffix";
		public const string kIOCalloutDeviceKey = "IOCalloutDevice";
		public const string kIODialinDeviceKey = "IODialinDevice";

		//  USB device properties.
		public const string kUSBDeviceClass = "bDeviceClass";
		public const string kUSBDeviceSubClass = "bDeviceSubClass";
		public const string kUSBDeviceProtocol = "bDeviceProtocol";
		public const string kUSBDeviceMaxPacketSize = "bMaxPacketSize0";
		public const string kUSBCompatibilityMatch = "USBCompatibilityMatch";
		public const string kUSBVendorID = "idVendor";         // good name
		public const string kUSBVendorName = kUSBVendorID;       // bad name - keep for backward compatibility
		public const string kUSBProductID = "idProduct";         // good name
		public const string kUSBProductName = kUSBProductID;      // bad name - keep for backward compatibility
		public const string kUSBDeviceReleaseNumber = "bcdDevice";
		public const string kUSBSpecReleaseNumber = "bcdUSB";
		public const string kUSBManufacturerStringIndex = "iManufacturer";
		public const string kUSBProductStringIndex = "iProduct";
		public const string kUSBSerialNumberStringIndex = "iSerialNumber";
		public const string kUSBDeviceNumConfigs = "bNumConfigurations";
		public const string kUSBInterfaceNumber = "bInterfaceNumber";
		public const string kUSBAlternateSetting = "bAlternateSetting";
		public const string kUSBNumEndpoints = "bNumEndpoints";
		public const string kUSBInterfaceClass = "bInterfaceClass";
		public const string kUSBInterfaceSubClass = "bInterfaceSubClass";
		public const string kUSBInterfaceProtocol = "bInterfaceProtocol";
		public const string kUSBInterfaceStringIndex = "iInterface";
		public const string kUSBConfigurationValue = "bConfigurationValue";
		public const string kUSBProductString = "USB Product Name";
		public const string kUSBVendorString = "USB Vendor Name";
		public const string kUSBSerialNumberString = "USB Serial Number";
		public const string kUSB1284DeviceID = "1284 Device ID";

		// Registry plane names
		public const string kIOServicePlane = "IOService";
		public const string kIOPowerPlane = "IOPower";
		public const string kIODeviceTreePlane = "IODeviceTree";
		public const string kIOAudioPlane = "IOAudio";
		public const string kIOFireWirePlane = "IOFireWire";
		public const string kIOUSBPlane = "IOUSB";
		public const string kIOUSBDeviceClassName = "IOUSBDevice";

		//public static byte[] kIOUSBDeviceUserClientTypeID = new byte[] { 0x9d, 0xc7, 0xb7, 0x80, 0x9e, 0xc0, 0x11, 0xD4, 0xa5, 0x4f, 0x00, 0x0a, 0x27, 0x05, 0x28, 0x61 };
		//public static byte[] kIOUSBInterfaceUserClientTypeID = new byte[] { 0x2d, 0x97, 0x86, 0xc6, 0x9e, 0xf3, 0x11, 0xD4, 0xad, 0x51, 0x00, 0x0a, 0x27, 0x05, 0x28, 0x61 };
		//public static byte[] kIOCFPlugInInterfaceID = new byte[] { 0xC2, 0x44, 0xE8, 0x58, 0x10, 0x9C, 0x11, 0xD4, 0x91, 0xD4, 0x00, 0x50, 0xE4, 0xC6, 0x42, 0x6F };

		// KERN_* codes.
		public const int KERN_SUCCESS = 0;
		public const int KERN_FAILURE = 5;

		// IOKit master port (default port is NULL).
		public static readonly IntPtr kIOMasterPortDefault = IntPtr.Zero;
		// Default CF allocator (default allocator is NULL).
		public static readonly IntPtr kCFAllocatorDefault = IntPtr.Zero;
		// Options for IORegistryCreateIterator(), IORegistryEntryCreateIterator, IORegistryEntrySearchCFProperty()
		public const uint kIORegistryIterateRecursively = 0x00000001;
		public const uint kIORegistryIterateParents = 0x00000002;

		public const uint kIOMessageServiceIsTerminated = 0xe0000010;

		// CF strings
		public const string kCFRunLoopDefaultMode = "kCFRunLoopDefaultMode";

		/// <summary>
		/// Gets a dictionary value.
		/// </summary>
		/// <param name="dictionary">The dictionary to use.</param>
		/// <param name="key">The key to look up.</param>
		/// <returns>A string on sucess; Otherwise, null.</returns>
		public static string GetCFPropertyString(IOObject dictionary, string key)
		{
			var value = String.Empty;
			var kernResult = GetCFPropertyString(dictionary, key, out value);
			if (kernResult == KERN_SUCCESS)
			{
				return value;
			}

			return null;
		}

		/// <summary>
		/// Gets a dictionary value.
		/// </summary>
		/// <param name="dictionary">The dictionary to use.</param>
		/// <param name="key">The key to look up.</param>
		/// <param name="value">Receives the value.</param>
		/// <returns>KERN_SUCCESS on success.</returns>
		public static int GetCFPropertyString(IOObject dictionary, string key, out string value)
		{
			var maxValueSize = 4096;
			var kernResult = KERN_FAILURE;
			var bsdPathAsCFValue = NativeMethods.IORegistryEntrySearchCFProperty(dictionary.Handle, kIOServicePlane, NativeMethods.__CFStringMakeConstantString(key), kCFAllocatorDefault, kIORegistryIterateRecursively);
			// var bsdPathAsCFString = IORegistryEntryCreateCFProperty(modemService, __CFStringMakeConstantString(key), kCFAllocatorDefault, 0);

			if (bsdPathAsCFValue != IntPtr.Zero)
			{
				// Convert the value from a CFString to a C (NULL-terminated) string.
				try
				{
					unsafe
					{
						fixed (char* bsdValue = new char[maxValueSize])
						{
							var result = NativeMethods.CFStringGetCString(bsdPathAsCFValue, bsdValue, maxValueSize, CFStringEncoding.kCFStringEncodingUTF8);

							if (result)
							{
								kernResult = KERN_SUCCESS;
								value = Marshal.PtrToStringAnsi((IntPtr)bsdValue);
							}
							else {
								value = null;
							}
						}
					}
				}
				finally
				{
					NativeMethods.CFRelease(bsdPathAsCFValue);
				}
			}
			else {
				value = null;
			}

			return kernResult;
		}


		/// <summary>
		/// Gets a dictionary value.
		/// </summary>
		/// <param name="dictionary">The dictionary to use.</param>
		/// <param name="key">The key to look up.</param>
		/// <returns>A string on sucess; Otherwise, null.</returns>
		public static int GetCFPropertyInt(IOObject dictionary, string key)
		{
			var result = 0;
			var kernResult = GetCFPropertyInt(dictionary, key, out result);
			if (kernResult == KERN_SUCCESS)
			{
				return result;
			}

			return 0;
		}

		/// <summary>
		/// Gets a dictionary value.
		/// </summary>
		/// <param name="dictionary">The dictionary to use.</param>
		/// <param name="key">The key to look up.</param>
		/// <param name="value">Receives the value.</param>
		/// <returns>An integer.</returns>
		public static int GetCFPropertyInt(IOObject dictionary, string key, out int value)
		{
			var kernResult = KERN_FAILURE;
			var bsdPathAsCFValue = NativeMethods.IORegistryEntrySearchCFProperty(dictionary.Handle, kIOServicePlane, NativeMethods.__CFStringMakeConstantString(key), kCFAllocatorDefault, kIORegistryIterateRecursively);
			//var bsdPathAsCFString = IORegistryEntryCreateCFProperty(modemService, __CFStringMakeConstantString(key), kCFAllocatorDefault, 0);

			if (bsdPathAsCFValue != IntPtr.Zero)
			{
				// Convert the value from a CFNumber to an integer.
				try
				{
					var bsdValue = IntPtr.Zero;
					var result = NativeMethods.CFNumberGetValue(bsdPathAsCFValue, CFNumberType.kCFNumberIntType, out bsdValue);

					if (result)
					{
						kernResult = KERN_SUCCESS;
						value = bsdValue.ToInt32();
					}
					else {
						value = 0;
					}
				}
				finally
				{
					NativeMethods.CFRelease(bsdPathAsCFValue);
				}
			}
			else {
				value = 0;
			}

			return kernResult;
		}

		/// <summary>
		/// Releases a CFObjects handle.
		/// </summary>
		/// <param name="obj">The object to release.</param>
		internal static void CFRelease(CFObject obj)
		{
			NativeMethods.CFRelease(obj.Handle);
		}

		/// <summary>
		/// Releases a CFObjects handle.
		/// </summary>
		/// <param name="obj">The object to release.</param>
		internal static void IOObjectRelease(IOObject obj)
		{
			NativeMethods.IOObjectRelease(obj.Handle);
		}

		/// <summary>
		/// Releases a CFObjects handle.
		/// </summary>
		/// <param name="obj">The object to release.</param>
		internal static IOObject IOIteratorNext(IOObject obj)
		{
			var result = NativeMethods.IOIteratorNext(obj.Handle);
			if (result != IntPtr.Zero)
			{
				return new IOObject(result);
			}

			return null;
		}

		public static string IORegistryEntryGetName(IntPtr entry)
		{
			var ret = "";

			unsafe
			{
				fixed(char* name = new char[255])
				{
					var result = NativeMethods.IORegistryEntryGetName(entry, name);
					if (result == KERN_SUCCESS)
					{
						ret = Marshal.PtrToStringAnsi((IntPtr)name);
					}
					else {
						ret = null;
					}
				}
			}
			return ret;
		}
	}
}

