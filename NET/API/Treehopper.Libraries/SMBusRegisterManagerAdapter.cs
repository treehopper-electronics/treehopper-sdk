using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries
{
    public class SMBusRegisterManagerAdapter : IRegisterManagerAdapter
    {
        protected SMBusDevice _dev;

        public SMBusRegisterManagerAdapter(SMBusDevice dev)
        {
            _dev = dev;
        }

        public Task<byte[]> read(int address, int width)
        {
            return _dev.ReadBufferDataAsync((byte)address, width);
        }

        public Task write(int address, byte[] data)
        {
            return _dev.WriteBufferDataAsync((byte)address, data);
        }
    }
}
