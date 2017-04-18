using System.Diagnostics;

namespace Treehopper.Libraries.ArduinoShim
{
    /// <summary>
    ///     A delegate used to represent when write data was requested
    /// </summary>
    /// <param name="message">The message written</param>
    public delegate void SerialShimWriter(string message);

    /// <summary>
    ///     Provides Serial-like functionality that can be directed
    ///     both to the Debug output, or also to the board's UART.
    /// </summary>
    public class SerialShim
    {
        private readonly TreehopperUsb board;
        private bool redirectToDebug = true;

        /// <summary>
        ///     Construct a new instance of SerialShim.
        /// </summary>
        /// <param name="board">the board to reference for hardware UART transactions</param>
        public SerialShim(TreehopperUsb board)
        {
            this.board = board;
        }

        /// <summary>
        ///     An event that fires when data was written
        /// </summary>
        public event SerialShimWriter WriteRequested;

        /// <summary>
        ///     Enable the UART module (or Debug logging) on the board with the given parameters
        /// </summary>
        /// <param name="baud">The baud rate to use</param>
        /// <param name="redirectToDebugStream">Whether to direct Serial.write() to Debug, or to send over the physical UART.</param>
        public void begin(int baud, bool redirectToDebugStream = true)
        {
            redirectToDebug = redirectToDebugStream;
            if (redirectToDebugStream)
                return;

            board.Uart.Baud = baud;
            board.Uart.Enabled = true;
        }

        /// <summary>
        ///     Write a value to the given stream.
        /// </summary>
        /// <param name="value">The value to write</param>
        public void write(dynamic value)
        {
            if (redirectToDebug)
            {
                string str = value.ToString();
                Debug.WriteLine(str);
                if (WriteRequested != null)
                    WriteRequested(str);
            }
            else
            {
                board.Uart.Send(value);
            }
        }
    }
}