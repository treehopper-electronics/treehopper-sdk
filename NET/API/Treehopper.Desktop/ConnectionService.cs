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

/** Discovers %Treehopper boards attached to your device.
This documentation set covers the %ConnectionService class found in **Treehopper.Desktop**, **Treehopper.Android**, and **Treehopper.Uwp** packages. 

\note %ConnectionService should always be accessed through its singleton property, ConnectionService.Instance. Do not create instances of %ConnectionService yourself.

## Basic usage
When running in .NET Core, .NET Framework, Mono, or Windows 10 UWP, no additional configuration is needed.

There are two ways to access discovered boards. If you simply want to wait until the first Treehopper board is attached
to the computer, the GetFirstDeviceAsync() method will return an awaitable task with a result that contains the board:

```
var board = await ConnectionService.Instance.GetFirstDeviceAsync();
```

## Advanced usage
For simple applications, you can retrieve a board instance with GetFirstDeviceAsync(), however, if you'd like to present the user with a list of devices from which to choose, you can reference the #Boards property.

\warning Even if you already have a board connected, the #Boards collection is not guaranteed to have a board populated on first invocation. Board discovery on many platforms is done asynchronously, so you should always bind to the [CollectionChanged](https://docs.microsoft.com/en-us/dotnet/api/system.collections.objectmodel.observablecollection-1.collectionchanged?view=netframework-4.7.1) event of this collection to get notified when boards are added.

GUI apps or more advanced console apps can query or bind directly to the #Boards property, which is an ObservableCollection that can notify when boards are attached or removed:

```
ConnectionService.Instance.Boards.CollectionChanged += async(o, e) => {
        if(e.NewItems.Count > 0) // a new board was added
            RunApp((TreehopperUsb)e.NewItems[0])
    };

// if we already have a board in the collection, start the app
if(ConnectionService.Instance.Boards.Count > 0)
    RunApp(ConnectionService.Instance.Boards[0]);

```

\warning After subscribing to CollectionChanged, you should always check the collection. A board may have already been added before you subscribed to the CollectionChanged event, and the event will not re-fire for new subscribers.

## Xamarin.Android

To integrate Treehopper into a Xamarin Android-based project, you must integrate %ConnectionService calls into your activity's implementation.

To do this, call the ActivityOnStart() and ActivityOnResume() methods in their respective overrides, like so:

```
protected override void OnStart()
{
    base.OnStart();
    ConnectionService.Instance.ActivityOnStart(this);
    ConnectionService.Instance.Boards.CollectionChanged += Boards_CollectionChanged;
}

private void Boards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
{
    // handle board attach/detached events here
}

protected override void OnResume()
{
    base.OnResume();
    ConnectionService.Instance.ActivityOnResume();
}
```
*/
    public abstract class ConnectionService : IConnectionService, IDisposable
    {
        private static readonly Lazy<WinUsbConnectionService> winUsbInstance = new Lazy<WinUsbConnectionService>();
        private static readonly Lazy<LibUsbConnectionService> libUsbInstance = new Lazy<LibUsbConnectionService>();
        private static readonly Lazy<MacUsbConnectionService> macUsbInstance = new Lazy<MacUsbConnectionService>();

        private static bool? isLinux;

        private static bool? isWindows;

        private static bool? isMac;

        private TaskCompletionSource<TreehopperUsb> waitForFirstBoard = new TaskCompletionSource<TreehopperUsb>();

        /// \cond PRIVATE
        public ConnectionService()
        {
            Boards = new ObservableCollection<TreehopperUsb>();
            Boards.CollectionChanged += Boards_CollectionChanged;
        }
        /// \endcond



        /** @name Main components
         *  @{
         */

        /// <summary>
        ///     The singleton instance through which to access %ConnectionService.
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
        /// The %Treehopper boards attached to the computer.
        /// </summary>
        /** For simple applications, you can retrieve a board instance with GetFirstDeviceAsync(), however, if you'd like to present the user with a list of devices from which to choose, you can reference this property.

        \warning Even if you already have a board connected to your device when you first invoke this property, this collection is not guaranteed to have a board populated on first invocation. Board discovery on many platforms is done asynchronously, so you should always bind to the [CollectionChanged](https://docs.microsoft.com/en-us/dotnet/api/system.collections.objectmodel.observablecollection-1.collectionchanged?view=netframework-4.7.1) event of this collection to get notified when boards are added.

        GUI apps or more advanced console apps can query or bind directly to the #Boards property, which is an ObservableCollection that can notify when boards are attached or removed:

        ```
        ConnectionService.Instance.Boards.CollectionChanged += async(o, e) => {
                if(e.NewItems.Count > 0) // a new board was added
                    RunApp((TreehopperUsb)e.NewItems[0])
            };

        // if we already have a board in the collection, start the app
        if(ConnectionService.Instance.Boards.Count > 0)
            RunApp(ConnectionService.Instance.Boards[0]);

        ```

        \warning After subscribing to CollectionChanged, you should always check the collection. A board may have already been added before you subscribed to the CollectionChanged event, and the event will not re-fire for new subscribers.

        */
        public ObservableCollection<TreehopperUsb> Boards { get; }
        
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
        ///@}


        /** @name Treehopper.Desktop-specific properties 
        @{ */

        /// <summary>
        /// Determines if executing in Windows
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
        /// Determines if we're running in Linux
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
        /// Determines if we're running in macOS
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
        /// Dispose this %ConnectionService to close device watchers and clean up resources
        /// </summary>
        public abstract void Dispose();

        ///@}


        
        
        

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