using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;

namespace ArduinoShim
{
    public delegate void SerialShimWriter(string message);

    public class SerialShim
    {
        TreehopperUsb board;
        bool redirectToDebug = true;

        public event SerialShimWriter WriteRequested;

        public SerialShim(TreehopperUsb board)
        {
            this.board = board;
        }

        public void begin(int baud, bool redirectToDebugStream = true)
        {
            this.redirectToDebug = redirectToDebugStream;
            if (redirectToDebugStream)
                return;

            board.Uart.Baud = baud;
            board.Uart.Enabled = true;
        }

        public void write(dynamic value)
        {
            if(redirectToDebug)
            {
                string str = value.ToString();
                Debug.WriteLine(str);
                if (WriteRequested != null)
                    WriteRequested(str);
            } else
            {
                board.Uart.Send(value);
            }
        }
    }
}
