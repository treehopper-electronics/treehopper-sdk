using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper
{
    public interface IOneWire
    {
        void StartOneWire();
        Task OneWireResetAndMatchAddress(UInt64 address);
        Task<List<UInt64>> OneWireSearch();
        Task<bool> OneWireReset();
        Task<byte[]> Receive(int numBytes = 0);
        Task Send(byte[] dataToSend);
        Task Send(byte data);
        
    }
}
