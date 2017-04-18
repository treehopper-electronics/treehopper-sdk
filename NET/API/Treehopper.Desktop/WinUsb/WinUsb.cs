using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Treehopper.Desktop.WinUsb
{
    internal class WinUsb
    {
        internal static class NativeMethods
        {
            internal const string WIN_USB_DLL = "winusb.dll";

            [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_AbortPipe", SetLastError = true)]
            internal static extern bool WinUsb_AbortPipe([In] SafeHandle InterfaceHandle, byte PipeID);

            //[DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_ControlTransfer", SetLastError = true)]
            //private static extern bool WinUsb_ControlTransfer([In] SafeHandle InterfaceHandle,
            //                                                  [In] UsbSetupPacket SetupPacket,
            //                                                  IntPtr Buffer,
            //                                                  int BufferLength,
            //                                                  out int LengthTransferred,
            //                                                  IntPtr pOVERLAPPED);

            [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_FlushPipe", SetLastError = true)]
            internal static extern bool WinUsb_FlushPipe([In] SafeHandle InterfaceHandle, byte PipeID);

            [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_Free", SetLastError = true)]
            internal static extern bool WinUsb_Free([In] IntPtr InterfaceHandle);

            [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_GetAssociatedInterface", SetLastError = true)]
            internal static extern bool WinUsb_GetAssociatedInterface([In] SafeHandle InterfaceHandle,
                byte AssociatedInterfaceIndex,
                ref IntPtr AssociatedInterfaceHandle);

            [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_GetCurrentAlternateSetting", SetLastError = true)]
            internal static extern bool WinUsb_GetCurrentAlternateSetting([In] SafeHandle InterfaceHandle,
                out byte SettingNumber);


            [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_GetDescriptor", SetLastError = true)]
            internal static extern bool WinUsb_GetDescriptor([In] SafeHandle InterfaceHandle,
                byte DescriptorType,
                byte Index,
                ushort LanguageID,
                IntPtr Buffer,
                int BufferLength,
                out int LengthTransferred);

            [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_GetOverlappedResult", SetLastError = true)]
            internal static extern bool WinUsb_GetOverlappedResult([In] SafeHandle InterfaceHandle,
                IntPtr pOVERLAPPED,
                out int lpNumberOfBytesTransferred,
                bool Wait);

            //[DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_GetPipePolicy", SetLastError = true)]
            //internal static extern bool WinUsb_GetPipePolicy([In] SafeHandle InterfaceHandle,
            //                                                 byte PipeID,
            //                                                 PipePolicyType policyType,
            //                                                 ref int ValueLength,
            //                                                 IntPtr Value);

            //[DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_GetPowerPolicy", SetLastError = true)]
            //internal static extern bool WinUsb_GetPowerPolicy([In] SafeHandle InterfaceHandle,
            //                                                  PowerPolicyType policyType,
            //                                                  ref int ValueLength,
            //                                                  IntPtr Value);

            [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_Initialize", SetLastError = true)]
            internal static extern bool WinUsb_Initialize([In] SafeHandle DeviceHandle,
                [Out] [In] ref SafeWinUsbHandle InterfaceHandle);

            //[DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_QueryDeviceInformation", SetLastError = true)]
            //internal static extern bool WinUsb_QueryDeviceInformation([In] SafeHandle InterfaceHandle,
            //                                                          DeviceInformationTypes InformationType,
            //                                                          ref int BufferLength,
            //                                                          [MarshalAs(UnmanagedType.AsAny), In, Out] object Buffer);

            //[DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_QueryInterfaceSettings", SetLastError = true)]
            //internal static extern bool WinUsb_QueryInterfaceSettings([In] SafeHandle InterfaceHandle,
            //                                                          byte AlternateInterfaceNumber,
            //                                                          [MarshalAs(UnmanagedType.LPStruct), In, Out] UsbInterfaceDescriptor
            //                                                              UsbAltInterfaceDescriptor);

            //[DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_QueryPipe", SetLastError = true)]
            //internal static extern bool WinUsb_QueryPipe([In] SafeHandle InterfaceHandle,
            //                                             byte AlternateInterfaceNumber,
            //                                             byte PipeIndex,
            //                                             [MarshalAs(UnmanagedType.LPStruct), In, Out] PipeInformation PipeInformation);

            [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_ReadPipe", SetLastError = true)]
            internal static extern bool WinUsb_ReadPipe([In] SafeHandle InterfaceHandle,
                byte PipeID,
                byte[] Buffer,
                int BufferLength,
                out int LengthTransferred,
                IntPtr pOVERLAPPED);

            [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_ReadPipe", SetLastError = true)]
            internal static extern bool WinUsb_ReadPipe([In] SafeHandle InterfaceHandle,
                byte PipeID,
                IntPtr pBuffer,
                int BufferLength,
                out int LengthTransferred,
                IntPtr pOVERLAPPED);

            [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_ResetPipe", SetLastError = true)]
            internal static extern bool WinUsb_ResetPipe([In] SafeHandle InterfaceHandle, byte PipeID);

            //[DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_SetPipePolicy", SetLastError = true)]
            //internal static extern bool WinUsb_SetPipePolicy([In] SafeHandle InterfaceHandle,
            //                                                 byte PipeID,
            //                                                 PipePolicyType policyType,
            //                                                 int ValueLength,
            //                                                 IntPtr Value);

            //[DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_SetPowerPolicy", SetLastError = true)]
            //internal static extern bool WinUsb_SetPowerPolicy([In] SafeHandle InterfaceHandle, PowerPolicyType policyType, int ValueLength, IntPtr Value);

            [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_WritePipe", SetLastError = true)]
            internal static extern unsafe bool WinUsb_WritePipe([In] SafeHandle InterfaceHandle,
                byte PipeID,
                byte[] Buffer,
                int BufferLength,
                out int LengthTransferred,
                NativeOverlapped* pOVERLAPPED);

            [DllImport(WIN_USB_DLL, EntryPoint = "WinUsb_WritePipe", SetLastError = true)]
            internal static extern bool WinUsb_WritePipe([In] SafeHandle InterfaceHandle,
                byte PipeID,
                byte[] pBuffer,
                int BufferLength,
                out int LengthTransferred,
                IntPtr pOVERLAPPED);
        }
    }
}