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
using System.Runtime.InteropServices;
using System.Text;
using LibUsbDotNet.Main;
using Microsoft.Win32.SafeHandles;
using Debug=System.Diagnostics.Debug;

namespace LibUsbDotNet.LibUsb
{
    /// <summary> LibUsb specific members for device registry settings.
    /// </summary> 
    public class LibUsbRegistry : UsbRegistry
    {
        private readonly string mDeviceFilename;
        private readonly int mDeviceIndex;

        private LibUsbRegistry(SafeFileHandle usbHandle, string deviceFileName, int deviceIndex)
        {
			throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the 0 based index of this libusb device
        /// </summary>
        public int DeviceIndex
        {
            get { return mDeviceIndex; }
        }

        /// <summary>
        /// Gets a list of available LibUsb devices.
        /// </summary>
        public static List<LibUsbRegistry> DeviceList
        {
            get
            {
				throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Check this value to determine if the usb device is still connected to the bus and ready to open.
        /// </summary>
        public override bool IsAlive
        {
            get
            {
                if (String.IsNullOrEmpty(SymbolicName)) throw new UsbException(this, "A symbolic name is required for this property.");
                List<LibUsbRegistry> deviceList = DeviceList;
                foreach (LibUsbRegistry registry in deviceList)
                {
                    if (String.IsNullOrEmpty(registry.SymbolicName)) continue;

                    if (registry.SymbolicName == SymbolicName)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Opens the USB device for communucation.
        /// </summary>
        /// <returns>Return a new instance of the <see cref="UsbDevice"/> class.
        /// If the device fails to open a null refrence is return. For extended error
        /// information use the <see cref="UsbDevice.UsbErrorEvent"/>.
        ///  </returns>
        public override UsbDevice Device
        {
            get
            {
                LibUsbDevice libUsbDevice;
                Open(out libUsbDevice);
                return libUsbDevice;
            }
        }

        /// <summary>
        /// Gets the DeviceInterfaceGuids for the WinUsb device.
        /// </summary>
        public override Guid[] DeviceInterfaceGuids
        {
            get
            {
                if (ReferenceEquals(mDeviceInterfaceGuids, null))
                {
                    if (!mDeviceProperties.ContainsKey(LIBUSB_INTERFACE_GUIDS)) return new Guid[0];

                    string[] saDeviceInterfaceGuids = mDeviceProperties[LIBUSB_INTERFACE_GUIDS] as string[];
                    if (ReferenceEquals(saDeviceInterfaceGuids, null)) return new Guid[0];

                    mDeviceInterfaceGuids = new Guid[saDeviceInterfaceGuids.Length];

                    for (int i = 0; i < saDeviceInterfaceGuids.Length; i++)
                    {
                        string sGuid = saDeviceInterfaceGuids[i].Trim(new char[] {' ', '{', '}', '[', ']', '\0'});
                        mDeviceInterfaceGuids[i] = new Guid(sGuid);
                    }
                }
                return mDeviceInterfaceGuids;
            }
        }

        /// <summary>
        /// Opens the USB device for communucation.
        /// </summary>
        /// <param name="usbDevice">The newly created UsbDevice.</param>
        /// <returns>True on success.</returns>
        public bool Open(out LibUsbDevice usbDevice)
        {
            bool bSuccess = LibUsbDevice.Open(mDeviceFilename, out usbDevice);
            if (bSuccess)
            {
                usbDevice.mUsbRegistry = this;
            }

            return bSuccess;
        }

        /// <summary>
        /// Opens the USB device for communucation.
        /// </summary>
        /// <param name="usbDevice">The newly created UsbDevice.</param>
        /// <returns>True on success.</returns>
        public override bool Open(out UsbDevice usbDevice)
        {
            usbDevice = null;
            LibUsbDevice libUsbDevice;
            bool bSuccess = Open(out libUsbDevice);
            if (bSuccess)
                usbDevice = libUsbDevice;
            return bSuccess;
        }

        internal ErrorCode GetCustomDeviceKeyValue(SafeFileHandle usbHandle, string key, out string propData, int maxDataLength)
        {
            byte[] propDataBytes;
            ErrorCode eReturn = GetCustomDeviceKeyValue(usbHandle, key, out propDataBytes, maxDataLength);
            if (eReturn == ErrorCode.None)
            {
                propData = Encoding.Unicode.GetString(propDataBytes);
                propData.TrimEnd(new char[] {'\0'});
            }
            else
            {
                propData = null;
            }

            return eReturn;
        }

        internal ErrorCode GetCustomDeviceKeyValue(SafeFileHandle usbHandle, string key, out byte[] propData, int maxDataLength)
        {
			throw new NotImplementedException();
        }

        private void GetPropertiesSPDRP(SafeHandle usbHandle)
        {
			throw new NotImplementedException();
        }
    }
}