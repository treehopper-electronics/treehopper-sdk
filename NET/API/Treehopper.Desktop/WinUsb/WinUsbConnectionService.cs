using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Desktop.WinUsb
{
    public class WinUsbConnectionService : ConnectionService
    {
        public WinUsbConnectionService() : base()
        {
            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_PnPEntity WHERE PNPClass = 'USBDevice'"))
            {
                collection = searcher.Get();
            }
                

            foreach (var device in collection)
            {
                string deviceID = (string)device.GetPropertyValue("DeviceID");
                var elements = deviceID.Split('\\');
                var vidPid = elements[1].Split('&');
                ushort vid = Convert.ToUInt16(vidPid[0].Substring(4, 4), 16);
                ushort pid = Convert.ToUInt16(vidPid[1].Substring(4, 4), 16);

                if(vid == TreehopperUsb.Settings.Vid && pid == TreehopperUsb.Settings.Pid)
                {
                    Debug.WriteLine("Found board!");
                    string serial = elements[2].ToLower();
                    if (Boards.Where(b => b.SerialNumber == serial).Count() > 0) // we already have the board
                        break;
                    var hardwareIds = ((string[])device.GetPropertyValue("HardwareID"))[0].Split('&');
                    var version = hardwareIds[2].Substring(4);

                    string name = (string)device.GetPropertyValue("Name");
                    string vidString = BitConverter.ToString(new byte[] { (byte)(vid >> 8), (byte)vid }).Replace("-", "").ToLower();
                    string pidString = BitConverter.ToString(new byte[] { (byte)(pid >> 8), (byte)pid }).Replace("-", "").ToLower();
                    string path = string.Format(@"\\.\usb#vid_{0}&pid_{1}#{2}#{{a5dcbf10-6530-11d2-901f-00c04fb951ed}}", vidString, pidString, serial);

                    Boards.Add(new TreehopperUsb(new WinUsbConnection(path, name, serial, short.Parse(version))));
                    
                }
            }

            collection.Dispose();
        }

    
    protected override void Rescan()
        {
            
        }
    }
}
