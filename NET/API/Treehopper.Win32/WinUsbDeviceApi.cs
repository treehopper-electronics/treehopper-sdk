using System;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace Treehopper
{
    /// <summary>
    ///  These declarations are translated from the C declarations in various files
    ///  in the WDK:
    ///  
    ///  usb.h
    ///  usb100.h
    ///  winusbio.h	
    /// </summary>

    sealed internal partial class WinUsbCommunications
    {
        internal static class NativeMethods
        {
            internal const UInt32 DEVICE_SPEED = 1;
            internal const Byte USB_ENDPOINT_DIRECTION_MASK = 0X80;

            internal enum POLICY_TYPE
            {
                SHORT_PACKET_TERMINATE = 1,
                AUTO_CLEAR_STALL,
                PIPE_TRANSFER_TIMEOUT,
                IGNORE_SHORT_PACKETS,
                ALLOW_PARTIAL_READS,
                AUTO_FLUSH,
                RAW_IO,
            }

            internal enum USBD_PIPE_TYPE
            {
                UsbdPipeTypeControl,
                UsbdPipeTypeIsochronous,
                UsbdPipeTypeBulk,
                UsbdPipeTypeInterrupt,
            }

            internal enum USB_DEVICE_SPEED
            {
                UsbLowSpeed = 1,
                UsbFullSpeed,
                UsbHighSpeed,
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct USB_CONFIGURATION_DESCRIPTOR
            {
                internal Byte bLength;
                internal Byte bDescriptorType;
                internal UInt16 wTotalLength;
                internal Byte bNumInterfaces;
                internal Byte bConfigurationValue;
                internal Byte iConfiguration;
                internal Byte bmAttributes;
                internal Byte MaxPower;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct USB_INTERFACE_DESCRIPTOR
            {
                internal Byte bLength;
                internal Byte bDescriptorType;
                internal Byte bInterfaceNumber;
                internal Byte bAlternateSetting;
                internal Byte bNumEndpoints;
                internal Byte bInterfaceClass;
                internal Byte bInterfaceSubClass;
                internal Byte bInterfaceProtocol;
                internal Byte iInterface;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct WINUSB_PIPE_INFORMATION
            {
                internal USBD_PIPE_TYPE PipeType;
                internal Byte PipeId;
                internal UInt16 MaximumPacketSize;
                internal Byte Interval;
            }

            [StructLayout(LayoutKind.Sequential)]
            internal struct USBD_ISO_PACKET_DESCRIPTOR
            {
                internal UInt32 Offset;
                internal UInt32 Length;
                internal UInt32 Status;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            internal struct WINUSB_SETUP_PACKET
            {
                internal Byte RequestType;
                internal Byte Request;
                internal UInt16 Value;
                internal UInt16 Index;
                internal UInt16 Length;
            }

            [DllImport("winusb.dll", SetLastError = true)]
            internal static extern Boolean WinUsb_ControlTransfer(SafeWinUsbHandle InterfaceHandle, WINUSB_SETUP_PACKET SetupPacket, Byte[] Buffer, UInt32 BufferLength, ref UInt32 LengthTransferred, IntPtr Overlapped);

            [DllImport("winusb.dll", SetLastError = true)]
            internal static extern Boolean WinUsb_Free(IntPtr InterfaceHandle);

            [DllImport("winusb.dll", SetLastError = true)]
            internal static extern Boolean WinUsb_Initialize(SafeFileHandle DeviceHandle, ref SafeWinUsbHandle InterfaceHandle);

            [DllImport("winusb.dll", EntryPoint = "WinUsb_GetDescriptor", SetLastError = true)]
            internal static extern bool WinUsb_GetDescriptor(SafeWinUsbHandle InterfaceHandle,
                                                byte DescriptorType,
                                                byte Index,
                                                ushort LanguageID,
                                                IntPtr Buffer,
                                                int BufferLength,
                                                out int LengthTransferred);

            //  Use this declaration to retrieve DEVICE_SPEED (the only currently defined InformationType).

            [DllImport("winusb.dll", SetLastError = true)]
            internal static extern Boolean WinUsb_QueryDeviceInformation(SafeWinUsbHandle InterfaceHandle, UInt32 InformationType, ref UInt32 BufferLength, ref Byte Buffer);

            [DllImport("winusb.dll", SetLastError = true)]
            internal static extern Boolean WinUsb_QueryInterfaceSettings(SafeWinUsbHandle InterfaceHandle, Byte AlternateInterfaceNumber, ref USB_INTERFACE_DESCRIPTOR UsbAltInterfaceDescriptor);

            [DllImport("winusb.dll", SetLastError = true)]
            internal static extern Boolean WinUsb_QueryPipe(SafeWinUsbHandle InterfaceHandle, Byte AlternateInterfaceNumber, Byte PipeIndex, ref WINUSB_PIPE_INFORMATION PipeInformation);

            [DllImport("winusb.dll", SetLastError = true)]
            internal static extern Boolean WinUsb_ReadIsochPipeAsap(IntPtr BufferHandle, UInt32 Offset, UInt32 Length, Boolean ContinueStream, UInt32 NumberOfPackets, ref USBD_ISO_PACKET_DESCRIPTOR IsoPacketDescriptors, IntPtr Overlapped);

            [DllImport("winusb.dll", SetLastError = true)]
            internal static extern Boolean WinUsb_ReadPipe(SafeWinUsbHandle InterfaceHandle, Byte PipeID, Byte[] Buffer, UInt32 BufferLength, ref UInt32 LengthTransferred, IntPtr Overlapped);

            [DllImport("winusb.dll", SetLastError = true)]
            internal static extern Boolean WinUsb_RegisterIsochBuffer(SafeWinUsbHandle InterfaceHandle, Byte PipeID, Byte[] Buffer, UInt32 BufferLength, out IntPtr BufferHandle);

            [DllImport("winusb.dll", SetLastError = true)]
            internal static extern Boolean WinUsb_SetCurrentAlternateSetting(SafeWinUsbHandle InterfaceHandle, Byte AlternateSetting);


            //  Two declarations for WinUsb_SetPipePolicy. 
            //  Use this one when the returned Value is a Byte (all except PIPE_TRANSFER_TIMEOUT):

            [DllImport("winusb.dll", SetLastError = true)]
            internal static extern Boolean WinUsb_SetPipePolicy(SafeWinUsbHandle InterfaceHandle, Byte PipeID, UInt32 PolicyType, UInt32 ValueLength, ref Byte Value);

            //  Use this alias when the returned Value is a UInt32 (PIPE_TRANSFER_TIMEOUT only):

            [DllImport("winusb.dll", SetLastError = true, EntryPoint = "WinUsb_SetPipePolicy")]
            internal static extern Boolean WinUsb_SetPipePolicy1(SafeWinUsbHandle InterfaceHandle, Byte PipeID, UInt32 PolicyType, UInt32 ValueLength, ref UInt32 Value);

            [DllImport("winusb.dll", SetLastError = true)]
            internal static extern Boolean WinUsb_UnregisterIsochBuffer(IntPtr BufferHandle);

            [DllImport("winusb.dll", SetLastError = true)]
            internal static extern Boolean WinUsb_WriteIsochPipeAsap(IntPtr BufferHandle, UInt32 Offset, UInt32 Length, Boolean ContinueStream, IntPtr Overlapped);

            [DllImport("winusb.dll", SetLastError = true)]
            internal static extern Boolean WinUsb_WritePipe(SafeWinUsbHandle InterfaceHandle, Byte PipeID, Byte[] Buffer, UInt32 BufferLength, ref UInt32 LengthTransferred, IntPtr Overlapped);
        }
    }
}
