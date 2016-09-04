using System;
using System.Runtime.InteropServices;
using LibUsbDotNet.Main;
using LibUsb.Descriptors;

namespace LibUsb.Profile
{
    /// <summary>
    /// The <see cref="LibUsbConfigHandle"/> class hold the internal pointer to a libusb <see cref="LibUsbConfigDescriptor"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// To acquire a <see cref="LibUsbConfigHandle"/> use:
    /// <list type="bullet">
    /// <item><see cref="LibUsbApi.GetActiveConfigDescriptor"/></item>
    /// <item><see cref="LibUsbApi.GetConfigDescriptor"/></item>
    /// <item><see cref="LibUsbApi.GetConfigDescriptorByValue"/></item>
    /// </list>
    /// </para>
    /// <para>To access configuration information see <see cref="LibUsbConfigDescriptor(LibUsbConfigHandle)"/>.</para>
    /// <example><code source="..\MonoLibUsb\MonoUsb.ShowConfig\ShowConfig.cs" lang="cs"/></example>
    /// </remarks>
    public class LibUsbConfigHandle:SafeContextHandle
    {
        private LibUsbConfigHandle() : base(IntPtr.Zero,true) {}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool ReleaseHandle() 
        {
            if (!IsInvalid)
            {
                LibUsbApi.FreeConfigDescriptor(handle);
                SetHandleAsInvalid();
            }
            return true;
        }
    }
}
