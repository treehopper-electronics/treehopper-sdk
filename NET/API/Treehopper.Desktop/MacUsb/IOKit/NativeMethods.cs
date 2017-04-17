using System;
using System.Runtime.InteropServices;

namespace Treehopper.Desktop.MacUsb.IOKit
{
	public unsafe static class NativeMethods
	{
		/// <summary>
		/// IOKit Framework Path
		/// </summary>
		public const string IOKitFrameworkPath = "IOKit.framework/IOKit";
		public const string CoreFoundationFrameworkPath = "CoreFoundation.framework/CoreFoundation";

		/*! @function IORegistryEntryCreateCFProperty
@abstract Create a CF representation of a registry entry's property.
@discussion This function creates an instantaneous snapshot of a registry entry property, creating a CF container analogue in the caller's task. Not every object available in the kernel is represented as a CF container; currently OSDictionary, OSArray, OSSet, OSSymbol, OSString, OSData, OSNumber, OSBoolean are created as their CF counterparts. 
@param entry The registry entry handle whose property to copy.
@param key A CFString specifying the property name.
@param allocator The CF allocator to use when creating the CF container.
@param options No options are currently defined.
@result A CF container is created and returned the caller on success. The caller should release with CFRelease. */


		[DllImport(CoreFoundationFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern void CFRunLoopStop(IntPtr runLoopRef);

		[DllImport(CoreFoundationFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern void CFRunLoopRun();

		[DllImport(CoreFoundationFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern void CFRunLoopAddSource(IntPtr rl, IntPtr source, IntPtr mode);

		[DllImport(CoreFoundationFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern IntPtr CFRunLoopGetCurrent();

		[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern IOKitError IOServiceAddInterestNotification(IntPtr notifyPort, IntPtr service, string interestType, IOServiceInterestCallback callback, int refCon, ref IntPtr notification );

		[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern IOKitError IOServiceAddMatchingNotification(IntPtr notifyPort, string notificationType, IntPtr matching, IOServiceMatchingCallback callback, IntPtr refCon, ref IntPtr notification );

		[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern IntPtr IONotificationPortGetRunLoopSource(IntPtr notify);

		[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern IntPtr CFNumberCreate(IntPtr allocator, CFNumberType theType, ref int valuePtr);

		[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern IntPtr IORegistryEntryCreateCFProperty(IntPtr entry, IntPtr key, IntPtr allocator, int options);

		[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern IntPtr __CFStringMakeConstantString(string str);

		[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern IntPtr IOServiceMatching(string name);

		[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern void CFDictionarySetValue(IntPtr theDict, IntPtr key, IntPtr value);

		[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern IOKitError IOServiceGetMatchingServices(IntPtr masterPort, IntPtr matching, out IntPtr iterator);

		[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern IOKitError IOCreatePlugInInterfaceForService(IntPtr service, IntPtr pluginType, IntPtr interfaceType, out IntPtr plugIn, out int theScore);

		[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern IOKitError IOObjectRelease(IntPtr obj);

		[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern IOKitError IOObjectRelease(IUnknownCGuts obj);

		[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern IntPtr IOIteratorNext(IntPtr iterator);

		[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern IOKitError IORegistryEntryGetName(IntPtr entry, char* buffer);

		[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern IntPtr IORegistryEntryCreateCFProperty(IntPtr entry, IntPtr key, IntPtr allocator, uint options);

		[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern IntPtr IORegistryEntrySearchCFProperty(IntPtr entry, string plane, IntPtr key, IntPtr allocator, uint options);

		[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern IOKitError CFRelease(IntPtr obj);

		[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern bool CFStringGetCString(IntPtr theString, char* buffer, long bufferSize, CFStringEncoding encoding);

		[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern bool CFNumberGetValue(IntPtr number, CFNumberType theType, out IntPtr valuePtr);

		[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern IOKitError IORegistryEntryCreateIterator(IntPtr entry, string plane, uint options, out IntPtr iterator);

		[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern IOKitError IORegistryEntryGetNameInPlane(IntPtr entry, string plane, char* name);

		[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern bool IOObjectConformsTo(IntPtr obj, string className);

		[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern IntPtr CFUUIDGetConstantUUIDWithBytes(IntPtr alloc, byte byte0, byte byte1, byte byte2, byte byte3, byte byte4, byte byte5, byte byte6, byte byte7, byte byte8, byte byte9, byte byte10, byte byte11, byte byte12, byte byte13, byte byte14, byte byte15);

		[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
		public static extern IntPtr IONotificationPortCreate(IntPtr masterPort);

		//[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
		//public static extern IntPtr IORegistryEntrySearchCFProperty(int entry, string, CFStringRef key, CFAllocatorRef allocator, IOOptionBits options );

		//[DllImport(IOKitFrameworkPath, CharSet = CharSet.Ansi)]
		//public static extern bool CFStringGetCString(IntPtr theString, [MarshalAs(UnmanagedType.LPStr)] string buffer, long bufferSize, CFStringEncoding encoding);
	}
}
