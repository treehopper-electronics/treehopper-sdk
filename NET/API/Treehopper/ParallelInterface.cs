namespace Treehopper
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;

    /// <summary>
    /// This module is used to provide an 8080-style R/W parallel interface (especially useful for displays)
    /// </summary>
    public class ParallelInterface : ReadWriteParallelInterface
    {
        private bool enabled;
        private TreehopperUsb board;

        internal ParallelInterface(TreehopperUsb board)
        {
            this.board = board;
        }

        private enum ParallelCmd
        {
            WriteCommand,
            ReadCommand,
            WriteData,
            ReadData
        }

        /// <summary>
        /// Controls which pins are used for the data bus.
        /// </summary>
        public Collection<Pin> DataBus { get; set; } = new Collection<Pin>();

        /// <summary>
        /// Gets or sets the pin used for Register Select (RS).
        /// </summary>
        public Pin RegisterSelectPin { get; set; }

        /// <summary>
        /// Gets or sets the pin used for Read/Write (R/W)
        /// </summary>
        public Pin ReadWritePin { get; set; }

        /// <summary>
        /// Gets or sets the pin used for Enable (E).
        /// </summary>
        public Pin EnablePin { get; set; }

        /// <summary>
        /// gets or sets the number of microseconds that should be delayed after pulse.
        /// </summary>
        public int DelayMicroseconds { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this peripheral is enabled
        /// </summary>
        public bool Enabled
        {
            get
            {
                return enabled;
            }

            set
            {
                if (enabled == value) return;
                enabled = value;
                UpdateConfig();
            }
        }

        /// <summary>
        /// Get the width of the data bus (i.e., the count of the number of pins in the bus)
        /// </summary>
        public int Width
        {
            get
            {
                return DataBus.Count;
            }
        }

        /// <summary>
        /// Write one or more words of data to the bus with the command flag asserted (RS=0);
        /// </summary>
        /// <param name="command"></param>
        public async Task WriteCommand(uint[] command)
        {
            int cmdLen = command.Length;
            byte[] cmd;

            // we have data to send with the command
            if (DataBus.Count <= 8)
            {
                // 8-bit data
                cmd = new byte[cmdLen + 3];

                for (int i = 0; i < cmdLen; i++)
                {
                    cmd[3 + i] = (byte)command[i];
                }
            }
            else
            {
                // 16-bit data
                cmd = new byte[(cmdLen * 2) + 3];

                for (int i = 0; i < cmdLen; i++)
                {
                    cmd[3 + (i * 2)] = (byte)(command[2 * i] >> 8);
                    cmd[3 + (i * 2) + 1] = (byte)command[(2 * i) + 1];
                }
            }

            cmd[0] = (byte)DeviceCommands.ParallelTransaction;
            cmd[1] = (byte)ParallelCmd.WriteCommand;
            cmd[2] = (byte)cmdLen;
            await board.SendPeripheralConfigPacket(cmd);
        }

        /// <summary>
        /// Write one or more words of data to the bus with the data flag asserted (RS=1);
        /// </summary>
        /// <param name="data"></param>
        public async Task WriteData(uint[] data)
        {
            int dataLen = data.Length;
            byte[] cmd;
            if (DataBus.Count <= 8)
            {
                // 8-bit data
                cmd = new byte[dataLen + 3];

                for (int i = 0; i < dataLen; i++)
                {
                    cmd[3 + i] = (byte)data[i];
                }
            }
            else
            {
                // 16-bit data
                cmd = new byte[(dataLen * 2) + 3];

                for (int i = 0; i < dataLen; i++)
                {
                    cmd[3 + (i * 2)] = (byte)(data[2 * i] >> 8);
                    cmd[3 + (i * 2) + 1] = (byte)data[(2 * i) + 1];
                }
            }

            cmd[0] = (byte)DeviceCommands.ParallelTransaction;
            cmd[1] = (byte)ParallelCmd.WriteData;
            cmd[2] = (byte)dataLen;
            await board.SendPeripheralConfigPacket(cmd);
        }

        /// <summary>
        /// Write a command to the bus and await data to be returned
        /// </summary>
        /// <param name="command">The command word to write</param>
        /// <param name="length">The number of words to read</param>
        /// <returns>An awaitable array of words read</returns>
        public async Task<ushort[]> ReadCommand(uint command, int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Read data words from the bus
        /// </summary>
        /// <param name="length">The number of words to read</param>
        /// <returns>An awaitable array of words read</returns>
        public async Task<ushort[]> ReadData(int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a string representation of the parallel interface's state
        /// </summary>
        /// <returns>the parallel interface's state</returns>
        public override string ToString()
        {
            if (enabled)
                return string.Format("Enabled, {0}-bit width, {1} us clock period", Width, DelayMicroseconds);
            else
                return "Not enabled";
        }

        private void UpdateConfig()
        {
            if (DataBus.Count > 16 || DataBus.Count < 4)
                throw new ArgumentOutOfRangeException("DataBus should have between 4 and 16 pins");

            var cmd = new byte[7 + DataBus.Count];
            cmd[0] = (byte)DeviceCommands.ParallelConfig;
            cmd[1] = (byte)(Enabled ? 1 : 0);               // enable/disable parallel module
            cmd[2] = (byte)DelayMicroseconds;               // placeholder for bus speed
            cmd[3] = (byte)DataBus.Count;                   // bus width
            cmd[4] = (byte)(RegisterSelectPin?.PinNumber ?? -1);
            cmd[5] = (byte)(ReadWritePin?.PinNumber ?? -1);
            cmd[6] = (byte)(EnablePin?.PinNumber ?? -1);
            for (int i = 0; i < DataBus.Count; i++)
            {
                cmd[7 + i] = (byte)DataBus[i].PinNumber;
                DataBus[i].Mode = PinMode.Reserved;
            }

            board.SendPeripheralConfigPacket(cmd);
        }
    }
}
