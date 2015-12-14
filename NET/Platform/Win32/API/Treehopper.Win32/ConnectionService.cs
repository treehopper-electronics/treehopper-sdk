using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Treehopper
{
    public sealed class ConnectionService : IConnectionService
    {
        static readonly ConnectionService instance = new ConnectionService();
        public static ConnectionService Instance { get { return instance; } }

        public ConnectionService()
        {
            string devicePath = string.Empty;
        }

        ObservableCollection<TreehopperUsb> boards = new ObservableCollection<TreehopperUsb>();
        public ObservableCollection<TreehopperUsb> Boards
        {
            get
            {
                return boards;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task<TreehopperUsb> First()
        {
            while(Boards.Count == 0)
            {
                UpdateDeviceList();
                await Task.Delay(100);
            }
                
            return Boards[0];
        }

        internal void UpdateDeviceList()
        {
            var deviceList = new List<string>();
            Int32 bufferSize = 0;
            IntPtr detailDataBuffer = IntPtr.Zero;
            var deviceInfoSet = new IntPtr();
            Boolean lastDevice = false;
            var myDeviceInterfaceData = new DeviceManagement.SP_DEVICE_INTERFACE_DATA();
            Guid myGuid = TreehopperUsb.Guid;
            try
            {
                deviceInfoSet = DeviceManagement.SetupDiGetClassDevs(ref myGuid, IntPtr.Zero, IntPtr.Zero, DeviceManagement.DIGCF_PRESENT | DeviceManagement.DIGCF_DEVICEINTERFACE);

                Boolean deviceFound = false;
                Int32 memberIndex = 0;

                myDeviceInterfaceData.cbSize = Marshal.SizeOf(myDeviceInterfaceData);

                do
                {
                    Boolean success = DeviceManagement.SetupDiEnumDeviceInterfaces
                        (deviceInfoSet,
                         IntPtr.Zero,
                         ref myGuid,
                         memberIndex,
                         ref myDeviceInterfaceData);

                    if (!success)
                    {
                        lastDevice = true;

                    }
                    else
                    {
                        DeviceManagement.SetupDiGetDeviceInterfaceDetail
                            (deviceInfoSet,
                             ref myDeviceInterfaceData,
                             IntPtr.Zero,
                             0,
                             ref bufferSize,
                             IntPtr.Zero);

                        detailDataBuffer = Marshal.AllocHGlobal(bufferSize);

                        Marshal.WriteInt32(detailDataBuffer, (IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8);

                        DeviceManagement.SetupDiGetDeviceInterfaceDetail
                            (deviceInfoSet,
                             ref myDeviceInterfaceData,
                             detailDataBuffer,
                             bufferSize,
                             ref bufferSize,
                             IntPtr.Zero);

                        // Skip over cbsize (4 bytes) to get the address of the devicePathName.

                        var pDevicePathName = new IntPtr(detailDataBuffer.ToInt64() + 4);

                        // Get the String containing the devicePathName.

                        deviceList.Add(Marshal.PtrToStringAuto(pDevicePathName));

                        deviceFound = true;
                    }
                    memberIndex = memberIndex + 1;
                }
                while (lastDevice != true);

                // Alright, now let's parse list and see which devices need to be added or deleted

                foreach(var id in deviceList)
                {
                    if(Boards.Where(i => i.Connection.DevicePath == id).Count() == 0)
                    {
                        var board = new TreehopperUsb(new UsbConnection(id));
                        Debug.WriteLine("Added board: " + board);
                        Boards.Add(board);
                    }
                }

                foreach(var board in Boards)
                {
                    if(deviceList.Where(i => i == board.Connection.DevicePath).Count() == 0)
                    {
                        Boards.Remove(board);
                    }
                }

               

            }


            catch (Exception ex)
            {
                DisplayException("ConnectionService", ex);
                throw;
            }

            finally
            {
                if (detailDataBuffer != IntPtr.Zero)
                {
                    // Free the memory allocated previously by AllocHGlobal.

                    Marshal.FreeHGlobal(detailDataBuffer);
                }
                if (deviceInfoSet != IntPtr.Zero)
                {
                    DeviceManagement.SetupDiDestroyDeviceInfoList(deviceInfoSet);
                }
            }
        }

        internal static void DisplayException(String Name, Exception e)
        {

            //  Create an error message.

            String message = "Exception: " + e.Message + Environment.NewLine + "Module: " + Name + Environment.NewLine + "Method: " +
                        e.TargetSite.Name;
            Debug.Print(message);
        }

    }
}
