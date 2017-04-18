using System;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    /// Solomon SSD1306 128x64 OLED display controller
    /// </summary>
    public class Ssd1306 : MonoGraphicDisplay
    {
        /// <summary>
        /// The size of the pixel array
        /// </summary>
        public enum DisplaySize
        {
            /// <summary>
            /// 128x32 display
            /// </summary>
            Pixels128x32,

            /// <summary>
            /// 128x64 display
            /// </summary>
            Pixels128x64,

            /// <summary>
            /// 96x16 display
            /// </summary>
            Pixels96x16
        }

        /// <summary>
        /// Whether VCC is supplied externally or internally
        /// </summary>
        public enum VccMode
        {
            /// <summary>
            /// External VCC supply
            /// </summary>
            External,

            /// <summary>
            /// Switched-capacitor VCC supply
            /// </summary>
            SwitchCap
        }
        enum Command
        {
            SetContrast = 0x81,
            DisplayAllOn_Resume = 0xA4,
            DisplayAllOn = 0xA5,
            NormalDisplay = 0xA6,
            InvertDisplay = 0xA7,
            DisplayOff = 0xAE,
            DisplayOn = 0xAF,

            DisplayOffset = 0xD3,
            SetComPins = 0xDA,

            SetVcomDetect = 0xDB,

            SetDisplayClockDiv = 0xD5,
            SetPrecharge = 0xD9,
            SetMultiplex = 0xA8,
            
            SetLowColumn = 0x00,
            SetHighColumn = 0x10,

            SetStartLine = 0x40,

            MemoryMode = 0x20,
            ColumnAddress = 0x21,
            PageAddress = 0x22,

            ComScanInc = 0xC0,
            ComScanDec = 0xC8,

            SegRemap = 0xA0,

            ChargePump = 0x8D,

            ActivateScroll = 0x2F,
            DeactivateScroll = 0x2E,
            SetVerticalScrollArea = 0xA3,
            RightHorizontalScroll = 0x26,
            LeftHorizontalScroll = 0x27,
            VerticalAndRightHorizontalScroll = 0x29,
            VerticalAndLeftHorizontalScroll = 0x2A,

        }

        private readonly SMBusDevice dev;

        /// <summary>
        /// A bool array representing pixel states
        /// </summary>
        public bool[,] BoolBuffer { get; set; }

        /// <summary>
        /// Construct a new SSD1306 OLED display
        /// </summary>
        /// <param name="I2c">The I2c module to use</param>
        /// <param name="width">The number of pixels wide</param>
        /// <param name="height">The number of pixels tall</param>
        /// <param name="address">The address</param>
        /// <param name="mode">The VCC mode of the display</param>
        public Ssd1306(I2c I2c, int width = 128, int height = 32, byte address = 0x3C, VccMode mode = VccMode.SwitchCap) : base(width, height)
        {
            if (!((Width == 128 && Height == 32) || (Width == 128 && Height == 64) || (Width == 96 && Height == 16)))
                throw new ArgumentException("The only supported display sizes are 128x32, 128x64, and 96x16");

            dev = new SMBusDevice(address, I2c, 400);

            RawBuffer = new byte[Width * Height / 8 + Width];
            BoolBuffer = new bool[Width, Height];

            this.mode = mode;
            InitAsync().Wait();
        }

        private readonly VccMode mode;

        private async Task InitAsync()
        {
            await sendCommand(Command.DisplayOff).ConfigureAwait(false);
            await sendCommand(Command.SetDisplayClockDiv).ConfigureAwait(false);
            await sendCommand(0x80).ConfigureAwait(false);

            if (Width == 128 && Height == 32)
            {
                await sendCommand(Command.SetComPins).ConfigureAwait(false);
                await sendCommand(0x02).ConfigureAwait(false);
                await sendCommand(Command.SetContrast).ConfigureAwait(false);
                await sendCommand(0x8F).ConfigureAwait(false);
            }
            else if (Width == 128 && Height == 64)
            {

                await sendCommand(Command.SetComPins).ConfigureAwait(false);
                await sendCommand(0x12).ConfigureAwait(false);
                await sendCommand(Command.SetContrast).ConfigureAwait(false);
                await sendCommand(0x9F).ConfigureAwait(false);
            }
            else
            {
                await sendCommand(Command.SetComPins).ConfigureAwait(false);
                await sendCommand(0x02).ConfigureAwait(false);
                await sendCommand(Command.SetContrast).ConfigureAwait(false);
                await sendCommand(0x10).ConfigureAwait(false);
            }

            await sendCommand(Command.SetMultiplex).ConfigureAwait(false);
            await sendCommand((byte)(Height - 1)).ConfigureAwait(false);

            await sendCommand(Command.DisplayOffset).ConfigureAwait(false);
            await sendCommand((byte)0x00).ConfigureAwait(false);

            await sendCommand(Command.SetStartLine | 0x0).ConfigureAwait(false);

            await sendCommand(Command.ChargePump).ConfigureAwait(false);
            if (mode == VccMode.External)
                await sendCommand(0x10).ConfigureAwait(false);
            else
                await sendCommand(0x14).ConfigureAwait(false);
            await sendCommand(Command.MemoryMode).ConfigureAwait(false);
            await sendCommand((byte)0x00).ConfigureAwait(false);

            await sendCommand((byte)((byte)Command.SegRemap | 0x1)).ConfigureAwait(false);
            await sendCommand(Command.ComScanDec).ConfigureAwait(false);

            await sendCommand(Command.SetPrecharge).ConfigureAwait(false);
            if (mode == VccMode.External)
                await sendCommand(0x22).ConfigureAwait(false);
            else
                await sendCommand(0xF1).ConfigureAwait(false);

            await sendCommand(Command.SetVcomDetect).ConfigureAwait(false);
            await sendCommand(0x40);
            await sendCommand(Command.DisplayAllOn_Resume).ConfigureAwait(false);
            await sendCommand(Command.NormalDisplay).ConfigureAwait(false);
            await sendCommand(Command.DeactivateScroll).ConfigureAwait(false);
            await sendCommand(Command.DisplayOn).ConfigureAwait(false);

            await Clear();
            await flush().ConfigureAwait(false);
        }

        private Task sendCommand(Command cmd)
        {
            return sendCommand((byte)cmd);
        }
        private Task sendCommand(byte cmd)
        {
            var dat = new byte[] { 0x00, cmd };
            return dev.WriteData(dat);
        }

        /// <summary>
        /// Flush the display
        /// </summary>
        /// <returns>An awaitable task that completes when finished</returns>
        protected override async Task flush()
        {
            await sendCommand(Command.ColumnAddress).ConfigureAwait(false);
            await sendCommand((byte)0).ConfigureAwait(false);
            await sendCommand((byte)(Width - 1)).ConfigureAwait(false);

            await sendCommand(Command.PageAddress).ConfigureAwait(false);
            await sendCommand((byte)0).ConfigureAwait(false);
            switch(Height)
            {
                case 64:
                    await sendCommand(7).ConfigureAwait(false);
                    break;
                case 32:
                    await sendCommand(3).ConfigureAwait(false);
                    break;
                case 16:
                    await sendCommand(1).ConfigureAwait(false);
                    break;
            }

            for (int i=0;i < (Width*Height / 8); i++)
            {
                var dat = new byte[128+1];
                dat[0] = 0x40;
                for (int j = 0; j < 128; j++)
                {
                    dat[j + 1] = RawBuffer[i];
                    i++;
                }
                i--;
                await dev.WriteData(dat).ConfigureAwait(false);
            }

        }

        /// <summary>
        /// Set the global brightness of this display
        /// </summary>
        /// <param name="brightness">The brightness, from 0-1, of this display</param>
        protected override void setBrightness(double brightness)
        {
            
        }
    }
}
