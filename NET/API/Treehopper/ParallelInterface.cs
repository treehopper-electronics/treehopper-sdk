using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper
{
    public enum ParallelCmd
    {
        WriteCommand,
        ReadCommand,
        WriteData,
        ReadData
    }
    public class ParallelInterface : IParallelInterface
    {

        public ParallelInterface(TreehopperUsb board)
        {
            this.board = board;
        }

        private TreehopperUsb board;

        public Collection<Pin> DataBus { get; set; } = new Collection<Pin>();
        public Pin RegisterSelectPin { get; set; }
        public Pin ReadWritePin { get; set; }

        public Pin EnablePin { get; set; }

        public int DelayMicroseconds { get; set; }

        private bool enabled;

        public bool Enabled
        {
            get { return enabled; }
            set {
                if (enabled == value) return;
                enabled = value;
                UpdateConfig();
            }
        }

        public int Width
        {
            get
            {
                return DataBus.Count;
            }
        }

        private void UpdateConfig()
        {
            if (DataBus.Count > 16 || DataBus.Count < 4)
                throw new ArgumentOutOfRangeException("DataBus should have between 4 and 16 pins");

            var cmd = new byte[7+DataBus.Count];
            cmd[0] = (byte)DeviceCommands.ParallelConfig;
            cmd[1] = (byte)(Enabled ? 1 : 0);               // enable/disable parallel module
            cmd[2] = (byte)DelayMicroseconds;               // placeholder for bus speed
            cmd[3] = (byte)DataBus.Count;                   // bus width
            cmd[4] = (byte)(RegisterSelectPin?.PinNumber ?? -1);
            cmd[5] = (byte)(ReadWritePin?.PinNumber ?? -1);
            cmd[6] = (byte)(EnablePin?.PinNumber ?? -1);
            for(int i=0;i<DataBus.Count;i++)
            {
                cmd[7 + i] = (byte)(DataBus[i].PinNumber);
                DataBus[i].Mode = PinMode.Reserved;
            }

            board.sendPeripheralConfigPacket(cmd);
        }

        public void WriteCommand(byte command, ushort[] data = null)
        {
            int dataLen = data?.Length ?? 0;
            byte[] cmd;
            if (data != null)
            {
                // we have data to send with the command
                if (DataBus.Count <= 8)
                {
                    // 8-bit data
                    cmd = new byte[dataLen + 2];

                    for (int i = 0; i < dataLen; i++)
                    {
                        cmd[4 + i] = (byte)(data[i]);
                    }
                }
                else
                {
                    // 16-bit data
                    cmd = new byte[dataLen * 2 + 2];

                    for (int i = 0; i < dataLen; i++)
                    {
                        cmd[4 + i * 2] = (byte)(data[2 * i] >> 8);
                        cmd[4 + i * 2 + 1] = (byte)(data[2 * i + 1]);
                    }
                }
            } else
            {
                // we have no data to send
                cmd = new byte[4];
            }
           

            cmd[0] = (byte)DeviceCommands.ParallelTransaction;
            cmd[1] = (byte)ParallelCmd.WriteCommand;
            cmd[2] = (byte)dataLen;
            cmd[3] = command;
            // cmd[4-...] already present from above

            board.sendPeripheralConfigPacket(cmd);
        }

        public void WriteData(ushort[] data)
        {
            int dataLen = data.Length;
            byte[] cmd;
            if (DataBus.Count <= 8)
            {
                // 8-bit data
                cmd = new byte[dataLen + 3];

                for (int i = 0; i < dataLen; i++)
                {
                    cmd[3 + i] = (byte)(data[i]);
                }
            }
            else
            {
                // 16-bit data
                cmd = new byte[dataLen * 2 + 3];

                for (int i = 0; i < dataLen; i++)
                {
                    cmd[3 + i * 2] = (byte)(data[2 * i] >> 8);
                    cmd[3 + i * 2 + 1] = (byte)(data[2 * i + 1]);
                }
            }

            cmd[0] = (byte)DeviceCommands.ParallelTransaction;
            cmd[1] = (byte)ParallelCmd.WriteData;
            cmd[2] = (byte)dataLen;
            // cmd[3...] data is already present

            board.sendPeripheralConfigPacket(cmd);
        }

        public async Task<ushort[]> ReadCommand(byte command, int length)
        {
            throw new NotImplementedException();
        }

        public async Task<ushort[]> ReadData(int length)
        {
            throw new NotImplementedException();
            //var receivedData = new ushort[length];
            //using (await board.ComsMutex.LockAsync())
            //{
            //    var cmd = new byte[3];
            //    cmd[0] = (byte)DeviceCommands.ParallelTransaction;
            //    cmd[1] = (byte)ParallelCmd.ReadData;
            //    cmd[2] = (byte)length;
            //    board.sendPeripheralConfigPacket(cmd);
            //    if(DataBus.Count <= 8)
            //    {
            //        // 8-bit transactions
            //        var data = await board.receiveCommsResponsePacket((byte)length);
            //        data.CopyTo(receivedData, 0);
            //    }
            //    else
            //    {
            //        // 16-bit transactions
            //        var data = await board.receiveCommsResponsePacket((uint)(length*2));
            //        for(int i=0;i<length;i++)
            //        {
            //            receivedData[i] = (ushort)(((ushort)data[2*i]) << 8 | (ushort)(data[2*i + 1]));
            //        }
            //    }
            //}
            //return receivedData;
        }
    }
}
