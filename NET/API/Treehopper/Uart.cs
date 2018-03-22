using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper
{
    interface Uart
    {
        Task StartUartAsync(int baud);
        int Baud { get; set; }
        Task SendAsync(byte[] dataToSend);
        Task<byte[]> ReceiveAsync();
    }
}
