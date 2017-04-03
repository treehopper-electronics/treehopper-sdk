using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MonoMac.IOKit;

namespace Treehopper.Desktop.MacUsb.IOKit
{
	public static class IOKitFramework
	{
		/// <summary>
		/// IOKit Framework Path
		/// </summary>
		public const string IOKitFrameworkPath = "IOKit.framework/IOKit";

		// Service Matching That is the 'IOProviderClass'.
		public const string kIOSerialBSDServiceValue = "IOSerialBSDClient";


		public const string kIOUSBInterfaceClassName = "IOUSBInterface";

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

		// IOReturn* codes
		public const int IOKitCommonError = 0;
		public const int kIOReturnSuccess = KERN_SUCCESS;                       // OK
		public const int kIOReturnError = IOKitCommonError + 0x2bc;             // general error 
		public const int kIOReturnNoMemory = IOKitCommonError + 0x2bd;          // can't allocate memory 
		public const int kIOReturnNoResources = IOKitCommonError + 0x2be;       // resource shortage 
		public const int kIOReturnIPCError = IOKitCommonError + 0x2bf;          // error during IPC 
		public const int kIOReturnNoDevice = IOKitCommonError + 0x2c0;          // no such device 
		public const int kIOReturnNotPrivileged = IOKitCommonError + 0x2c1;     // privilege violation 
		public const int kIOReturnBadArgument = IOKitCommonError + 0x2c2;       // invalid argument 
		public const int kIOReturnLockedRead = IOKitCommonError + 0x2c3;        // device read locked 
		public const int kIOReturnLockedWrite = IOKitCommonError + 0x2c4;       // device write locked 
		public const int kIOReturnExclusiveAccess = IOKitCommonError + 0x2c5;   // exclusive access and


		public static byte[] kIOUSBInterfaceUserClientTypeID = new byte[] { 0x2d, 0x97, 0x86, 0xc6, 0x9e, 0xf3, 0x11, 0xD4, 0xad, 0x51, 0x00, 0x0a, 0x27, 0x05, 0x28, 0x61 };
		public static byte[] kIOCFPlugInInterfaceID = new byte[] { 0xC2, 0x44, 0xE8, 0x58, 0x10, 0x9C, 0x11, 0xD4, 0x91, 0xD4, 0x00, 0x50, 0xE4, 0xC6, 0x42, 0x6F };

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

		/// <summary>
		/// CF number types
		/// </summary>
		public enum CFNumberType
		{
			/* Fixed-width types */
			kCFNumberSInt8Type = 1,
			kCFNumberSInt16Type = 2,
			kCFNumberSInt32Type = 3,
			kCFNumberSInt64Type = 4,
			kCFNumberFloat32Type = 5,
			kCFNumberFloat64Type = 6,   /* 64-bit IEEE 754 */
										/* Basic C types */
			kCFNumberCharType = 7,
			kCFNumberShortType = 8,
			kCFNumberIntType = 9,
			kCFNumberLongType = 10,
			kCFNumberLongLongType = 11,
			kCFNumberFloatType = 12,
			kCFNumberDoubleType = 13,
			/* Other */
			kCFNumberCFIndexType = 14,
			//kCFNumberNSIntegerType CF_ENUM_AVAILABLE(10_5, 2_0) = 15,
			//kCFNumberCGFloatType CF_ENUM_AVAILABLE(10_5, 2_0) = 16,
			kCFNumberMaxType = 16
		};

		/// <summary>
		/// CF string encoding types.
		/// </summary>
		public enum CFStringEncoding
		{
			kCFStringEncodingMacRoman = 0,
			kCFStringEncodingWindowsLatin1 = 0x0500, /* ANSI codepage 1252 */
			kCFStringEncodingISOLatin1 = 0x0201, /* ISO 8859-1 */
			kCFStringEncodingNextStepLatin = 0x0B01, /* NextStep encoding*/
			kCFStringEncodingASCII = 0x0600, /* 0..127 (in creating CFString, values greater than 0x7F are treated as corresponding Unicode value) */
			kCFStringEncodingUnicode = 0x0100, /* kTextEncodingUnicodeDefault  + kTextEncodingDefaultFormat (aka kUnicode16BitFormat) */
			kCFStringEncodingUTF8 = 0x08000100, /* kTextEncodingUnicodeDefault + kUnicodeUTF8Format */
			kCFStringEncodingNonLossyASCII = 0x0BFF, /* 7bit Unicode variants used by Cocoa & Java */

			kCFStringEncodingUTF16 = 0x0100, /* kTextEncodingUnicodeDefault + kUnicodeUTF16Format (alias of kCFStringEncodingUnicode) */
			kCFStringEncodingUTF16BE = 0x10000100, /* kTextEncodingUnicodeDefault + kUnicodeUTF16BEFormat */
			kCFStringEncodingUTF16LE = 0x14000100, /* kTextEncodingUnicodeDefault + kUnicodeUTF16LEFormat */

			kCFStringEncodingUTF32 = 0x0c000100, /* kTextEncodingUnicodeDefault + kUnicodeUTF32Format */
			kCFStringEncodingUTF32BE = 0x18000100, /* kTextEncodingUnicodeDefault + kUnicodeUTF32BEFormat */
			kCFStringEncodingUTF32LE = 0x1c000100 /* kTextEncodingUnicodeDefault + kUnicodeUTF32LEFormat */
		};


		/// <summary>
		/// Returns an iterator to look up available communication devices.
		/// </summary>
		/// <returns>An instance of the type <see cref="IOIterator"/> on success; Otherwise, false.</returns>
		public static IOIterator FindUsbDevices()
		{
			var usbDeviceIterator = IntPtr.Zero;
			int kernResult = NativeMethods.IOServiceGetMatchingServices(kIOMasterPortDefault, NativeMethods.IOServiceMatching(kIOUSBDeviceClassName), out usbDeviceIterator);

			if (KERN_SUCCESS != kernResult)
			{
				Debug.WriteLine("No devices were found");
				return null;
			}

			if (usbDeviceIterator == IntPtr.Zero)
			{
				return null;
			}

			return new IOIterator(usbDeviceIterator);
		}

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

		public unsafe static void GetUsbDevice(IOObject usbService)
		{
			IOCFPlugInInterface[] plugInInterface = new IOCFPlugInInterface[5];
			var device = IntPtr.Zero;

			if (usbService.Handle == IntPtr.Zero)
			{
				throw new NullReferenceException("USB Service is null");
			}

			int score = 0;

			var status = NativeMethods.IOCreatePlugInInterfaceForService(usbService.Handle, kIOUSBInterfaceUserClientTypeID, kIOCFPlugInInterfaceID, plugInInterface, IntPtr.Zero);
			if (status == kIOReturnSuccess)
			{
				//var currentService = IntPtr.Zero;
				//while ((currentService = NativeMethods.IOIteratorNext(iterator)) != IntPtr.Zero)
				//{
				//	unsafe
				//	{
				//		fixed (char* serviceName = new char[4096])
				//		{
				//			status = NativeMethods.IORegistryEntryGetNameInPlane(currentService, kIOServicePlane, serviceName);
				//		}

				//		if (status == kIOReturnSuccess)
				//		{
				//			device = currentService;
				//			break;
				//		}
				//		else
				//		{
				//			// Release the service object which is no longer needed
				//			NativeMethods.IOObjectRelease(currentService);
				//		}
				//	}
				}

				// Release the iterator
				//NativeMethods.IOObjectRelease(iterator);
			}

			//return new IOObject(device);


		//public static IOObject GetUsbDevice(IOObject usbService)
		//{
		//	var iterator = IntPtr.Zero;
		//	var device = IntPtr.Zero;

		//	if (usbService.Handle == IntPtr.Zero)
		//	{
		//		throw new NullReferenceException("USB Service is null");
		//	}

		//	var status = NativeMethods.IORegistryEntryCreateIterator(usbService.Handle, kIOServicePlane, kIORegistryIterateParents | kIORegistryIterateRecursively, out iterator);
		//	if (status == kIOReturnSuccess)
		//	{
		//		var currentService = IntPtr.Zero;
		//		while ((currentService = NativeMethods.IOIteratorNext(iterator)) != IntPtr.Zero)
		//		{
		//			unsafe
		//			{
		//				fixed (char* serviceName = new char[4096])
		//				{
		//					status = NativeMethods.IORegistryEntryGetNameInPlane(currentService, kIOServicePlane, serviceName);
		//				}

		//				if (status == kIOReturnSuccess)
		//				{
		//					device = currentService;
		//					break;
		//				}
		//				else {
		//					// Release the service object which is no longer needed
		//					NativeMethods.IOObjectRelease(currentService);
		//				}
		//			}
		//		}

		//		// Release the iterator
		//		NativeMethods.IOObjectRelease(iterator);
		//	}

		//	return new IOObject(device);
		//}

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



		public unsafe static class NativeMethods
		{
			/*! @function IORegistryEntryCreateCFProperty
    @abstract Create a CF representation of a registry entry's property.
    @discussion This function creates an instantaneous snapshot of a registry entry property, creating a CF container analogue in the caller's task. Not every object available in the kernel is represented as a CF container; currently OSDictionary, OSArray, OSSet, OSSymbol, OSString, OSData, OSNumber, OSBoolean are created as their CF counterparts. 
    @param entry The registry entry handle whose property to copy.
    @param key A CFString specifying the property name.
    @param allocator The CF allocator to use when creating the CF container.
    @param options No options are currently defined.
    @result A CF container is created and returned the caller on success. The caller should release with CFRelease. */

			[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
			public static extern IntPtr IORegistryEntryCreateCFProperty(IntPtr entry, IntPtr key, IntPtr allocator, int options);
			
			[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
			public static extern IntPtr __CFStringMakeConstantString(string str);

			[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
			public static extern IntPtr IOServiceMatching(string name);

			[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
			public static extern void CFDictionarySetValue(IntPtr theDict, IntPtr key, IntPtr value);

			[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
			public static extern int IOServiceGetMatchingServices(IntPtr masterPort, IntPtr matching, out IntPtr iterator);

			[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
			public static extern int IOCreatePlugInInterfaceForService(IntPtr service, Byte[] pluginType, Byte[] interfaceType, IOCFPlugInInterface[] plugIn, IntPtr theScore);

			[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
			public static extern int IOObjectRelease(IntPtr obj);

			[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
			public static extern IntPtr IOIteratorNext(IntPtr iterator);

			[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
			public static extern int IORegistryEntryGetName(IntPtr entry, char* buffer);

			[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
			public static extern IntPtr IORegistryEntryCreateCFProperty(IntPtr entry, IntPtr key, IntPtr allocator, uint options);

			[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
			public static extern IntPtr IORegistryEntrySearchCFProperty(IntPtr entry, string plane, IntPtr key, IntPtr allocator, uint options);

			[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
			public static extern int CFRelease(IntPtr obj);

			[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
			public static extern bool CFStringGetCString(IntPtr theString, char* buffer, long bufferSize, CFStringEncoding encoding);

			[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
			public static extern bool CFNumberGetValue(IntPtr number, CFNumberType theType, out IntPtr valuePtr);

			[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
			public static extern int IORegistryEntryCreateIterator(IntPtr entry, string plane, uint options, out IntPtr iterator);

			[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
			public static extern int IORegistryEntryGetNameInPlane(IntPtr entry, string plane, char* name);

			[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
			public static extern bool IOObjectConformsTo(IntPtr obj, string className);


		}
	}
}

