// Portions of this file inspired by https://github.com/jcoenraadts/hid-sharp
// MIT license

using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32.SafeHandles;
using Treehopper.Desktop.WinUsb;

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
            add($"vid_{TreehopperUsb.Settings.Vid:x}&pid_{TreehopperUsb.Settings.Pid:x}");

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

        private void add(string matchDevicePath)
        {
            //Create structs to hold interface information
            SP_DEVINFO_DATA devInfoData = new SP_DEVINFO_DATA();
            SP_DEVICE_INTERFACE_DATA devInterfaceData = new SP_DEVICE_INTERFACE_DATA();
            devInfoData.cbSize = (uint)Marshal.SizeOf(devInfoData);
            devInterfaceData.cbSize = (uint)(Marshal.SizeOf(devInterfaceData));

            Guid G = new Guid("{a5dcbf10-6530-11d2-901f-00c04fb951ed}");
            IntPtr devInfo = SetupApi.SetupDiGetClassDevs(ref G, IntPtr.Zero, IntPtr.Zero, SetupApi.DIGCF_DEVICEINTERFACE | SetupApi.DIGCF_PRESENT);

            //Loop through all available entries in the device list, until false
            SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData = new SP_DEVICE_INTERFACE_DETAIL_DATA();
            if (IntPtr.Size == 8) // for 64 bit operating systems
                deviceInterfaceDetailData.cbSize = 8;
            else
                deviceInterfaceDetailData.cbSize = 4 + Marshal.SystemDefaultCharSize; // for 32 bit systems

            int j = -1;
            bool b = true;
            int error;
            SafeFileHandle tempHandle;

            while (b)
            {
                j++;
                b = SetupApi.SetupDiEnumDeviceInterfaces(devInfo, IntPtr.Zero, ref G, (uint)j, ref devInterfaceData);
                error = Marshal.GetLastWin32Error();
                if (b == false)
                    break;

                uint size = 0;
                SetupApi.SetupDiGetDeviceInterfaceDetail(devInfo, ref devInterfaceData, ref deviceInterfaceDetailData, 255, out size, ref devInfoData);
                if(deviceInterfaceDetailData.DevicePath.ToLower().Contains(matchDevicePath.ToLower()))
                {
                    ulong devPropType = 0;
                    StringBuilder sb = new StringBuilder(256);
                    int actualSize = 0;
                    SetupApi.SetupDiGetDevicePropertyW(devInfo, ref devInfoData, ref SetupApi.FriendlyName, ref devPropType, sb, 256, out actualSize, 0);
                    string name = sb.ToString();
                    SetupApi.SetupDiGetDevicePropertyW(devInfo, ref devInfoData, ref SetupApi.HardwareIds, ref devPropType, sb, 256, out actualSize, 0);
                    string hardware = sb.ToString();
                    var refStart = hardware.IndexOf("REV_");
                    var revisionString = hardware.Substring(refStart+4, 4);
                    var serial = deviceInterfaceDetailData.DevicePath.Split('#')[2];
                    Boards.Add(new TreehopperUsb(new WinUsbConnection(deviceInterfaceDetailData.DevicePath, name, serial, short.Parse(revisionString))));
                }
            }
            SetupApi.SetupDiDestroyDeviceInfoList(devInfo);
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
                            Task.Run(() => { service.add(msg.DevicePath); });
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
        }
    }
}