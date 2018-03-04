using System;
using System.Linq;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Memory
{
    public class SpiFlash
    {
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
            ReadSecurityRegister = 0x48
        }

        public enum JedecManufacturer
        {
            WinbondNexcom = 0xEF
        }

        public enum StatusProtectMode
        {
            SoftwareProtection,
            HardwareProtection,
            PowerSupplyLockDown,
            OneTimeProgram
        }

        private readonly SpiDevice dev;

        public SpiFlash(Spi dev, SpiChipSelectPin cs)
        {
            this.dev = new SpiDevice(dev, cs, ChipSelectMode.SpiActiveLow, 6);
        }

        public async Task<JedecId> ReadJedecIdAsync()
        {
            var result = await dev.SendReceiveAsync(new byte[4] {(byte) Command.ReadJedecId, 0x00, 0x00, 0x00});

            var id = new JedecId();
            id.Manufacturer = (JedecManufacturer) result[1];
            id.MemoryType = result[2];
            id.CapacityId = result[3];

            return id;
        }

        public async Task<Status> ReadStatusAsync()
        {
            var cmd = new byte[2];
            cmd[0] = (byte) Command.ReadStatus1;
            var status1 = await dev.SendReceiveAsync(cmd);

            cmd[0] = (byte) Command.ReadStatus2;
            var status2 = await dev.SendReceiveAsync(cmd);

            cmd[0] = (byte) Command.ReadStatus3;
            var status3 = await dev.SendReceiveAsync(cmd);

            var status = new Status();

            status.Busy = (status1[1] & 1) > 0 ? true : false;
            status.WriteEnableLatch = (status1[1] & 2) > 0 ? true : false;
            status.Blockprotect = (status1[1] >> 2) & 0x07;
            status.TopBottomProtect = (status1[1] & 32) > 0 ? true : false;
            status.SectorProtect = (status1[1] & 64) > 0 ? true : false;
            status.StatusProtection = (StatusProtectMode) ((status1[1] >> 7) | (status2[1] & 0x01));

            return status;
        }

        public async Task<byte[]> ReadArrayAsync(int address, int count)
        {
            while ((await ReadStatusAsync().ConfigureAwait(false)).Busy)
            {
            }

            var data = new byte[count + 4];
            data[0] = (byte) Command.ReadData;
            data[1] = (byte) (address >> 16);
            data[2] = (byte) (address >> 8);
            data[3] = (byte) address;
            var result = await dev.SendReceiveAsync(data);
            return result.Skip(4).Take(count).ToArray();
        }

        public async Task<byte> ReadByteAsync(int address)
        {
            var res = await ReadArrayAsync(address, 1);
            return res[0];
        }

        public async Task Write(byte[] data, int address)
        {
            //var check = await ReadArray(address, data.Length).ConfigureAwait(false);
            while ((await ReadStatusAsync().ConfigureAwait(false)).Busy)
            {
            }

            await WriteEnable().ConfigureAwait(false);

            var status = await ReadStatusAsync().ConfigureAwait(false);
            if (status.WriteEnableLatch != true)
                throw new Exception("Write Enable Latch is not set. Check write-protect");
            var header = new byte[4];
            header[0] = (byte) Command.PageProgram;
            header[1] = (byte) (address >> 16);
            header[2] = (byte) (address >> 8);
            header[3] = (byte) address;

            await dev.SendReceiveAsync(header.Concat(data).ToArray()).ConfigureAwait(false);
        }

        public async Task EraseChip()
        {
            while ((await ReadStatusAsync().ConfigureAwait(false)).Busy)
            {
            }

            await WriteEnable().ConfigureAwait(false);
            var status = await ReadStatusAsync().ConfigureAwait(false);
            if (status.WriteEnableLatch != true)
                throw new Exception("Write Enable Latch is not set. Check write-protect");

            await dev.SendReceiveAsync(new[] {(byte) Command.ChipErase}).ConfigureAwait(false);
            while ((await ReadStatusAsync().ConfigureAwait(false)).Busy)
                await Task.Delay(100);
        }

        public Task WriteEnable()
        {
            return dev.SendReceiveAsync(new[] {(byte) Command.WriteEnable});
        }

        public Task WriteDisable()
        {
            return dev.SendReceiveAsync(new[] {(byte) Command.WriteDisable});
        }

        public async Task WriteStatus1(byte val)
        {
            await WriteEnable();
            await dev.SendReceiveAsync(new[] {(byte) Command.WriteStatus1, val});
        }

        public async Task WriteStatus2(byte val)
        {
            await WriteEnable();
            await dev.SendReceiveAsync(new[] {(byte) Command.WriteStatus2, val});
        }

        public async Task WriteStatus3(byte val)
        {
            await WriteEnable();
            await dev.SendReceiveAsync(new[] {(byte) Command.WriteStatus3, val});
        }

        public class JedecId
        {
            public JedecManufacturer Manufacturer { get; set; }
            public byte MemoryType { get; set; }
            public byte CapacityId { get; set; }
        }

        public struct Status
        {
            public bool Busy;
            public bool WriteEnableLatch;
            public int Blockprotect;
            public bool TopBottomProtect;
            public bool SectorProtect;
            public StatusProtectMode StatusProtection;
            public int SecurityLockBits;
            public bool ComplementProtect;
            public bool IsSuspended;
        }
    }
}