using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper
{
    public enum ParallelMode
    {
        Intel8080,
        Motorola6800,

    }
    public interface IParallelInterface
    {
        bool Enabled { get; set; }
        void WriteCommand(byte command, ushort[] data = null);
        void WriteData(ushort[] data);
        int DelayMicroseconds { get; set; }
        int Width { get; }
        Task<ushort[]> ReadCommand(byte command, int length);

        Task<ushort[]> ReadData(int length);

    }
}
