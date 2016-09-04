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
using LibUsbDotNet.Info;
using LibUsbDotNet.Internal;
using LibUsbDotNet.Main;
using Microsoft.Win32.SafeHandles;

namespace LibUsbDotNet.LibUsb
{
    /// <summary> Contains members that are specific to the LibUsb-Win32 driver.
    /// </summary> 
    /// <remarks> Use the <see cref="T:LibUsbDotNet.UsbDevice"/> class instead to allow your code to work with either LibUsb or WinUsb.
    /// </remarks> 
    public class LibUsbDevice : UsbDevice, IUsbDevice
    {
        private readonly List<int> mClaimedInterfaces = new List<int>();
        private readonly string mDeviceFilename;


        internal LibUsbDevice(UsbApiBase api, SafeHandle usbHandle, string deviceFilename)
            : base(api, usbHandle) { mDeviceFilename = deviceFilename; }

        /// <summary>
        /// Gets a list of libusb devices directly from the kernel; bypassing the windows registry.  
        /// This function is intended for users that do not use the native kernel driver.  
        /// If using the native kernel (sys) driver supplied with LibUsbDotNet see the <see cref="UsbDevice.AllDevices"/>.
        /// <seealso cref="UsbGlobals"/>
        /// <seealso cref="UsbDevice.AllLibUsbDevices"/>
        /// <seealso cref="UsbDevice.AllWinUsbDevices"/>
        /// <seealso cref="UsbDevice.OpenUsbDevice(LibUsbDotNet.Main.UsbDeviceFinder)"/>
        /// </summary>
        public static List<LibUsbDevice> LegacyLibUsbDeviceList
        {
            get
            {
				throw new NotImplementedException();
            }
        }


        /// <summary>
        /// Gets the Device filename for this device.
        /// </summary>
        public string DeviceFilename
        {
            get { return mDeviceFilename; }
        }

        #region IUsbDevice Members

        ///<summary>
        /// Opens the USB device handle.
        ///</summary>
        ///<returns>
        ///True if the device is already opened or was opened successfully.
        ///False if the device does not exists or is no longer valid.  
        ///</returns>
        public override bool Open()
        {
			throw new NotImplementedException();
        }

        /// <summary>
        /// Claims the specified interface of the device.
        /// </summary>
        /// <param name="interfaceID">The interface to claim.</param>
        /// <returns>True on success.</returns>
        public bool ClaimInterface(int interfaceID)
        {
			throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the DriverMode this USB device is using.
        /// </summary>
        public override DriverModeType DriverMode
        {
            get { return DriverModeType.LibUsb; }
        }

        /// <summary>
        /// Closes the <see cref="UsbDevice"/> and disposes any <see cref="UsbDevice.ActiveEndpoints"/>.
        /// </summary>
        /// <returns>True on success.</returns>
        public override bool Close()
        {
            if (IsOpen)
            {
                ReleaseAllInterfaces();
                ActiveEndpoints.Clear();
                mUsbHandle.Close();
            }
            return true;
        }

        /// <summary>
        /// Releases an interface that was previously claimed with <see cref="ClaimInterface"/>.
        /// </summary>
        /// <param name="interfaceID">The interface to release.</param>
        /// <returns>True on success.</returns>
        public bool ReleaseInterface(int interfaceID)
        {
			throw new NotImplementedException();
        }


        /// <summary>
        /// Sets an alternate interface for the most recent claimed interface.
        /// </summary>
        /// <param name="alternateID">The alternate interface to select for the most recent claimed interface See <see cref="ClaimInterface"/>.</param>
        /// <returns>True on success.</returns>
        public bool SetAltInterface(int alternateID)
        {
            if (mClaimedInterfaces.Count == 0) throw new UsbException(this, "You must claim an interface before setting an alternate interface.");
            return SetAltInterface(mClaimedInterfaces[mClaimedInterfaces.Count - 1], alternateID);
        }

        /// <summary>
        /// Sets the USB devices active configuration value. 
        /// </summary>
        /// <param name="config">The active configuration value. A zero value means the device is not configured and a non-zero value indicates the device is configured.</param>
        /// <returns>True on success.</returns>
        /// <remarks>
        /// A USB device can have several different configurations, but only one active configuration.
        /// </remarks>
        public bool SetConfiguration(byte config)
        {
            int uTransferLength;

            UsbSetupPacket setupPkt = new UsbSetupPacket();
            setupPkt.RequestType = (byte)UsbEndpointDirection.EndpointOut | (byte)UsbRequestType.TypeStandard | (byte)UsbRequestRecipient.RecipDevice;
            setupPkt.Request = (byte)UsbStandardRequest.SetConfiguration;
            setupPkt.Value = config;
            setupPkt.Index = 0;
            setupPkt.Length = 0;

            bool bSuccess = ControlTransfer(ref setupPkt, null, 0, out uTransferLength);
            if (bSuccess)
                mCurrentConfigValue = config;
            else
                UsbError.Error(ErrorCode.Win32Error, Marshal.GetLastWin32Error(), "SetConfiguration", this);

            return bSuccess;
        }

        #endregion

        /// <summary>
        /// Opens the USB device for communucation.
        /// </summary>
        /// <param name="deviceFilename">The LibUsb device filename to open.</param>
        /// <param name="usbDevice">The newly created UsbDevice.</param>
        /// <returns>True on success.</returns>
        public static bool Open(string deviceFilename, out LibUsbDevice usbDevice)
        {
			throw new NotImplementedException();
        }

        /// <summary>
        /// Releases all interface claimed by <see cref="ClaimInterface"/>.
        /// </summary>
        /// <returns>True on success.</returns>
        public int ReleaseAllInterfaces()
        {
            int claimedInterfaces = 0;
            while (mClaimedInterfaces.Count > 0)
            {
                claimedInterfaces++;
                ReleaseInterface(mClaimedInterfaces[mClaimedInterfaces.Count - claimedInterfaces]);
            }

            return claimedInterfaces;
        }

        /// <summary>
        /// Releases the last interface claimed by <see cref="ClaimInterface"/>.
        /// </summary>
        /// <returns>True on success.</returns>
        public bool ReleaseInterface()
        {
            //throw new UsbException(this, "You must claim an interface before releasing an interface.");
            if (mClaimedInterfaces.Count == 0) return true;
            return ReleaseInterface(mClaimedInterfaces[mClaimedInterfaces.Count - 1]);
        }

        /// <summary>
        /// Sets an alternate interface for the specified interface.
        /// </summary>
        /// <param name="interfaceID">The interface index to specify an alternate setting for.</param>
        /// <param name="alternateID">The alternate interface setting.</param>
        /// <returns>True on success.</returns>
        public bool SetAltInterface(int interfaceID, int alternateID)
        {
			throw new NotImplementedException();
        }

        /// <summary>
        /// Sends a usb device reset command.
        /// </summary>
        /// <remarks>
        /// After calling <see cref="ResetDevice"/>, the <see cref="LibUsbDevice"/> instance is disposed and
        /// no longer usable.  A new <see cref="LibUsbDevice"/> instance must be obtained from the device list.
        /// </remarks>
        /// <returns>True on success.</returns>
        public bool ResetDevice()
        {
			throw new NotImplementedException();
        }

        internal bool ControlTransferEx(UsbSetupPacket setupPacket,
                                        IntPtr buffer,
                                        int bufferLength,
                                        out int lengthTransferred,
                                        int timeout)
        {
			throw new NotImplementedException();
        }

        internal bool UsbIoSync(int controlCode, Object inBuffer, int inSize, IntPtr outBuffer, int outSize, out int ret) 
		{
			throw new NotImplementedException();
		}
    }
}