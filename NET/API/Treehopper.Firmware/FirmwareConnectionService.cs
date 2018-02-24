using HidSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Treehopper.Firmware
{
    public class FirmwareConnectionService
    {
        public static readonly FirmwareConnectionService _instance = new FirmwareConnectionService();
        private SynchronizationContext currentContext;

        public static FirmwareConnectionService Instance => _instance;

        public FirmwareConnectionService()
        {
            currentContext = SynchronizationContext.Current;
            DeviceList.Local.Changed += DeviceList_Changed;
            DeviceList.Local.RaiseChanged(); // Fire off the event to get things started
        }

        public ObservableCollection<FirmwareUpdateDevice> Boards { get; set; } = new ObservableCollection<FirmwareUpdateDevice>();

        /// <summary>
        ///     The PID used by the bootloader
        /// </summary>
        public int BootloaderPid { get; set; } = 0xeac9;

        /// <summary>
        ///     The VID used by the bootloader
        /// </summary>
        public int BootloaderVid { get; set; } = 0x10c4;

        private void DeviceList_Changed(object sender, DeviceListChangedEventArgs e)
        {
            var devs = DeviceList.Local.GetHidDevices(BootloaderVid, BootloaderPid);
            var boardsToRemove = Boards.Where(board => 
                devs.Where(dev => dev.DevicePath == board.DevicePath).Count() == 0).ToList();

            foreach (var board in boardsToRemove)
                if (currentContext == null)
                    Boards.Remove(board);
                else
                    currentContext.Post(delegate { Boards.Remove(board); }, null);
                

            var devsToAdd = devs.Where(dev => Boards.Where(board => board.DevicePath == dev.DevicePath).Count() == 0);

            foreach(var dev in devsToAdd)
            {
                if(currentContext == null)
                    Boards.Add(new FirmwareUpdateDevice(dev));
                else
                    currentContext.Post(delegate { Boards.Add(new FirmwareUpdateDevice(dev)); }, null);
            }
                
            
        }
    }
}
