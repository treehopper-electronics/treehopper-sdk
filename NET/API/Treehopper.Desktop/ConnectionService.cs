using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Treehopper.Desktop.WinUsb;
using Treehopper.Desktop.LibUsb;
using Treehopper.Desktop.MacUsb;

namespace Treehopper.Desktop
{
    /// <summary>
    /// This class is used for discovering <see cref="TreehopperUsb"/> devices attached to this computer.
    /// </summary>
	public abstract class ConnectionService : IConnectionService, IDisposable
    {
        private static Lazy<WinUsbConnectionService> winUsbInstance = new Lazy<WinUsbConnectionService>();
        private static Lazy<LibUsbConnectionService> libUsbInstance = new Lazy<LibUsbConnectionService>();
		private static Lazy<MacUsbConnectionService> macUsbInstance = new Lazy<MacUsbConnectionService>();

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
                    return winUsbInstance.Value;

				if (IsMac)
					return macUsbInstance.Value;

                if (IsLinux)
                    return libUsbInstance.Value;

                throw new Exception("Unsupported operating system");
            }
        }

        /// <summary>
        /// Occurs when a <see cref="TreehopperBoard"/> is removed from the system.
        /// </summary>
        //public event BoardEventHandler BoardRemoved;
        public event PropertyChangedEventHandler PropertyChanged;

        private TaskCompletionSource<TreehopperUsb> waitForFirstBoard = new TaskCompletionSource<TreehopperUsb>();

        /// <summary>
        /// Manages Treehopper boards connected to the computer. If optional filter parameters are provided, only boards matching the filter will be available.
        /// </summary>
        /// <param name="nameFilter">Name filter</param>
        /// <param name="serialFilter">Serial filter</param>
        public ConnectionService(string nameFilter = "", string serialFilter = "")
        {
            Boards.CollectionChanged += Boards_CollectionChanged;
        }

        private void Boards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
			if (Boards.Count == 0)
				waitForFirstBoard = new TaskCompletionSource<TreehopperUsb> ();
			
            else if ((e.OldItems?.Count ?? 0) == 0 && e.NewItems.Count > 0)
                waitForFirstBoard.TrySetResult(Boards[0]);
			
        }

        /// <summary>
        /// Collection of <see cref="TreehopperUsb"/> devices attached to this computer
        /// </summary>
        public ObservableCollection<TreehopperUsb> Boards { get; } = new ObservableCollection<TreehopperUsb>();
			

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

		private static bool? isLinux;

		static bool? isWindows;

		static bool? isMac;

		/// <summary>
		/// Determines if we're running under Linux, FreeBSD, or other UNIX-like OS (except macOS)
		/// </summary>
		public static bool IsWindows
		{
			get
			{
				if (isWindows == null)
				{
					if (Environment.OSVersion.Platform.ToString() != "Unix")
					{
						isWindows = true;
					}
					else
					{
						isWindows = false;
					}
				}
				return (bool)isWindows;
			}
		}

        /// <summary>
        /// Determines if we're running under Linux, FreeBSD, or other UNIX-like OS (except macOS)
        /// </summary>
        public static bool IsLinux
        {
            get
            {
                if (isLinux == null)
                {
					if (Environment.OSVersion.Platform.ToString() == "Unix" && !IsMac)
					{
						isLinux = true;
					}
					else
					{
						isLinux = false;
					}
                }
                return (bool)isLinux;
            }
        }

        [DllImport("libc")]
        static extern int uname(IntPtr buf);

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
					if (Environment.OSVersion.Platform.ToString() == "Unix")
					{
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

                }
                return isMac.GetValueOrDefault();
            }
        }

        public virtual void Dispose()
        {

        }
    }
}
