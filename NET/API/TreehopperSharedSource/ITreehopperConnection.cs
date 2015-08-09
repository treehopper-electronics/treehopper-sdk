using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper
{
    public delegate void PinEventData(byte[] data);

    public interface ITreehopperConnection
    {
        void Open();
        void Close();

        void SendDataPinConfigChannel(byte[] data);
        void SendDataPeripheralChannel(byte[] data);

        event PinEventData PinEventDataReceived;
    }
}
