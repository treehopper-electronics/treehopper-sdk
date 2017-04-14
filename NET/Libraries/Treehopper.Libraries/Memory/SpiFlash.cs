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

        public enum Command
        {
            BlockErase_4k = 0x20,
            BlockErase_32k = 0x52,
            BlockErase_64k = 0xD8,
            ChipErase = 0x60,
            StatusRead = 0x05,
            StatusWrite = 0x01,
            ArrayRead = 0x0B,
            ArrayReadLowFreq = 0x03,
            Sleep = 0xB9,
            Wake = 0xAB,
            BytePageProgram = 0x02,
            IdRead = 0x09,
            MacRead = 0x4B
        }

        public enum WriteProtect
        {
            WriteEnable = 0x06,
            WriteDisable = 0x04
        }


        public SpiFlash(Spi dev, SpiChipSelectPin cs)
        {
            this.dev = new SpiDevice(dev, cs);

        }

        public async Task<ushort> ReadDeviceID()
        {
            var result = await dev.SendReceive(new byte[3] { (byte)Command.IdRead, 0x00, 0x00 });

            return (ushort)(result[1] << 8 | result[2]);
        }

        async Task command(Command cmd, bool isWrite = false)
        {
            if (isWrite)
                await command((Command)WriteProtect.WriteEnable);

            await dev.SendReceive(new byte[] { (byte)cmd });
        }
    }
}
