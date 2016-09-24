using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper
{
    public interface IFirmwareConnection
    {
        Task<bool> OpenAsync();
        void Close();

        Task<bool> Write(byte[] data);
        Task<byte[]> Read(int numBytes);
    }
}
