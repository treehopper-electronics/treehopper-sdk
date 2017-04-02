// Copyright © 2006-2010 Travis Robinson. All rights reserved.
// 
// website: http://sourceforge.net/projects/libusbdotnet
// e-mail:  libusbdotnet@gmail.com
// 
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the
// Free Software Foundation; either version 2 of the License, or 
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
// for more details.
// 
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA. or 
// visit www.gnu.org.
// 
// 
using System;
using System.Collections.Generic;
using LibUsbDotNet.Main;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.WinUsb.Internal;
using Debug = System.Diagnostics.Debug;
using LibUsbDotNet.WinHid;
using LibUsbDotNet.MacUsb;


namespace LibUsbDotNet
{
    public abstract partial class UsbDevice
    {
        private static WinUsbAPI _winUsbApi;
        private static WinHidApi _winHidApi;

        private static object mHasWinUsbDriver;
        private static object mHasLibUsbWinBackDriver;

        private static UsbKernelVersion mUsbKernelVersion;

        /// <summary>
        /// Gets a list of all available USB devices (WinUsb, LibUsb, Linux LibUsb v1.x).
        /// </summary>
        /// <remarks>
        /// Use this property to get a list of USB device that can be accessed by LibUsbDotNet.
        /// Using this property as opposed to <see cref="AllLibUsbDevices"/> and <see cref="AllWinUsbDevices"/>
        /// will ensure your source code is platform-independent.
        /// </remarks>Ø
        public static UsbRegDeviceList AllDevices
        {
            get
            {
                UsbRegDeviceList regDevReturnList = new UsbRegDeviceList();

                if (UsbDevice.IsWindows)
                    return AllWinUsbDevices;
                else if (UsbDevice.IsLinux && !UsbDevice.IsMac)
                    return AllLibUsbDevices;
                else
                    return AllMacUsbDevices; // fill in with Mac later
            }
        }

        /// <summary>
        /// Gets a list of all available libusb-win32 USB devices.
        /// </summary>
        /// <remarks>
        /// <para>
        /// On linux/mac, gets a list of libusb-1.0 devices.
        /// </para>
        /// </remarks>
        public static UsbRegDeviceList AllLibUsbDevices
        {
            get
            {
                UsbRegDeviceList regDevList = new UsbRegDeviceList();

				List<LibUsbDevice> legacyLibUsbDeviceList = LibUsbDevice.MonoUsbDeviceList;
				foreach (LibUsbDevice usbDevice in legacyLibUsbDeviceList)
				{
					regDevList.Add(new LibUsbRegistry(usbDevice));
				}

                return regDevList;
            }
        }

		/// <summary>
		/// Gets a list of all available IOKit USB devices
		/// </summary>
		/// <remarks>
		/// <para>
		/// This only works on Mac
		/// </para>
		/// </remarks>
		public static UsbRegDeviceList AllMacUsbDevices
		{
			get
			{
				UsbRegDeviceList regDevList = new UsbRegDeviceList();

				foreach (MacUsbRegistry usbDevice in MacUsbRegistry.Devices)
				{
					regDevList.Add(usbDevice);
				}

				return regDevList;
			}
		}



        /// <summary>
        /// Returns the last error number reported by LibUsbDotNet.
        /// </summary>
        public static int LastErrorNumber
        {
            get { return UsbError.mLastErrorNumber; }
        }

        /// <summary>
        /// Returns the last error string reported by LibUsbDotNet.
        /// </summary>
        public static string LastErrorString
        {
            get { return UsbError.mLastErrorString; }
        }


        internal static WinUsbAPI WinUsbApi
        {
            get
            {
                if (ReferenceEquals(_winUsbApi, null))
                    _winUsbApi = new WinUsbAPI();
                return _winUsbApi;
            }
        }

        internal static WinHidApi WinHidApi
        {
            get
            {
                if (ReferenceEquals(_winHidApi, null))
                    _winHidApi = new WinHidApi();
                return _winHidApi;
            }
        }



        /// <summary>
        /// Opens the usb device that matches the <see cref="UsbDeviceFinder"/>.
        /// </summary>
        /// <param name="usbDeviceFinder">The <see cref="UsbDeviceFinder"/> class used to find the usb device.</param>
        /// <returns>An valid/open usb device class if the device was found or Null if the device was not found.</returns>
        public static UsbDevice OpenUsbDevice(UsbDeviceFinder usbDeviceFinder) 
        {
            return OpenUsbDevice((Predicate<UsbRegistry>) usbDeviceFinder.Check);
        }

        /// <summary>
        /// Opens the usb device that matches the find predicate.
        /// </summary>
        /// <param name="findDevicePredicate">The predicate function used to find the usb device.</param>
        /// <returns>An valid/open usb device class if the device was found or Null if the device was not found.</returns>
        public static UsbDevice OpenUsbDevice(Predicate<UsbRegistry> findDevicePredicate)
        {
            UsbDevice usbDeviceFound;

            UsbRegDeviceList allDevices = AllDevices;
            UsbRegistry regDeviceFound = allDevices.Find(findDevicePredicate);

            if (ReferenceEquals(regDeviceFound, null)) return null;

            usbDeviceFound = regDeviceFound.Device;

            return usbDeviceFound;
        }

        /// <summary>
        /// Opens a WinUsb device by its DeviceInterfaceGUID.
        /// </summary>
        /// <remarks>
        /// This is the Microsoft-recommended way for opening a WinUsb device.  
        /// LibUsb device can be opened in this way as well.  In order to open
        /// LibUsb devices in this manner, an entry must be added to the driver
        /// inf file:
        /// <para>[Install.HW]</para>
        /// <para>Addreg=Add_LibUsb_Guid_Reg</para>
        /// <para>[Add_LibUsb_Guid_Reg]</para>
        /// <para>HKR,,LibUsbInterfaceGUIDs,0x10000,"{Your-Unique-Guid-String}"</para>
        /// </remarks>
        /// <param name="devInterfaceGuid">Device Interface GUID of the usb device to open.</param>
        /// <param name="usbDevice">On success, a new <see cref="UsbDevice"/> instance.</param>
        /// <returns>True on success.</returns>
        public static bool OpenUsbDevice(ref Guid devInterfaceGuid, out UsbDevice usbDevice)
        {
            usbDevice = null;
            UsbRegDeviceList usbRegDevices = AllDevices;
            foreach (UsbRegistry usbRegistry in usbRegDevices)
            {
                foreach (Guid guid in usbRegistry.DeviceInterfaceGuids)
                {
                    if (guid == devInterfaceGuid)
                    {
                        usbDevice = usbRegistry.Device;
                        if (usbDevice != null) return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Global static error event for all Usb errors.
        /// </summary>
        /// <example>
        /// Sample code to reset an endpoint if a critical error occurs.
        /// <code>
        /// // Hook the usb error handler function
        /// UsbGlobals.UsbErrorEvent += UsbErrorEvent;
        ///private void UsbErrorEvent(object sender, UsbError e)
        ///{
        /// // If the error is from a usb endpoint
        /// if (sender is UsbEndpointBase)
        /// {
        ///     // If the endpoint transfer failed
        ///     if (e.Win32ErrorNumber == 31)
        ///     {
        ///         // If the USB device is still open, connected, and valid
        ///         if (usb.IsOpen)
        ///         {
        ///             // Try to reset then endpoint
        ///             if (((UsbEndpointBase) sender).Reset())
        ///             {
        ///                 // Endpoint reset successful.
        ///                 // Tell LibUsbDotNet to ignore this error and continue.
        ///                 e.Handled = true;
        ///             }
        ///         }
        ///     }
        /// }
        /// }
        /// </code>
        /// </example>
        public static event EventHandler<UsbError> UsbErrorEvent;
    }
}