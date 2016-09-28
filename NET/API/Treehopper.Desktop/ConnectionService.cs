﻿using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using LibUsbDotNet.DeviceNotify;
using System.Collections.ObjectModel;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Treehopper
{

    public class ConnectionService : IConnectionService
    {
        private static readonly ConnectionService instance = new ConnectionService();
        public static ConnectionService Instance { get { return instance; } }


        public static bool IsWindows { get { return LibUsbDotNet.UsbDevice.IsWindows; } }
        public static bool IsLinux { get { return LibUsbDotNet.UsbDevice.IsLinux; } }
        public static bool IsMac { get { return LibUsbDotNet.UsbDevice.IsMac; } }

        private IDeviceNotifier myNotifier = DeviceNotifier.OpenDeviceNotifier();
        /// <summary>
        /// Occurs when a <see cref="TreehopperBoard"/> is removed from the system.
        /// </summary>
        //public event BoardEventHandler BoardRemoved;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Contains the collection of <see cref="TreehopperBoard"/>s currently attached to the system.
        /// </summary>
        //public ObservableCollection<TreehopperBoard> Boards { get; set; }

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

        /// <summary>
        /// Manages Treehopper boards connected to the computer. If optional filter parameters are provided, only boards matching the filter will be available.
        /// </summary>
        /// <param name="nameFilter">Name filter</param>
        /// <param name="serialFilter">Serial filter</param>
        public ConnectionService(string nameFilter = "", string serialFilter = "")
        {
            UsbDevice.ForceLibUsbWinBack = false;
            pollingTimer = new System.Timers.Timer();
            PollingTimerIsEnabled = false;
            PollingTimerInterval = 500;
            SerialFilterIsEnabled = serialFilter.Length > 0;
            NameFilterIsEnabled = nameFilter.Length > 0;
            SerialFilter = serialFilter;
            NameFilter = nameFilter;

            Rescan(); // add all the boards that were already connected when we started up

            // now, setup a device notifier so we can be alerted when boards are added/removed
            myNotifier.OnDeviceNotify += myNotifier_OnDeviceNotify;
            pollingTimer.Elapsed += devicePollingTimer_Elapsed;
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

        public ObservableCollection<TreehopperUsb> Boards { get; } = new ObservableCollection<TreehopperUsb>();

        void devicePollingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Rescan();
        }

        private void Rescan()
        {
            foreach (UsbRegistry regDevice in UsbDevice.AllDevices)
            {
                if (regDevice.Vid != TreehopperUsb.Settings.Vid || regDevice.Pid != TreehopperUsb.Settings.Pid)
                    continue;
                if (regDevice.Device == null)
                    continue;
                if (Boards.Where(i => i.Connection.DevicePath == regDevice.SymbolicName).Count() == 0)
                {
                    var board = new TreehopperUsb(new UsbConnection(regDevice));
                    Debug.WriteLine("Added board: " + board);
                    Boards.Add(board);
                }
            }

            foreach (var board in Boards.ToList())
            {
                bool deviceFound = false;
                foreach (UsbRegistry dev in UsbDevice.AllDevices)
                {
                    if (dev.Vid != TreehopperUsb.Settings.Vid || dev.Pid != TreehopperUsb.Settings.Pid)
                        continue;
                    if (dev.SymbolicName == board.Connection.DevicePath)
                        deviceFound = true;
                }
                if (!deviceFound)
                {
                    Debug.WriteLine("Removing board: " + board);
                    Boards.Remove(board);
                    board.Dispose();
                }
            }
        }

        void myNotifier_OnDeviceNotify(object sender, DeviceNotifyEventArgs e)
        {
            if (PollingTimerIsEnabled)
                return;
            if (e.Device.IdVendor != TreehopperUsb.Settings.Vid || e.Device.IdProduct != TreehopperUsb.Settings.Pid)
                return;

            if(e.EventType == EventType.DeviceArrival) // board added
            {
                foreach(UsbRegistry regDevice in UsbDevice.AllDevices)
                {
                    if(regDevice.Device != null)
                    {
						if (regDevice.Vid == TreehopperUsb.Settings.Vid || regDevice.Pid == TreehopperUsb.Settings.Pid) {
							bool isInBoardList = false;
							foreach (var board in Boards) {
								if (regDevice.SymbolicName == board.Connection.DevicePath) {
									isInBoardList = true;
								}
							}
							if (!isInBoardList && regDevice.Device.Info.SerialString.Length > 0) {
								Debug.WriteLine ("Adding board");
								Boards.Add (new TreehopperUsb(new UsbConnection(regDevice)));
							}
						}
                    }
                   
                }
            } else if(e.EventType == EventType.DeviceRemoveComplete) // board removed
            {
                string id = e.Device.SymbolicName.FullName.ToLower();

                Debug.WriteLine ("Device removed: " +id);

                var board = Boards.Where(x => x.Connection.SerialNumber == e.Device.SymbolicName.SerialNumber).ToList();
                if (board.Count > 0)
                {
                    board[0].Dispose();
                    Boards.Remove(board[0]);
                }


            }
        }

        private bool PassesFilter(UsbConnection connection)
        {
            if (SerialFilterIsEnabled && connection.SerialNumber != SerialFilter)
                return false;
            if (NameFilterIsEnabled && connection.Name != NameFilter)
                return false;
            return true;
        }

        public async Task<TreehopperUsb> GetFirstDeviceAsync()
        {
            while (Boards.Count == 0)
            {
                await Task.Delay(100);
            }

            return Boards[0];
        }

        public void Dispose()
        {
            
        }
    }
}
