using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Desktop.WinUsb
{
    class SafeWinUsbHandle : SafeHandle
    {
        public SafeWinUsbHandle()
            : base(IntPtr.Zero, true) { }

        public SafeWinUsbHandle(IntPtr handle)
            : base(handle, true) { }

        ///<summary>
        ///Gets a value indicating whether the <see cref="SafeWinUsbInterfaceHandle"/> value is invalid.
        ///</summary>
        ///
        ///<returns>
        ///true if the <see cref="SafeWinUsbInterfaceHandle"/> is valid; otherwise, false.
        ///</returns>
        public override bool IsInvalid => (handle == IntPtr.Zero || handle.ToInt64() == -1);

        ///<summary>
        ///Executes the code required to free the <see cref="SafeWinUsbInterfaceHandle"/>.
        ///</summary>
        ///
        ///<returns>
        ///true if the <see cref="SafeWinUsbInterfaceHandle"/> is released successfully; otherwise, in the event of a catastrophic failure, false. In this case, it generates a ReleaseHandleFailed Managed Debugging Assistant.
        ///</returns>
        ///
        protected override bool ReleaseHandle()
        {
            bool bSuccess = true;
            if (!IsInvalid)
            {
                bSuccess = WinUsb.NativeMethods.WinUsb_Free(handle);
                handle = IntPtr.Zero;
            }
            return bSuccess;
        }
    }
}
