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
        //Motorola6800,
    }
    public interface IWriteOnlyParallelInterface
    {
        bool Enabled { get; set; }
        void WriteCommand(uint[] command);
        void WriteData(uint[] data);
        int DelayMicroseconds { get; set; }
        int Width { get; }
    }

    public interface IReadWriteParallelInterface : IWriteOnlyParallelInterface
    {
        Task<ushort[]> ReadCommand(uint command, int length);

        Task<ushort[]> ReadData(int length);

    }
}
