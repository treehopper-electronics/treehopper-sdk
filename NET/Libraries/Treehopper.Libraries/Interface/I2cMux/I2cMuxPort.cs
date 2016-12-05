using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface.I2cMux
{
    public class I2cMuxPort : II2c
    {
        public bool Enabled
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

        public double Speed
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

        public Task<byte[]> SendReceive(byte address, byte[] dataToWrite, byte numBytesToRead)
        {
            throw new NotImplementedException();
        }
    }
}
