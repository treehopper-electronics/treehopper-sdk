using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper
{
    public delegate void PinEventData(byte[] data);

    public delegate void ConnectionAddedHandler(ITreehopperConnection connectionAdded);
    public delegate void ConnectionRemovedHandler(ITreehopperConnection connectionRemoved);

    public interface ITreehopperConnection
    {
        bool Open();
        void Close();

        string SerialNumber { get; set; }
        string DeviceName { get; set; }

        void SendDataPinConfigChannel(byte[] data);
        void SendDataPeripheralChannel(byte[] data);

        event PinEventData PinEventDataReceived;
    }
}
