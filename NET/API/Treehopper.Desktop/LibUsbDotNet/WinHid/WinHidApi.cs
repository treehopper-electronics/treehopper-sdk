using LibUsbDotNet.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibUsbDotNet.Main;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.IO;
using System.Diagnostics;

namespace LibUsbDotNet.WinHid
{
    internal class WinHidApi : UsbApiBase
    {
        public override bool AbortPipe(SafeHandle interfaceHandle, byte pipeID)
        {
            Debug.WriteLine("Unimplemented: WinHidApi: AbortPipe");
            return false;
        }

        public override bool ControlTransfer(SafeHandle interfaceHandle, UsbSetupPacket setupPacket, IntPtr buffer, int bufferLength, out int lengthTransferred)
        {
            lengthTransferred = 0;
            Debug.WriteLine("Unimplemented: WinHidApi: ControlTransfer");
            return false;
        }

        public override bool FlushPipe(SafeHandle interfaceHandle, byte pipeID)
        {
            Debug.WriteLine("Unimplemented: WinHidApi: FlushPipe");
            return false;
        }

        public override bool GetDescriptor(SafeHandle interfaceHandle, byte descriptorType, byte index, ushort languageID, IntPtr buffer, int bufferLength, out int lengthTransferred)
        {
            lengthTransferred = 0;
            Debug.WriteLine("Unimplemented: WinHidApi: GetDescriptor");
            return false;
        }

        public override bool GetOverlappedResult(SafeHandle interfaceHandle, IntPtr pOverlapped, out int numberOfBytesTransferred, bool wait)
        {
            return Kernel32.GetOverlappedResult(interfaceHandle, pOverlapped, out numberOfBytesTransferred, wait);
        }

        public override bool ReadPipe(UsbEndpointBase endPointBase, IntPtr pBuffer, int bufferLength, out int lengthTransferred, int isoPacketSize, IntPtr pOverlapped)
        {
            var res = Kernel32.ReadFile(endPointBase.Device.Handle, pBuffer, bufferLength, out lengthTransferred, pOverlapped);
            return res > 0;
        }

        public override bool ResetPipe(SafeHandle interfaceHandle, byte pipeID)
        {
            Debug.WriteLine("Unimplemented: WinHidApi: ResetPipe");
            return false;
        }

        public override bool WritePipe(UsbEndpointBase endPointBase, IntPtr pBuffer, int bufferLength, out int lengthTransferred, int isoPacketSize, IntPtr pOverlapped)
        {
            //var fileHandle = (SafeFileHandle)endPointBase.Device.Handle;
            //var file = new FileStream(handle, FileAccess.ReadWrite, 65, true);
            //    file.Write(output, 0, bufferLength+1);
           var res = Kernel32.WriteFile(endPointBase.Device.Handle, pBuffer, bufferLength, out lengthTransferred, pOverlapped);
            
            return res > 0;
                //return Hid.NativeMethods.HidD_SetOutputReport(, pBuffer, bufferLength);
        }
        static SafeFileHandle handle;

        internal static bool OpenDevice(out SafeFileHandle sfhDevice, string DevicePath)
        {
            sfhDevice =
            Kernel32.CreateFile(DevicePath,
                        NativeFileAccess.FILE_GENERIC_WRITE | NativeFileAccess.FILE_GENERIC_READ,
                        NativeFileShare.FILE_SHARE_WRITE | NativeFileShare.FILE_SHARE_READ,
                        IntPtr.Zero,
                        NativeFileMode.OPEN_EXISTING,
                        NativeFileFlag.FILE_ATTRIBUTE_NORMAL | NativeFileFlag.FILE_FLAG_OVERLAPPED,
                        IntPtr.Zero);
            //sfhDevice =
            //    Kernel32.CreateFile(DevicePath,
            //                        0,
            //                        NativeFileShare.FILE_SHARE_WRITE | NativeFileShare.FILE_SHARE_READ,
            //                        IntPtr.Zero,
            //                        NativeFileMode.OPEN_EXISTING,
            //                        0, //NativeFileFlag.FILE_ATTRIBUTE_NORMAL | NativeFileFlag.FILE_FLAG_OVERLAPPED,
            //                        IntPtr.Zero);

            //sfhDevice = FileIo.CreateFile(DevicePath, FileIo.GenericRead | FileIo.GenericWrite, FileIo.FileShareRead | FileIo.FileShareWrite, IntPtr.Zero, FileIo.OpenExisting, NativeFileFlag.FILE_ATTRIBUTE_NORMAL | NativeFileFlag.FILE_FLAG_OVERLAPPED, IntPtr.Zero);

            handle = sfhDevice;

            return (!sfhDevice.IsInvalid && !sfhDevice.IsClosed);
        }
    }
}
