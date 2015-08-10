using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper
{
    public class TreehopperUsbConnection : ITreehopperConnection
    {
        #region Instance
        public event PinEventData PinEventDataReceived;

        public string SerialNumber
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string DeviceName
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public bool Open()
        {
            return true;
        }

        public void SendDataPeripheralChannel(byte[] data)
        {
            throw new NotImplementedException();
        }

        public void SendDataPinConfigChannel(byte[] data)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ConnectionManager

        public static List<TreehopperUsbConnection> ConnectedDevices { get; set; }

        public static event ConnectionAddedHandler ConnectionAdded;

        public static event ConnectionRemovedHandler ConnectionRemoved;

        public static void StartConnectionManager()
        {
            ConnectedDevices = new List<TreehopperUsbConnection>();
        }

        #endregion

    }
}
