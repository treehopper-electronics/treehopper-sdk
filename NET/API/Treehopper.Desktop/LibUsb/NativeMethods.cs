using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Treehopper.Desktop.LibUsb
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate int HotplugCallbackFunction (IntPtr context, IntPtr device, HotplugEvent e, IntPtr userData);

	internal static class NativeMethods
	{
		internal const CallingConvention CC = 0;
		internal const string LIBUSB_DLL = "libusb-1.0.dll";

		internal const int HotplugMatchAny = -1;

		// libusb_init
		[DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = false, EntryPoint = "libusb_init")]
		internal static extern int Init(out IntPtr pContext);

		// libusb_detach_kernel_driver
		[DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = false, EntryPoint = "libusb_detach_kernel_driver")]
		internal static extern int DetachKernelDriver(LibUsbDeviceHandle dev, int interface_number);

		// libusb_exit
		[DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = false, EntryPoint = "libusb_exit")]
		internal static extern void Exit(IntPtr pContext);

		// libusb_hotplug_register_callback
		[DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = false, EntryPoint = "libusb_hotplug_register_callback")]
		internal static extern int HotplugRegisterCallback ([In] IntPtr context, HotplugEvent events, int flags, int vendor_id, int product_id, int dev_class, HotplugCallbackFunction cb_fn, IntPtr user_data, IntPtr HandleRef);

		// libusb_handle_events
		[DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = false, EntryPoint = "libusb_handle_events")]
		internal static extern int HandleEvents(IntPtr pContext, IntPtr completed);

		// libusb_get_device_list
		[DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = false, EntryPoint = "libusb_get_device_list")]
		public static extern int GetDeviceList([In] IntPtr context, [Out] out IntPtr device);

		// libusb_free_device_list
		[DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = false, EntryPoint = "libusb_free_device_list")]
		internal static extern void FreeDeviceList(IntPtr pHandleList, int unrefDevices);

		// libusb_get_device_descriptor
		[DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = false, EntryPoint = "libusb_get_device_descriptor")]
		public static extern int GetDeviceDescriptor([In] IntPtr deviceProfileHandle, [Out] LibUsbDeviceDescriptor deviceDescriptor);

		// libusb_get_string_descriptor_ascii
		[DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = false, EntryPoint = "libusb_get_string_descriptor_ascii")]
		public static extern int GetStringDescriptor([In] LibUsbDeviceHandle deviceProfileHandle, [In] byte desc_index, StringBuilder sb, int length);

		[DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = false, EntryPoint = "libusb_get_string_descriptor_ascii")]
		public static extern int GetStringDescriptor([In] LibUsbDeviceHandle deviceProfileHandle, [In] byte desc_index, Byte[] charData, int length);

		// libusb_open
		[DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = false, EntryPoint = "libusb_open")]
		internal static extern int Open([In] IntPtr deviceProfileHandle, ref IntPtr deviceHandle);

		[DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = false, EntryPoint = "libusb_open_device_with_vid_pid")]
		internal static extern IntPtr Open([In] IntPtr context, ushort vendor_id, ushort product_id);


		// libusb_set_configuration
		[DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = false, EntryPoint = "libusb_set_configuration")]
		public static extern int SetConfiguration([In] LibUsbDeviceHandle deviceHandle, int configurationNumber);

		// libusb_claim_interface
		[DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = false, EntryPoint = "libusb_claim_interface")]
		public static extern int ClaimInterface([In] LibUsbDeviceHandle deviceHandle, int interfaceNumber);

		// libusb_set_interface_alt_setting
		[DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = false, EntryPoint = "libusb_set_interface_alt_setting")]
		public static extern int SetInterfaceAltSetting([In] LibUsbDeviceHandle deviceHandle, int interfaceNumber, int alternateSetting);


		// libusb_close
		[DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = false, EntryPoint = "libusb_close")]
		internal static extern void Close([In] IntPtr deviceHandle);

		// libusb_bulk_transfer
		[DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = false, EntryPoint = "libusb_bulk_transfer")]
		internal static extern LibUsbError BulkTransfer([In] LibUsbDeviceHandle deviceHandle, byte endpoint, Byte[] Data, int length, out int actualLength, int timeout);

		[DllImport(LIBUSB_DLL, CallingConvention = CC, SetLastError = false, EntryPoint = "libusb_interrupt_transfer")]
		public static extern int InterruptTransfer([In] LibUsbDeviceHandle deviceHandle, byte endpoint, Byte[] pData, int length, out int actualLength,	int timeout);
	}
}
