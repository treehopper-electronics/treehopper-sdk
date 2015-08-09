using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Win32
{
    public class TreehopperConnection : ITreehopperConnection
    {
        public event PinEventData PinEventDataReceived;

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            throw new NotImplementedException();
        }

        public void SendDataPeripheralChannel(byte[] data)
        {
            throw new NotImplementedException();
        }

        public void SendDataPinConfigChannel(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
