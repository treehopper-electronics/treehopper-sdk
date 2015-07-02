using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using LibUsbDotNet.DeviceNotify;
using System.Collections.ObjectModel;

namespace Treehopper
{
    /// <summary>
    /// Used to send BoardAdded and BoardRemoved events.
    /// </summary>
    /// <param name="sender">The TreehopperManager that fired the event</param>
    /// <param name="board">The TreehopperBoard that was added or removed</param>
    public delegate void BoardEventHandler(TreehopperManager sender, TreehopperBoard board);
    /// <summary>
    /// Manages Treehopper boards connected to this computer
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class is responsible for instantiating <see cref="TreehopperBoard"/>s connected to the system, and reacting when boards are
    /// connected or disconnected during the lifecycle of the application.
    /// </para>
    /// <para>
    /// The manager can be configured to only respond to boards matching a specified <see cref="TreehopperBoard.Name"/> or 
    /// <see cref="TreehopperBoard.SerialNumber"/> using the <see cref="NameFilter"/> or <see cref="SerialFilter"/> properties.
    /// </para>
    /// <para>
    /// Automatically receiving device change notifications is supported in Forms and WPF applications. For console-based applications, 
    /// enable polling with the <see cref="PollingTimerIsEnabled"/> property.
    /// </para>
    /// </remarks>
    public class TreehopperManager
    {
        //private static IDeviceNotifier myNotifier = DeviceNotifier.OpenDeviceNotifier();
        private static IDeviceNotifier myNotifier = DeviceNotifier.OpenDeviceNotifier();
        int vid = 0x04d8;
        int pid = 0xF426;

        private event BoardEventHandler PreBoardAdded;

        /// <summary>
        /// Occurs when a board is added.
        /// </summary>
        public event BoardEventHandler BoardAdded
        {
            add
            {
                PreBoardAdded += value;
                foreach(var board in Boards)
                    PreBoardAdded(this, Boards[0]);
            }
            remove
            {
                PreBoardAdded -= value;
            }
        }

        /// <summary>
        /// Occurs when a <see cref="TreehopperBoard"/> is removed from the system.
        /// </summary>
        public event BoardEventHandler BoardRemoved;

        /// <summary>
        /// Contains the collection of <see cref="TreehopperBoard"/>s currently attached to the system.
        /// </summary>
        public ObservableCollection<TreehopperBoard> Boards { get; set; }

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
        public TreehopperManager(string nameFilter = "", string serialFilter = "")
        {
            UsbDevice.ForceLibUsbWinBack = false;
            pollingTimer = new System.Timers.Timer();
            PollingTimerIsEnabled = false;
            PollingTimerInterval = 500;
            SerialFilterIsEnabled = serialFilter.Length > 0;
            NameFilterIsEnabled = nameFilter.Length > 0;
            SerialFilter = serialFilter;
            NameFilter = nameFilter;
            Boards = new ObservableCollection<TreehopperBoard>();

            Boards.CollectionChanged += Boards_CollectionChanged;

            Scan(); // add all the boards that were already connected when we started up
            

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


        void devicePollingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Scan();
        }

        private void Scan()
        {
            // get a list of all current devices attached to the computer
            
           
            // Go through the list of existing Boards and remove any that no longer exist.
            List<TreehopperBoard> BoardsToRemove = new List<TreehopperBoard>();
            foreach(var board in Boards.ToList())
            {
                bool boardExistsInDeviceList = false;
                foreach (UsbRegistry regDevice in UsbDevice.AllDevices)
                {
                    if (regDevice.Vid == vid && regDevice.Pid == pid)
                    {
                        if(regDevice.Device != null)
                        {
                            TreehopperBoard newBoard = new TreehopperBoard(regDevice.Device);
                            if (newBoard.Equals(board))
                            {
                                boardExistsInDeviceList = true;
                            }
                        }
                        else
                        { // If this property reads null, it's probably because the board is open.
                            boardExistsInDeviceList = true;
                        }
                        
                    }
                }
                if (!boardExistsInDeviceList)
                {
                    BoardsToRemove.Add(board);
                }
            }

            foreach(var board in BoardsToRemove)
            {
                Boards.Remove(board);
                Debug.WriteLine("New board list has " + Boards.Count + " Boards");
            }
            

            // Now add any new boards
            foreach (UsbRegistry regDevice in UsbDevice.AllDevices)
            {
                if (regDevice.Vid == vid && regDevice.Pid == pid)
                {
                    if(regDevice.Device != null)
                    {
                        TreehopperBoard newBoard = new TreehopperBoard(regDevice.Device);
                        if (PassesFilter(newBoard))
                        {
                            // add the board to the list if it doesn't already exist
                            bool alreadyInList = false;
                            foreach (var board in Boards)
                            {
                                if (board.Equals(newBoard))
                                {
                                    alreadyInList = true;
                                }
                            }
                            if (!alreadyInList)
                            {
                                Boards.Add(newBoard);
                                Debug.WriteLine("Adding " + newBoard.ToString() + ". New board list has " + Boards.Count + " Boards");
                            }

                        }
                    }
                   
                }
            }
        }

        void Boards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (TreehopperBoard board in e.NewItems)
                {
                    if (PreBoardAdded != null) PreBoardAdded(this, board);
                }
            }
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (TreehopperBoard board in e.OldItems)
                {
                    if (BoardRemoved != null) BoardRemoved(this, board);
                }
            }
        }

        void myNotifier_OnDeviceNotify(object sender, DeviceNotifyEventArgs e)
        {
            if (PollingTimerIsEnabled)
                return;
            if (e.Device.IdVendor != vid || e.Device.IdProduct != pid)
                return;

            if(e.EventType == EventType.DeviceArrival) // board added
            {
                foreach(UsbRegistry regDevice in UsbDevice.AllDevices)
                {
                    if(regDevice.Device != null)
                    {
						if (regDevice.Vid == vid || regDevice.Pid == pid) {
							bool isInBoardList = false;
							foreach (var board in Boards) {
								if (regDevice.Device.Info.SerialString == board.SerialNumber) {
									isInBoardList = true;
								}
							}
							if (!isInBoardList) {
								Debug.WriteLine ("Adding board");
								Boards.Add (new TreehopperBoard (regDevice.Device));
							}
						}

						// This code only works under WinUSB, since e.Device.SerialNumber is empty in LibUsb
//						if (regDevice.Device.Info.SerialString.Length > 0) 
//						{
//							if (regDevice.Device.Info.SerialString == e.Device.SerialNumber)
//							{
//								TreehopperBoard board = new TreehopperBoard(regDevice.Device);
//								if (PassesFilter(board))
//									Boards.Add(board);
//							}
//						}
                        
                    }
                   
                }
            } else if(e.EventType == EventType.DeviceRemoveComplete) // board removed
            {
				Debug.WriteLine ("DEVICE REMOVED!");
				TreehopperBoard boardToRemove = null;
				foreach (var board in Boards) {
					bool deviceExists = false;
//					var deviceList = UsbDevice.AllDevices.ToList ();
//					foreach (UsbRegistry regDevice in deviceList) {
//						if (board.SerialNumber == regDevice.Device.Info.SerialString) {
//							deviceExists = true;
//						}
//					}
					if (!deviceExists) {
						boardToRemove = board;
					}
				}
				if (boardToRemove != null) {
					Boards.Remove (boardToRemove);
					Debug.WriteLine ("Removed board ");

				}
				// This code only works on Windows. LibUsb doesn't get access to the serial number.
				/*
                var board = Boards.Where(x => x.SerialNumber == e.Device.SerialNumber).ToList();
                if(board.Count > 0)
                {
                    board[0].Dispose();
                    Boards.Remove(board[0]);
                }
                */

            }
        }

        private bool PassesFilter(TreehopperBoard board)
        {
            if (SerialFilterIsEnabled && board.SerialNumber != SerialFilter)
                return false;
            if (NameFilterIsEnabled && board.Name != NameFilter)
                return false;
            return true;
        }

    }
}
