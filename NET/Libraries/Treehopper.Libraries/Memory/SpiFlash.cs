using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Memory
{
    public class SpiFlash
    {
        SpiDevice dev;

        public enum JedecManufacturer
        {
            WinbondNexcom = 0xEF,
        }

        public class JedecId
        {
            public JedecManufacturer Manufacturer { get; set; }
            public byte MemoryType { get; set; }
            public byte CapacityId { get; set; }
        }

        public enum Command
        {
            WriteEnable = 0x06,
            WriteDisable = 0x04,

            ReadStatus1 = 0x05,
            WriteStatus1 = 0x01,
            ReadStatus2 = 0x35,
            WriteStatus2 = 0x31,
            ReadStatus3 = 0x15,
            WriteStatus3 = 0x11,

            ChipErase = 0x60,

            ReadJedecId = 0x9F,
            ReadUniqueID = 0x4B,

            ReadUniqueId = 0x4B,

            PageProgram = 0x02,

            BlockErase_4k = 0x20,
            BlockErase_32k = 0x52,
            BlockErase_64k = 0xD8,

            ReadData = 0x03,
            ReadSfdp = 0x5A,

            EraseSecurityRegister = 0x44,
            ProgramSecurityRegister = 0x42,
            ReadSecurityRegister = 0x48,
        }

        public SpiFlash(Spi dev, SpiChipSelectPin cs)
        {
            this.dev = new SpiDevice(dev, cs, ChipSelectMode.SpiActiveLow, 6);

        }

        public async Task<JedecId> ReadJedecId()
        {
            var result = await dev.SendReceive(new byte[4] { (byte)Command.ReadJedecId, 0x00, 0x00, 0x00 });

            var id = new JedecId();
            id.Manufacturer = (JedecManufacturer)result[1];
            id.MemoryType = result[2];
            id.CapacityId = result[3];

            return id;
        }
        
        public async Task<byte> ReadStatus()
        {
            var cmd = new byte[2];
            cmd[0] = (byte)Command.ReadStatus1;
            var result = await dev.SendReceive(cmd);
            return result[1];
        }
        public async Task<byte[]> ReadArray(int address, int count)
        {
            var data = new byte[count + 4];
            data[0] = (byte)Command.ReadData;
            data[1] = (byte)(address >> 16);
            data[2] = (byte)(address >> 8);
            data[3] = (byte)(address);
            var result = await dev.SendReceive(data);
            return result.Skip(4).Take(count).ToArray();
        }

        public async Task<byte> ReadByte(int address)
        {
            var res = await ReadArray(address, 1);
            return res[0];
        }

        public async Task Write(byte[] data, int address)
        {
            await WriteEnable();
            var header = new byte[4];
            header[0] = (byte)Command.PageProgram;
            header[1] = (byte)(address >> 16);
            header[2] = (byte)(address >> 8);
            header[3] = (byte)(address);

            await dev.SendReceive(header.Concat(data).ToArray()).ConfigureAwait(false);
        }

        public async Task EraseChip()
        {
            await WriteEnable().ConfigureAwait(false);
            await dev.SendReceive(new byte[] { (byte)Command.ChipErase }).ConfigureAwait(false);
        }

        public Task WriteEnable()
        {
            return dev.SendReceive(new byte[] { (byte)Command.WriteEnable });
        }

        public Task WriteDisable()
        {
            return dev.SendReceive(new byte[] { (byte)Command.WriteDisable });
        }
    }
}
