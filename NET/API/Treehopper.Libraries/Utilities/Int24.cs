using System.Runtime.InteropServices;

namespace Treehopper.Libraries.Utilities
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Int24
    {
        private readonly byte _b0;
        private readonly byte _b1;
        private readonly byte _b2;

        public Int24(int value)
        {
            _b0 = (byte) (value & 0xFF);
            _b1 = (byte) (value >> 8);
            _b2 = (byte) (value >> 16);
        }

        public int Value => _b0 | (_b1 << 8) | (_b2 << 16) | ((_b2 & 0x80) > 0 ? 0xFF : 0x00);

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UInt24
    {
        private readonly byte _b0;
        private readonly byte _b1;
        private readonly byte _b2;

        public UInt24(uint value)
        {
            _b0 = (byte) (value & 0xFF);
            _b1 = (byte) (value >> 8);
            _b2 = (byte) (value >> 16);
        }

        public uint Value => (uint) (_b0 | (_b1 << 8) | (_b2 << 16));

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}