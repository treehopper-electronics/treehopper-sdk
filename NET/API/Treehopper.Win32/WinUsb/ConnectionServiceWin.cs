using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Treehopper
{
    public sealed class ConnectionServiceWin : IConnectionService, IDisposable
    {
        DevNotifyNativeWindow notifyWindow;

        Thread staThread;
        public ConnectionServiceWin()
        {
            Debug.WriteLine("Detected Windows platform. Starting notification window.");
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                notifyWindow = new DevNotifyNativeWindow(OnHandleChange, OnDeviceChange);
            }
            else
            {
                staThread = new Thread(new ThreadStart(() =>
                {
                    notifyWindow = new DevNotifyNativeWindow(OnHandleChange, OnDeviceChange);
                    Application.Run();
                }));
                staThread.SetApartmentState(ApartmentState.STA);
                staThread.Name = "DevNotifyNativeWindow STA Thread";
                staThread.Start();
            }
            Debug.WriteLine("Detected UNIX platform. Starting /dev watcher.");

            StartListener();
        }

        SemaphoreSlim message;

        public async void StartListener()
        {
            message = new SemaphoreSlim(0);
            while (true)
            {
                await message.WaitAsync();
                UpdateDeviceList();
                message = new SemaphoreSlim(0);
            }
            

        }

        internal void OnDeviceChange(ref Message m)
        {
            message?.Release();
            
        }


        internal void OnHandleChange(IntPtr windowHandle)
        {
            message?.Release();
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
            while (Boards.Count == 0)
            {
                //UpdateDeviceList();
                await Task.Delay(100);
            }

            return Boards[0];
        }

        internal void UpdateDeviceList()
        {
            try
            {
                var deviceList = new Dictionary<string, string>();
                ManagementObjectSearcher managementdeviceList = new ManagementObjectSearcher("Select * from Win32_PnPEntity");
                if (managementdeviceList != null)
                {


                    foreach (ManagementObject device in managementdeviceList.Get())
                    {
                        if (device["DeviceID"].ToString().Contains("VID_10C4&PID_8A7E"))
                        {
                            string name = device["Name"].ToString();
                            string deviceId = device["DeviceID"].ToString().ToLower();
                            var paths = deviceId.Split('\\');
                            string devPath = "\\\\?\\usb#" + paths[1] + "#" + paths[2] + "#{" + TreehopperUsb.Settings.Guid.ToString() + "}";

                            deviceList.Add(devPath, name);

                            //Debug.WriteLine(String.Format("Found Treehopper board. Name = {0}, Path = {1}", name, devPath));

                            //foreach (var prop in device.Properties)
                            //{
                            //    Debug.WriteLine(prop.Name + ": " + prop.Value);
                            //}
                        }
                    }

                }


                // Alright, now let's parse list and see which devices need to be added or deleted
                foreach (var id in deviceList)
                {
                    if (Boards.Where(i => i.Connection.DevicePath == id.Key).Count() == 0)
                    {
                        var board = new TreehopperUsb(new WinUsbConnection(id.Key, id.Value));
                        Debug.WriteLine("Added board: " + board);
                        Boards.Add(board);
                    }
                }

                foreach (var board in Boards.ToList())
                {
                    if (deviceList.Keys.Where(i => i == board.Connection.DevicePath).Count() == 0)
                    {
                        Debug.WriteLine("Removing board: " + board);
                        Boards.Remove(board);
                        board.Dispose();
                    }
                }
            } catch(Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message);
            }
        }


        public void Dispose()
        {
            staThread.Abort();
        }
    }
}
