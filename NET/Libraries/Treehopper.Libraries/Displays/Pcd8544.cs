using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    public class Pcd8544 : MonoGraphicDisplay
    {
        private DigitalOutPin dc;
        private DigitalOutPin rst;
        private SpiDevice spi;

        private enum Command
        {
            EnterExtendedCommandMode = 0x21,
            ExitExtendedCommandMode = 0x20,

            DisplayOff = 0x08,
            DisplayOn = 0x0C,

            /// <summary>
            /// Set the display bias (n = 3 is suggested). Requires extended mode
            /// </summary>
            SetBias = 0x10,

            /// <summary>
            /// Set Vop. Requires extended mode
            /// </summary>
            SetVop = 0x80,

            SetTempCoefficient = 0x04,

            SetY = 0x40,
            SetX = 0x80,
        }

        public Pcd8544(Spi spi, SpiChipSelectPin csPin, DigitalOutPin dc, DigitalOutPin rst, byte biasValue = 0x04) : base(84, 48)
        {
            this.spi = new SpiDevice(spi, csPin, ChipSelectMode.SpiActiveLow, 6);

            this.dc = dc;
            this.rst = rst;

            this.dc.MakeDigitalPushPullOut();
            this.rst.MakeDigitalPushPullOut();

            this.rst.DigitalValue = true;
            this.rst.DigitalValue = false;
            this.rst.DigitalValue = true;

            sendCommand(Command.EnterExtendedCommandMode).Wait();
            sendCommand(Command.SetVop, 0x30).Wait();
            sendCommand(Command.SetTempCoefficient, 0x00).Wait();
            sendCommand(Command.SetBias, biasValue).Wait();

            sendCommand(Command.ExitExtendedCommandMode).Wait();
            sendCommand(Command.DisplayOn).Wait();
        }

        protected async override Task flush()
        {
            // reset the pointer to (0, 0);
            await sendCommand(Command.SetX);
            await sendCommand(Command.SetY);

            dc.DigitalValue = true;
            var chunk = new byte[252];

            // we have to spit up the display buffer into two chunks so we don't violate the max-size of the SPI transfer
            Array.Copy(this.RawBuffer, 0, chunk, 0, 252);
            await spi.SendReceive(chunk, SpiBurstMode.BurstTx);

            Array.Copy(this.RawBuffer, 252, chunk, 0, 252);
            await spi.SendReceive(chunk, SpiBurstMode.BurstTx);
        }

        protected override void setBrightness(double brightness)
        {
            
        }

        private Task sendCommand(Command command, byte value = 0)
        {
            dc.DigitalValue = false;
            return spi.SendReceive(new byte[] { (byte)((byte)command | value) });
        }
    }
}
