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
using System.Diagnostics;
using System.Runtime.InteropServices;
using LibUsbDotNet.Main;

namespace LibUsb.Profile
{
    /// <summary>
    /// Wraps a profile handle into a <see cref="System.Runtime.ConstrainedExecution.CriticalFinalizerObject"/>. 
    /// Profile handles are used for getting device descriptor information and opening the device.  Profile handles
    /// are known connected and usually supported usb device that can be opened and used.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When a <see cref="LibUsbProfileHandle"/> instance is created and wrapped around the 
    /// <a href="http://libusb.sourceforge.net/api-1.0/group__dev.html#ga77eedd00d01eb7569b880e861a971c2b">libusb_device</a>
    /// pointer, <see cref="LibUsbApi.RefDevice"/> is called.  When all references to this 
    /// <see cref="LibUsbProfileHandle"/> instance are out-of-scope or have all been closed, this profile handle is de-referenced with 
    /// <see cref="LibUsbApi.UnrefDevice"/>.
    /// When the reference count equals zero, memory is freed and resources are released.
    /// </para>
    /// <para>
    /// The <see cref="LibUsbProfileHandle"/> class ensures all device profiles get closed and freed 
    /// regardless of abnormal program terminations or coding errors. 
    /// </para>
    /// <para>
    /// Certain operations can be performed using just the <see cref="LibUsbProfileHandle"/>, but in order to do 
    /// any I/O you will have to first obtain a <see cref="LibUsbDeviceHandle"/> using <see cref="LibUsbApi.Open"/>.
    /// </para>
    /// </remarks>    
    public class LibUsbProfileHandle : SafeContextHandle
    {


        /// <summary>
        /// Wraps a raw usb device profile handle pointer in a <see cref="LibUsbProfileHandle"/> class.
        /// </summary>
        /// <param name="pProfileHandle">the profile handle to wrap.</param>
        public LibUsbProfileHandle(IntPtr pProfileHandle) : base(pProfileHandle,true)
        {
            lock (oDeviceProfileRefLock)
            {
                LibUsbApi.RefDevice(pProfileHandle);
                mDeviceProfileRefCount++;
            }
        }

        #region Overridden Members

        /// <summary>
        /// When overridden in a derived class, executes the code required to free the handle.
        /// </summary>
        /// <returns>
        /// true if the handle is released successfully; otherwise, in the event of a catastrophic failure, false. In this case, it generates a ReleaseHandleFailed Managed Debugging Assistant.
        /// </returns>
        protected override bool ReleaseHandle()
        {
            lock (oDeviceProfileRefLock)
            {
                if (!IsInvalid)
                {
                    LibUsbApi.UnrefDevice(handle);
                    mDeviceProfileRefCount--;
                    SetHandleAsInvalid();
                    Debug.Print(GetType().Name + " : ReleaseHandle #{0}", mDeviceProfileRefCount);
                }
                return true;
            }
        }

        #endregion

        #region STATIC Members

        internal static int DeviceProfileRefCount
        {
            get
            {
                lock (oDeviceProfileRefLock)
                    return mDeviceProfileRefCount;
            }
        }

        private static int mDeviceProfileRefCount;
        private static readonly object oDeviceProfileRefLock = new object();

        #endregion
    }
}