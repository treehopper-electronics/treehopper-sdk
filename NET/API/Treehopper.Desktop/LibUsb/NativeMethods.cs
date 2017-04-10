using System;
using System.Runtime.InteropServices;

namespace Treehopper.Desktop.LibUsb
{
	internal static class NativeMethods
	{
		internal const CallingConvention CC = 0;
		internal const string LIBUSB_DLL = "libusb-1.0.dll";

		// libusb_init
		[DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = false, EntryPoint = "libusb_init")]
		internal static extern int Init(ref IntPtr pContext);

		// libusb_exit
		[DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = false, EntryPoint = "libusb_exit")]
		internal static extern void Exit(IntPtr pContext);


		[DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = false, EntryPoint = "libusb_get_device_list")]
		public static extern int GetDeviceList([In]IntPtr context, [Out] out IntPtr device);


		[DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = false, EntryPoint = "libusb_free_device_list")]
		internal static extern void FreeDeviceList(IntPtr pHandleList, int unrefDevices);


		[DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = false, EntryPoint = "libusb_get_device_descriptor")]
		public static extern int GetDeviceDescriptor([In] IntPtr deviceProfileHandle,
													  [Out] LibUsbDeviceDescriptor deviceDescriptor);

		// libusb_open
		[DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = false, EntryPoint = "libusb_open")]
		internal static extern int Open([In] IntPtr deviceProfileHandle, ref IntPtr deviceHandle);

		// libusb_close
		[DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = false, EntryPoint = "libusb_close")]
		internal static extern void Close([In] IntPtr deviceHandle);

		// libusb_bulk_transfer
		[DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = false, EntryPoint = "libusb_bulk_transfer")]
		internal static extern int BulkTransfer([In] LibUsbDeviceHandle deviceHandle,
											  byte endpoint,
											  Byte[] Data,
											  int length,
											  out int actualLength,
											  int timeout);

	}
}
