namespace Treehopper.Desktop.WinUsb
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Management;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    
    public class WinUsbConnectionService : ConnectionService
    {
        private DevNotifyNativeWindow mNotifyWindow;

        public WinUsbConnectionService() : base()
        {
            var query = string.Format(@"Select DeviceID, HardwareID, Name From Win32_PnPEntity WHERE PNPClass = 'USBDevice' AND DeviceID LIKE '%VID_{0:X}&PID_{1:X}%'",
                TreehopperUsb.Settings.Vid,
                TreehopperUsb.Settings.Pid);

            addBoards(query);

            // We can only hear WM_DEVICECHANGE messages if we're an STA thread that's properly pumping windows messages.
            // There's no easy way to tell if the calling thread is pumping messages, so just check the apartment state, and assume people
            // aren't creating STA threads without a message pump.
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                mNotifyWindow = new DevNotifyNativeWindow(this);
            }
            else
            {
                // We're definitely not in an STA Thread (we're probably running in a console), so start a new STA Thread, and call
                // Application.Run() to start pumping windows messages.
                Thread staThread = new Thread(new ThreadStart(() =>
                {
                    mNotifyWindow = new DevNotifyNativeWindow(this);
                    Application.Run();
                }));

                staThread.SetApartmentState(ApartmentState.STA);
                staThread.Name = "DevNotifyNativeWindow STA Thread";
                staThread.Start();
            }
        }


        protected override void Rescan()
        {
            Debug.WriteLine("Device change");
        }

        void addBoards(string query)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            using (var searcher = new ManagementObjectSearcher(query))
            {
                using (var collection = searcher.Get())
                {
                    foreach (var device in collection)
                    {
                        string deviceID = (string)device.GetPropertyValue("DeviceID");
                        var elements = deviceID.Split('\\');

                        string serial = elements[2].ToLower();
                        if (Boards.Where(b => b.SerialNumber == serial).Count() > 0) // we already have the board
                            break;
                        var hardwareIds = ((string[])device.GetPropertyValue("HardwareID"))[0].Split('&');
                        var version = hardwareIds[2].Substring(4);

                        string name = (string)device.GetPropertyValue("Name");
                        string path = string.Format(@"\\.\usb#vid_{0:x}&pid_{1:x}#{2}#{{a5dcbf10-6530-11d2-901f-00c04fb951ed}}", TreehopperUsb.Settings.Vid, TreehopperUsb.Settings.Pid, serial);

                        Boards.Add(new TreehopperUsb(new WinUsbConnection(path, name, serial, short.Parse(version))));
                    }
                }
            }

            sw.Stop();
            double val = sw.Elapsed.TotalMilliseconds;
        }

        internal sealed class DevNotifyNativeWindow : NativeWindow
        {
            private const string WINDOW_CAPTION = "{18662f14-0871-455c-bf99-eff135425e3a}";
            private const int WM_DEVICECHANGE = 0x219;
            private WinUsbConnectionService winUsbConnectionService;

            internal DevNotifyNativeWindow(WinUsbConnectionService winUsbConnectionService)
            {
                CreateParams cp = new CreateParams();
                cp.Caption = WINDOW_CAPTION;
                cp.X = -100;
                cp.Y = -100;
                cp.Width = 50;
                cp.Height = 50;
                CreateHandle(cp);
                this.winUsbConnectionService = winUsbConnectionService;
            }

            protected override void OnHandleChange()
            {
                //mDelHandleChanged(Handle);
                base.OnHandleChange();
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_DEVICECHANGE)
                {
                    winUsbConnectionService.Rescan();
                }
                base.WndProc(ref m);
            }
        }
    }
}

