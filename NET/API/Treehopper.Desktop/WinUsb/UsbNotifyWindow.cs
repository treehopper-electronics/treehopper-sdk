using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using WinApi.Core;
using WinApi.Windows;
using WinApi.Windows.Controls;
using WinApi.User32;

namespace Treehopper.Desktop.WinUsb
{
    internal class UsbNotifyWindow : WindowCore, IConstructionParamsProvider
    {
        private const string WINDOW_CAPTION = "{18662f14-0871-455c-bf99-eff135425e3a}";
        private const int WM_DEVICECHANGE = 0x219;

        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;

        private const int DBT_DEVTYP_DEVICEINTERFACE = 0x00000005;
        private readonly DevBroadcastHdr mDevInterface = new DevBroadcastHdr();

        public WinUsbConnectionService Service { get; set; }
        private DevNotifySafeHandle mDevInterfaceHandle;

        public UsbNotifyWindow()
        {

        }

        internal UsbNotifyWindow(WinUsbConnectionService winUsbConnectionService)
        {
            Service = winUsbConnectionService;
        }

        protected override void OnMessage(ref WindowMessage msg)
        {
            switch(msg.Id)
            {
                
            }
            base.OnMessage(ref msg);
        }

        protected override IntPtr WindowProc(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == WM_DEVICECHANGE)
            {
                var theEvent = wParam.ToInt64();
                if (theEvent == DBT_DEVICEARRIVAL || theEvent == DBT_DEVICEREMOVECOMPLETE)
                {
                    var devBroadcastMsg = new DevBroadcastDeviceInterface();
                    Marshal.PtrToStructure(lParam, devBroadcastMsg);
                    if (devBroadcastMsg.DeviceType != 0x05)
                        return base.WindowProc(hwnd, msg, wParam, lParam);
                    if (theEvent == DBT_DEVICEARRIVAL)
                    {
                        Task.Run(() => { Service.add(devBroadcastMsg.DevicePath); });
                    }
                    else
                    {
                        var board = Service.Boards
                            .Where(x => x.Connection.DevicePath.Substring(3).ToLower() ==
                                        devBroadcastMsg.DevicePath.Substring(3).ToLower())
                            .FirstOrDefault();
                        if (board != null)
                        {
                            Debug.WriteLine("Removing: " + board);
                            board.Connection.Dispose(); // kill the connection first
                            board.Dispose();
                            Service.Boards.Remove(board);
                        }
                    }
                }
            }

            return base.WindowProc(hwnd, msg, wParam, lParam);
        }

        protected override void OnHandleChange()
        {
            if (mDevInterfaceHandle != null)
            {
                mDevInterfaceHandle.Dispose();
                mDevInterfaceHandle = null;
            }
            if (Handle != IntPtr.Zero)
            {
                mDevInterfaceHandle = NativeMethods.RegisterDeviceNotification(Handle, mDevInterface, 0);
                if (mDevInterfaceHandle == null || mDevInterfaceHandle.IsInvalid)
                {
                    Debug.WriteLine($"Device notification register failed with error {Marshal.GetLastWin32Error()}");
                }
            }
            base.OnHandleChange();
        }

        public IConstructionParams GetConstructionParams()
        {
            return new InvisibleWindowConstructionParams();
        }

        internal static class NativeMethods
        {
            [DllImport("user32.dll", SetLastError = true, EntryPoint = "RegisterDeviceNotificationA",
                CharSet = CharSet.Ansi)]
            internal static extern DevNotifySafeHandle RegisterDeviceNotification(
                IntPtr hRecipient,
                [MarshalAs(UnmanagedType.AsAny)] [In] object notificationFilter,
                int flags);

            [DllImport("user32.dll", SetLastError = true)]
            internal static extern bool UnregisterDeviceNotification(IntPtr handle);
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class DevBroadcastHdr
        {
            public int Size;
            public int DeviceType = DBT_DEVTYP_DEVICEINTERFACE;
            public int Reserved;
            public Guid UsbDevice = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED");
            private char mNameHolder;

            internal DevBroadcastHdr()
            {
                Size = Marshal.SizeOf(this);
            }
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class DevBroadcastDeviceInterface
        {
            public int Size;
            public int DeviceType;
            public int Reserved;
            public Guid ClassGuid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)] public string DevicePath;
        }

        internal class DevNotifySafeHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public DevNotifySafeHandle() : base(true)
            {
            }

            protected override bool ReleaseHandle()
            {
                if (handle != IntPtr.Zero)
                {
                    var bSuccess = NativeMethods.UnregisterDeviceNotification(handle);
                    handle = IntPtr.Zero;
                }

                return true;
            }
        }

        public class InvisibleWindowConstructionParams : ConstructionParams
        {
            public override WindowStyles Styles
                => WindowStyles.WS_DISABLED;

            public override WindowExStyles ExStyles
                => WindowExStyles.WS_EX_TRANSPARENT;

            public override int X => -50;
            public override int Y => -50;

            public override int Width => 0;
            public override int Height => 0;
        }
    }
}
