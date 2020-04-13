using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries
{
    public interface IRegisterManagerAdapter
    {
        Task<byte[]> read(int address, int width);

        Task write(int address, byte[] data);
    }
}
