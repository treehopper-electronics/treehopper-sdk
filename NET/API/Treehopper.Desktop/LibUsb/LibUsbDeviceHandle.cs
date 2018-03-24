using System;
using System.Runtime.InteropServices;

namespace Treehopper.Desktop.LibUsb
{
    internal class LibUsbDeviceHandle : SafeHandle
    {
        internal LibUsbDeviceHandle(IntPtr pDeviceHandle) : base(pDeviceHandle, true)
        {
        }

        public override bool IsInvalid
        {
            get
            {
                if (handle != IntPtr.Zero)
                    return handle == new IntPtr(-1);
                return true;
            }
        }

        /// <summary>
        ///     Closes the <see cref="LibUsbDeviceHandle" />.
        /// </summary>
        /// <returns>
        ///     true if the <see cref="LibUsbDeviceHandle" /> is released successfully; otherwise, in the event of a catastrophic
        ///     failure, false. In this case, it generates a ReleaseHandleFailed Managed Debugging Assistant.
        /// </returns>
        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
            {
                NativeMethods.Close(handle);
                SetHandleAsInvalid();
            }
            return true;
        }

        /// <summary>
        ///     Closes the <see cref="LibUsbDeviceHandle" /> reference.  When all references are no longer is use, the device
        ///     is closed in the <see cref="ReleaseHandle" /> finalizer.
        /// </summary>
        /// <remarks>
        ///     <note title="Libusb-1.0 API Note:" type="cpp">
        ///         The <see cref="Close" /> method is roughly equivalent to
        ///         <a href="http://libusb.sourceforge.net/api-1.0/group__dev.html#ga779bc4f1316bdb0ac383bddbd538620e">libusb_close()</a>
        ///         .
        ///     </note>
        /// </remarks>
        public new void Close()
        {
            base.Close();
        }
    }
}