using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper
{
    public delegate void PinEventData(byte[] data);

    public delegate void ConnectionAddedHandler(IConnection connectionAdded);
    public delegate void ConnectionRemovedHandler(IConnection connectionRemoved);

    public interface IConnection : INotifyPropertyChanged, IDisposable
    {
        Task<bool> OpenAsync();
        void Close();

        // these are read-only; TreehopperUsb is responsible for setting these using application commands
        string SerialNumber { get; } 
        string Name { get; }

        string DevicePath { get; set; }

        void SendDataPinConfigChannel(byte[] data);
        void SendDataPeripheralChannel(byte[] data);

        int UpdateRate { get; set; }

        Task<byte[]> ReadPeripheralResponsePacket(uint bytesToRead);

        event PinEventData PinEventDataReceived;
    }
}
