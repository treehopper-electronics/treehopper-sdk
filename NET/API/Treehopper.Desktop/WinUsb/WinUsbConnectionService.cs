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
using Microsoft.Win32.SafeHandles;
using Treehopper.Desktop.WinUsb;
using WinApi.Windows;
using WinApi.User32;

/// <summary>
/// Win32-based Treehopper library for Windows desktop support
/// </summary>
namespace Treehopper.Desktop.WinUsb
{
    public class WinUsbConnectionService : ConnectionService
    {
        private UsbNotifyWindow mNotifyWindow;
        private SynchronizationContext currentContext;

        public WinUsbConnectionService()
        {
            currentContext = SynchronizationContext.Current;
            add($"vid_{TreehopperUsb.Settings.Vid:x}&pid_{TreehopperUsb.Settings.Pid:x}");
            var wf = WindowFactory.Create();
            // We can only hear WM_DEVICECHANGE messages if we're an STA thread that's properly pumping windows messages.
            // There's no easy way to tell if the calling thread is pumping messages, so just check the apartment state, and assume people
            // aren't creating STA threads without a message pump.
            var mNotifyWindow = wf.CreateWindowEx(() => new UsbNotifyWindow(this), null);
            var staThread = new Thread(() =>
            {
                mNotifyWindow.Show();
                new EventLoop().Run(mNotifyWindow);
            })
            {
                //staThread.SetApartmentState(ApartmentState.STA);
                Name = "DevNotifyNativeWindow STA Thread"
            };
            staThread.Start();
        }

        internal void add(string matchDevicePath)
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
                    var path = deviceInterfaceDetailData.DevicePath;

                    var newBoard = new TreehopperUsb(new WinUsbConnection(path, name, serial, ushort.Parse(revisionString)));

                    Debug.WriteLine("Adding: " + newBoard);

                    if (currentContext == null)
                        Boards.Add(newBoard);
                    else
                        currentContext.Post(
                            delegate {
                                Boards.Add(newBoard);
                            }, null);
                    
                }
            }
            SetupApi.SetupDiDestroyDeviceInfoList(devInfo);
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}