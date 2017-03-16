using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using LibUsbDotNet.DeviceNotify;
using System.Collections.ObjectModel;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Treehopper.Desktop
{
    /// <summary>
    /// This class is used for discovering <see cref="TreehopperUsb"/> devices attached to this computer.
    /// </summary>
    public abstract class ConnectionService : IConnectionService
    {
        private static readonly ConnectionService winUsbInstance = new WinUsb.WinUsbConnectionService();
        //private static readonly ConnectionService libUsbInstance = new LibUsb.LibUsbConnectionService();

        /// <summary>
        /// Retrieve a reference to the static instance of the <see cref="ConnectionService"/> that should be used for discovering boards.
        /// </summary>
        /// <remarks>
        /// A single instance of <see cref="ConnectionService"/> is created and started upon the first reference to <see cref="Instance"/>.
        /// In general, there is no need to construct your own <see cref="ConnectionService"/>; just access <see cref="Instance"/> for any
        /// board discovery functionalities you need.
        /// </remarks>
        public static ConnectionService Instance
        {
            get {
                if(IsWindows)
                    return winUsbInstance;

                //if (IsLinux)
                //    return libUsbInstance;

                throw new Exception("Unsupported operating system");
            }
        }

        /// <summary>
        /// Determines if we're running under Windows
        /// </summary>
        public static bool IsWindows { get { return !IsLinux; } }

        /// <summary>
        /// Occurs when a <see cref="TreehopperBoard"/> is removed from the system.
        /// </summary>
        //public event BoardEventHandler BoardRemoved;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Enable the Serial Number filter.
        /// </summary>
        public bool SerialFilterIsEnabled { get; set; }

        /// <summary>
        /// Enable the Name filter.
        /// </summary>
        public bool NameFilterIsEnabled { get; set; }

        /// <summary>
        /// Gets or set the Serial Number filter.
        /// </summary>
        /// <remarks>
        /// This filter is only enabled when <see cref="SerialFilterIsEnabled"/> is set to true.
        /// </remarks>
        public string SerialFilter { get; set; }

        /// <summary>
        /// Gets or sets the Name filter.
        /// </summary>
        /// <remarks>
        /// This filter is only enabled when <see cref="NameFilterIsEnabled"/> is set to true.
        /// </remarks>
        public string NameFilter { get; set; }

        System.Timers.Timer pollingTimer;


        private TaskCompletionSource<TreehopperUsb> waitForFirstBoard = new TaskCompletionSource<TreehopperUsb>();

        /// <summary>
        /// Manages Treehopper boards connected to the computer. If optional filter parameters are provided, only boards matching the filter will be available.
        /// </summary>
        /// <param name="nameFilter">Name filter</param>
        /// <param name="serialFilter">Serial filter</param>
        public ConnectionService(string nameFilter = "", string serialFilter = "")
        {
            pollingTimer = new System.Timers.Timer();
            PollingTimerIsEnabled = false;
            PollingTimerInterval = 500;
            SerialFilterIsEnabled = serialFilter.Length > 0;
            NameFilterIsEnabled = nameFilter.Length > 0;
            SerialFilter = serialFilter;
            NameFilter = nameFilter;

            Boards.CollectionChanged += Boards_CollectionChanged;

            Rescan(); // add all the boards that were already connected when we started up

            // now, setup a device notifier so we can be alerted when boards are added/removed
            pollingTimer.Elapsed += devicePollingTimer_Elapsed;
        }

        private void Boards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if ((e.OldItems?.Count ?? 0) == 0 && e.NewItems.Count > 0)
                waitForFirstBoard.TrySetResult(Boards[0]);
        }

        private bool pollingTimerIsEnabled;

        /// <summary>
        /// Gets or sets a value controlling whether the polling timer is active or not.
        /// </summary>
        /// <remarks>
        /// <para>
        /// In order to avoid conflicts with the device notification system, the device notification system is automatically
        /// disabled when the polling timer is activated.
        /// </para>
        /// <para>
        /// Since the polling timer is inefficient compared to the device notification system, it is advisable to only use it
        /// when the device notification system doesn't work properly.
        /// </para>
        /// </remarks>
        public bool PollingTimerIsEnabled
        {
            get { return pollingTimerIsEnabled; }
            set { 
                pollingTimerIsEnabled = value;
                if (value)
                    pollingTimer.Start();
                else
                    pollingTimer.Stop();
            }
        }

        private int pollingTimerInterval;

        /// <summary>
        /// Control the interval used by the polling timer. The default is 100 milliseconds.
        /// </summary>
        public int PollingTimerInterval
        {
            get { return pollingTimerInterval; }
            set { 
                pollingTimerInterval = value;
                pollingTimer.Interval = value;
            }
        }

        /// <summary>
        /// Collection of <see cref="TreehopperUsb"/> devices attached to this computer
        /// </summary>
        public ObservableCollection<TreehopperUsb> Boards { get; } = new ObservableCollection<TreehopperUsb>();

        void devicePollingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Rescan();
        }

        protected abstract void Rescan();

        

        private bool PassesFilter(UsbConnection connection)
        {
            if (SerialFilterIsEnabled && connection.Serial != SerialFilter)
                return false;
            if (NameFilterIsEnabled && connection.Name != NameFilter)
                return false;
            return true;
        }

        /// <summary>
        /// Get a reference to the first device discovered.
        /// </summary>
        /// <returns>The first board found.</returns>
        /// <remarks>
        /// <para>
        /// If no devices have been plugged into the computer, 
        /// this call will await indefinitely until a board is plugged in.
        /// </para>
        /// 
        /// <para>
        /// Remember to call <see cref="TreehopperUsb.ConnectAsync()"/> before starting communication.
        /// </para>
        /// </remarks>
        public Task<TreehopperUsb> GetFirstDeviceAsync()
        {
            return waitForFirstBoard.Task;
        }

        private static bool mIsLinux;

        /// <summary>
        /// Determines if we're running under Linux, FreeBSD, or other UNIX-like OS (except macOS)
        /// </summary>
        public static bool IsLinux
        {
            get
            {
                if (ReferenceEquals(mIsLinux, null))
                {
                    switch (Environment.OSVersion.Platform.ToString())
                    {
                        case "Win32S":
                        case "Win32Windows":
                        case "Win32NT":
                        case "WinCE":
                        case "Xbox":
                            mIsLinux = false;
                            break;
                        case "Unix":
                            mIsLinux = true;
                            break;
                        default:
                            throw new NotSupportedException(string.Format("Operating System:{0} not supported.", Environment.OSVersion));
                    }
                }
                return (bool)mIsLinux;
            }
        }

        [DllImport("libc")]
        static extern int uname(IntPtr buf);

        static bool? isMac;

        /// <summary>
        /// Determines if we're running under macOS (OS X)
        /// </summary>
        public static bool IsMac
        {
            get
            {
                if (isMac == null)
                {
                    isMac = false;
                    IntPtr buf = IntPtr.Zero;
                    try
                    {
                        buf = Marshal.AllocHGlobal(8192);
                        // This is a hacktastic way of getting sysname from uname ()
                        if (uname(buf) == 0)
                        {
                            string os = Marshal.PtrToStringAnsi(buf);
                            if (os == "Darwin")
                                isMac = true;
                        }
                    }
                    catch
                    {

                    }
                    finally
                    {
                        if (buf != IntPtr.Zero)
                            Marshal.FreeHGlobal(buf);
                    }

                }
                return isMac.GetValueOrDefault();
            }
        }

        public void Dispose()
        {
            pollingTimer.Dispose();
        }
    }
}
