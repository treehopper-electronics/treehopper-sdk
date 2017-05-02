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

        public Register(RegisterManager regManager, int address, int width, bool isBigEndian)
        {
            manager = regManager;
            Width = width;
            IsLittleEndian = !isBigEndian;
            Address = address;
        }

        public Task Write()
        {
            return manager.Write(this);
        }

        public int Width { get; }
        internal abstract long GetValue();
        internal abstract void SetValue(long value);

        internal byte[] GetBytes()
        {
            var retVal = new byte[Width];
            for (var i = 0; i < Width; i++)
                retVal[i] = (byte)((GetValue() >> (8 * i)) & 0xFF);

            if (BitConverter.IsLittleEndian ^ IsLittleEndian)
                retVal = retVal.Reverse().ToArray();

            return retVal;
        }

        internal void SetBytes(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian ^ IsLittleEndian)
                bytes = bytes.Reverse().ToArray();

            long regVal = 0;

            for (var i = 0; i < bytes.Length; i++)
                regVal |= bytes[i] << (i * 8);

            SetValue(regVal);
        }
    }
}
