using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries
{
    public abstract class Register
    {
        protected RegisterManager manager;
        public bool IsLittleEndian { get; }
        public int Address { get;  }
        public int Width { get; }

        public Register(RegisterManager regManager, int address, int width, bool isBigEndian)
        {
            manager = regManager;
            Width = width;
            IsLittleEndian = !isBigEndian;
            Address = address;
        }

        public Task writeAsync()
        {
            return manager.write(this);
        }

        public void write()
        {
            Task.Run(writeAsync).Wait();
        }

        public Task readAsync()
        {
            return manager.read(this);
        }

        public void read()
        {
            Task.Run(readAsync).Wait();
        }

        internal abstract long getValue();
        internal abstract void setValue(long value);

        internal byte[] getBytes()
        {
            var retVal = new byte[Width];
            for (var i = 0; i < Width; i++)
                retVal[i] = (byte)((getValue() >> (8 * i)) & 0xFF);

            if (BitConverter.IsLittleEndian ^ IsLittleEndian)
                retVal = retVal.Reverse().ToArray();

            return retVal;
        }

        internal void setBytes(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian ^ IsLittleEndian)
                bytes = bytes.Reverse().ToArray();

            long regVal = 0;

            for (var i = 0; i < bytes.Length; i++)
                regVal |= bytes[i] << (i * 8);

            setValue(regVal);
        }
    }
}
