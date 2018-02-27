using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Treehopper.Desktop.LibUsb;
using Treehopper.Desktop.MacUsb;
using Treehopper.Desktop.WinUsb;

namespace Treehopper
{
    /// <summary>
    /// Win32, Mono, and IOKit implementations for running Treehopper on Windows, macOS, and Linux
    /// </summary>
    namespace Desktop
    {
        /// Dummy namespace just for nicer documentation
    }

    public abstract class ConnectionService : IConnectionService, IDisposable
    {
        private static readonly Lazy<WinUsbConnectionService> winUsbInstance = new Lazy<WinUsbConnectionService>();
        private static readonly Lazy<LibUsbConnectionService> libUsbInstance = new Lazy<LibUsbConnectionService>();
        private static readonly Lazy<MacUsbConnectionService> macUsbInstance = new Lazy<MacUsbConnectionService>();

        private static bool? isLinux;

        private static bool? isWindows;

        private static bool? isMac;

        private TaskCompletionSource<TreehopperUsb> waitForFirstBoard = new TaskCompletionSource<TreehopperUsb>();

        public ConnectionService()
        {
            Boards.CollectionChanged += Boards_CollectionChanged;
        }

        /// <summary>
        ///     The singleton instance through which to access ConnectionService.
        /// </summary>
        /// <remarks>
        ///     This instance is created and started upon the first reference to a property or method
        ///     on this object. This typically only becomes an issue if you expect to have debug messages
        ///     from ConnectionService printing even if you haven't actually accessed the object yet.
        /// </remarks>
        public static ConnectionService Instance
        {
            get
            {
                if (IsWindows)
                    return winUsbInstance.Value;

                if (IsMac)
                    return macUsbInstance.Value;

                if (IsLinux)
                    return libUsbInstance.Value;

                throw new Exception("Unsupported operating system");
            }
        }

        /// <summary>
        /// [Treehopper.Desktop.dll] Determines if we're running under Linux, FreeBSD, or other UNIX-like OS (except macOS)
        /// </summary>
        public static bool IsWindows
        {
            get
            {
                if (isWindows == null)
                    if (Environment.OSVersion.Platform.ToString() != "Unix")
                        isWindows = true;
                    else
                        isWindows = false;
                return (bool) isWindows;
            }
        }

        /// <summary>
        /// [Treehopper.Desktop.dll] Determines if we're running under Linux, FreeBSD, or other UNIX-like OS (except macOS)
        /// </summary>
        public static bool IsLinux
        {
            get
            {
                if (isLinux == null)
                    if (Environment.OSVersion.Platform.ToString() == "Unix" && !IsMac)
                        isLinux = true;
                    else
                        isLinux = false;
                return (bool) isLinux;
            }
        }

        /// <summary>
        /// [Treehopper.Desktop.dll] Determines if we're running under macOS (OS X)
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
                        var buf = IntPtr.Zero;
                        try
                        {
                            buf = Marshal.AllocHGlobal(8192);
                            // This is a hacktastic way of getting sysname from uname ()
                            if (uname(buf) == 0)
                            {
                                var os = Marshal.PtrToStringAnsi(buf);
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

        /// <summary>
        /// The Treehopper boards attached to the computer.
        /// </summary>
        public ObservableCollection<TreehopperUsb> Boards { get; } = new ObservableCollection<TreehopperUsb>();


        /// <summary>
        ///     Get a reference to the first device discovered.
        /// </summary>
        /// <returns>The first board found.</returns>
        /// <remarks>
        ///     <para>
        ///         If no devices have been plugged into the computer,
        ///         this call will await indefinitely until a board is plugged in.
        ///     </para>
        /// </remarks>
        public Task<TreehopperUsb> GetFirstDeviceAsync()
        {
            return waitForFirstBoard.Task;
        }

        public abstract void Dispose();

        private void Boards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Boards.Count == 0)
                waitForFirstBoard = new TaskCompletionSource<TreehopperUsb>();

            else if ((e.OldItems?.Count ?? 0) == 0 && e.NewItems.Count > 0)
                waitForFirstBoard.TrySetResult(Boards[0]);
        }

        [DllImport("libc")]
        private static extern int uname(IntPtr buf);
    }
}