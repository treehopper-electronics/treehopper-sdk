using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32.SafeHandles;

/// <summary>
/// Win32-based Treehopper library for Windows desktop support
/// </summary>
namespace Treehopper.Desktop.WinUsb
{
    public class WinUsbConnectionService : ConnectionService
    {
        private UsbNotifyWindow mNotifyWindow;

        public WinUsbConnectionService()
        {
            // WMI queries take forever, so spin up a task to handle this so we don't block the app
            Task.Run(() => initialAdd());

            // We can only hear WM_DEVICECHANGE messages if we're an STA thread that's properly pumping windows messages.
            // There's no easy way to tell if the calling thread is pumping messages, so just check the apartment state, and assume people
            // aren't creating STA threads without a message pump.
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                mNotifyWindow = new UsbNotifyWindow(this);
            }
            else
            {
                // We're definitely not in an STA Thread (we're probably running in a console), so start a new STA Thread, and call
                // Application.Run() to start pumping windows messages.
                var staThread = new Thread(() =>
                {
                    mNotifyWindow = new UsbNotifyWindow(this);
                    Application.Run();
                });

                staThread.SetApartmentState(ApartmentState.STA);
                staThread.Name = "DevNotifyNativeWindow STA Thread";
                staThread.Start();
            }
        }

        private void initialAdd()
        {
            var query =
                $@"Select DeviceID, HardwareID, Name From Win32_PnPEntity WHERE DeviceID LIKE '%VID_{
                        TreehopperUsb.Settings.Vid
                    :X}&PID_{TreehopperUsb.Settings.Pid:X}%'";

            using (var searcher = new ManagementObjectSearcher(query))
            using (var collection = searcher.Get())
            {
                foreach (var device in collection)
                {
                    var deviceID = (string) device.GetPropertyValue("DeviceID");
                    var elements = deviceID.Split('\\');

                    var serial = elements[2].ToLower();

                    var hardwareIds = ((string[]) device.GetPropertyValue("HardwareID"))[0].Split('&');
                    var version = hardwareIds[2].Substring(4);

                    var name = (string) device.GetPropertyValue("Name");
                    var path =
                        $@"\\.\usb#vid_{TreehopperUsb.Settings.Vid:x}&pid_{TreehopperUsb.Settings.Pid:x}#{
                                serial
                            }#{{a5dcbf10-6530-11d2-901f-00c04fb951ed}}";

                    Boards.Add(new TreehopperUsb(new WinUsbConnection(path, name, serial, short.Parse(version))));
                }
            }
        }

        private void addBoardFromPath(string path)
        {
            var items = path.ToLower().Split('#');
            var serial = items[2];
            var name = "";
            var version = "";

            var query =
                $@"SELECT Name, DeviceID, HardwareID FROM Win32_PnPEntity WHERE PNPClass = 'USBDevice' AND DeviceID LIKE '%{
                        serial.ToUpper()
                    }%'";

            using (var searcher = new ManagementObjectSearcher(query))
            using (var collection = searcher.Get())
            {
                foreach (var device in collection)
                {
                    name = (string) device.GetPropertyValue("Name");
                    var hardwareIds = ((string[]) device.GetPropertyValue("HardwareID"))[0].Split('&');
                    version = hardwareIds[2].Substring(4);

                    var newBoard = new TreehopperUsb(
                        new WinUsbConnection(path.ToLower(), name, serial, short.Parse(version)));
                    Debug.WriteLine("Adding: " + newBoard);
                    Boards.Add(newBoard);
                }
            }
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        internal sealed class UsbNotifyWindow : NativeWindow
        {
            private const string WINDOW_CAPTION = "{18662f14-0871-455c-bf99-eff135425e3a}";
            private const int WM_DEVICECHANGE = 0x219;

            private const int DBT_DEVICEARRIVAL = 0x8000;
            private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;

            private const int DBT_DEVTYP_DEVICEINTERFACE = 0x00000005;
            private readonly DevBroadcastHdr mDevInterface = new DevBroadcastHdr();

            private readonly WinUsbConnectionService service;
            private DevNotifySafeHandle mDevInterfaceHandle;

            internal UsbNotifyWindow(WinUsbConnectionService winUsbConnectionService)
            {
                var cp = new CreateParams();
                cp.Caption = WINDOW_CAPTION;
                cp.X = -100;
                cp.Y = -100;
                cp.Width = 50;
                cp.Height = 50;
                CreateHandle(cp);
                service = winUsbConnectionService;
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

            [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_DEVICECHANGE)
                {
                    var theEvent = m.WParam.ToInt64();
                    if (theEvent == DBT_DEVICEARRIVAL || theEvent == DBT_DEVICEREMOVECOMPLETE)
                    {
                        var msg = new DevBroadcastDeviceInterface();
                        Marshal.PtrToStructure(m.LParam, msg);
                        if (msg.DeviceType != 0x05)
                            return;
                        if (theEvent == DBT_DEVICEARRIVAL)
                        {
                            Task.Run(() => { service.addBoardFromPath(msg.DevicePath); });
                        }
                        else
                        {
                            var board = service.Boards
                                .Where(x => x.Connection.DevicePath.Substring(3).ToLower() ==
                                            msg.DevicePath.Substring(3).ToLower())
                                .FirstOrDefault();
                            if (board != null)
                            {
                                Debug.WriteLine("Removing: " + board);
                                board.Connection.Dispose(); // kill the connection first
                                board.Dispose();
                                service.Boards.Remove(board);
                            }
                        }
                    }
                }

                base.WndProc(ref m);
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
                public Guid ClassGuid = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED");
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
        }
    }
}