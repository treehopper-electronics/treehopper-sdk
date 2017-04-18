using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Utilities
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Int24
    {
        private Byte _b0;
        private Byte _b1;
        private Byte _b2;

        public Int24(Int32 value)
        {
            _b0 = (byte)(value & 0xFF);
            _b1 = (byte)(value >> 8);
            _b2 = (byte)(value >> 16);
        }
        public Int32 Value => _b0 | (_b1 << 8) | (_b2 << 16) | ((_b2 & 0x80) > 0 ? 0xFF : 0x00);

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UInt24
    {
        private Byte _b0;
        private Byte _b1;
        private Byte _b2;

        public UInt24(UInt32 value)
        {
            _b0 = (byte)(value & 0xFF);
            _b1 = (byte)(value >> 8);
            _b2 = (byte)(value >> 16);
        }
        public UInt32 Value => (uint)(_b0 | (_b1 << 8) | (_b2 << 16));

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
