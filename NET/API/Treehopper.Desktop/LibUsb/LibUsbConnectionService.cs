using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// LibUSB-based Treehopper library for Linux support
/// </summary>
namespace Treehopper.Desktop.LibUsb
{
    /// <summary>
    /// LibUSB-based ConnectionService implementation
    /// </summary>
    public class LibUsbConnectionService : ConnectionService
    {
        private readonly IntPtr context;
        private IntPtr callbackHandle;
        private SynchronizationContext currentContext;
        public LibUsbConnectionService()
        {
            currentContext = SynchronizationContext.Current;
            NativeMethods.Init(out context);

            HotplugCallbackFunction cb = Callback;

            NativeMethods.HotplugRegisterCallback(context, HotplugEvent.DeviceArrived | HotplugEvent.DeviceLeft, 0,
                (int) TreehopperUsb.Settings.Vid, (int) TreehopperUsb.Settings.Pid, NativeMethods.HotplugMatchAny, cb,
                IntPtr.Zero, callbackHandle);
            InitialAdd();
            Task.Run(async () =>
            {
                while (true)
                {
                    NativeMethods.HandleEvents(context, IntPtr.Zero);
                    await Task.Delay(100).ConfigureAwait(false);
                }
            });
        }

        private int Callback(IntPtr context, IntPtr deviceProfile, HotplugEvent e, IntPtr userData)
        {
            //Task.Run(() =>
            //{
                if (e == HotplugEvent.DeviceArrived)
                {
                    var newBoard = new TreehopperUsb(new LibUsbConnection(deviceProfile));
                    Debug.WriteLine("Adding " + newBoard);
                    if (currentContext == null)
                        Boards.Add(newBoard);
                    else
                        currentContext.Post(
                            delegate
                            {
                                Boards.Add(newBoard);
                            }, null);
                }
                else if (e == HotplugEvent.DeviceLeft)
                {
                    var devicePath = deviceProfile.ToString();
                Debug.WriteLine("Removing devicePath " + devicePath);
                    RemoveDevice(devicePath);
                }
            //});
            return 0;
        }

        internal void RemoveDevice(string matchPath)
        {
            var boardToRemove = Boards.FirstOrDefault(b => b.Connection.DevicePath == matchPath);
            if (boardToRemove != null)
            {
                boardToRemove.Dispose();
                Debug.WriteLine("Removing " + boardToRemove);
                if (currentContext == null)
                    Boards.Remove(boardToRemove);
                else
                    currentContext.Post(
                        delegate {
                            Boards.Remove(boardToRemove);
                        }, null);
            }
        }

        private void InitialAdd()
        {
            IntPtr deviceProfilePtrPtr;
            var ret = NativeMethods.GetDeviceList(context, out deviceProfilePtrPtr);
            if (ret > 0 || deviceProfilePtrPtr == IntPtr.Zero)
            {
                for (var i = 0; i < ret; i++)
                {
                    // calculate the offset pointer
                    var deviceProfilePtr =
                        Marshal.ReadIntPtr(new IntPtr(deviceProfilePtrPtr.ToInt64() + i * IntPtr.Size));

                    var desc = new LibUsbDeviceDescriptor();
                    NativeMethods.GetDeviceDescriptor(deviceProfilePtr, desc);

                    if (desc.idVendor == TreehopperUsb.Settings.Vid && desc.idProduct == TreehopperUsb.Settings.Pid)
                    {
                        var board = new TreehopperUsb(new LibUsbConnection(deviceProfilePtr));
                        Debug.WriteLine("Adding " + board);
                        Boards.Add(board);
                    }
                }
            }
        }

        public override void Dispose()
        {
            NativeMethods.Exit(context);
        }
    }
}