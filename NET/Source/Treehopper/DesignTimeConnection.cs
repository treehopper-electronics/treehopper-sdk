using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper
{
    public class DesignTimeConnection : IConnection
    {
        public DesignTimeConnection()
        {
            serialNumber = RandomString(16);
            name = "MyTreehopper";
            
        }
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
        }

        private string serialNumber;
        public string SerialNumber
        {
            get
            {
                return serialNumber;
            }
        }

        public string DevicePath { get; set; }

        public event PinEventData PinEventDataReceived;
        public event PropertyChangedEventHandler PropertyChanged;

        public void Close()
        {
            
        }

        public bool Open()
        {
            return false;
        }

        public void SendDataPeripheralChannel(byte[] data)
        {
            
        }

        public void SendDataPinConfigChannel(byte[] data)
        {
            
        }

        static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task<byte[]> ReadPeripheralResponsePacket(uint bytesToRead)
        {
            return new byte[0];
        }
    }
}
