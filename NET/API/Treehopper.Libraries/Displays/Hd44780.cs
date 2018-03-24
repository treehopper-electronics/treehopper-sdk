using System;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    ///     HD44780-compatible character display
    /// </summary>
    [Supports("Hitachi", "HD44780")]
    public class Hd44780 : CharacterDisplay
    {
        /// <summary>
        ///     Whether to use 4 or 8 bits for transactions
        /// </summary>
        public enum BitMode
        {
            /// <summary>
            ///     4-bit mode
            /// </summary>
            FourBit = 0x00,

            /// <summary>
            ///     8-bit mode
            /// </summary>
            EightBit = 0x10
        }

        /// <summary>
        ///     Whether the cursor should blink
        /// </summary>
        public enum BlinkState
        {
            /// <summary>
            ///     The cursor doesn't blink
            /// </summary>
            BlinkOff = 0x00,

            /// <summary>
            ///     The cursor blinks
            /// </summary>
            BlinkOn = 0x01
        }

        /// <summary>
        ///     An enumeration of commands the display supports
        /// </summary>
        public enum Command
        {
            /// <summary>
            ///     Clear the display
            /// </summary>
            ClearDisplay = 0x01,

            /// <summary>
            ///     Return the cursor to top-left
            /// </summary>
            ReturnHome = 0x02,

            /// <summary>
            ///     Entry mode set
            /// </summary>
            EntryModeSet = 0x04,

            /// <summary>
            ///     Set the display mode
            /// </summary>
            DisplayControl = 0x08,

            /// <summary>
            ///     Move the cursor
            /// </summary>
            CursorShift = 0x10,

            /// <summary>
            ///     Control display functions
            /// </summary>
            FunctionSet = 0x20,

            /// <summary>
            ///     Set the CGRAM address to write to
            /// </summary>
            SetCgramAddr = 0x40,

            /// <summary>
            ///     Set DDRAM address to write to
            /// </summary>
            SetDdramAddr = 0x80
        }

        /// <summary>
        ///     The cursor display state
        /// </summary>
        public enum CursorState
        {
            /// <summary>
            ///     Cursor is not displayed
            /// </summary>
            CursorOff = 0x00,

            /// <summary>
            ///     Cursor is displayed
            /// </summary>
            CursorOn = 0x02
        }

        /// <summary>
        ///     The display state
        /// </summary>
        public enum DisplayState
        {
            /// <summary>
            ///     Display off
            /// </summary>
            DisplayOff = 0x00,

            /// <summary>
            ///     Display on
            /// </summary>
            DisplayOn = 0x04
        }

        /// <summary>
        ///     The font mode to use
        /// </summary>
        public enum FontMode
        {
            /// <summary>
            ///     Use 5x8 pixel fonts
            /// </summary>
            Font_5x8 = 0x00,

            /// <summary>
            ///     Use 5x10 pixel fonts
            /// </summary>
            Font_5x10 = 0x04
        }

        /// <summary>
        ///     Whether the display is single or multi-line
        /// </summary>
        public enum LinesMode
        {
            /// <summary>
            ///     One-line mode
            /// </summary>
            OneLine = 0x00,

            /// <summary>
            ///     Two or more lines
            /// </summary>
            TwoOrMoreLines = 0x08
        }

        private readonly DigitalOut backlight;

        private readonly BitMode bits;
        private readonly WriteOnlyParallelInterface iface;
        private readonly LinesMode lines;
        private bool blink;
        private bool cursor;
        private bool display;
        private FontMode font;

        /// <summary>
        ///     Construct a new HD44780-compatible display
        /// </summary>
        /// <param name="iface">The writable parallel interface to use</param>
        /// <param name="Columns">The number of columns in the display</param>
        /// <param name="Rows">The number of rows of the display</param>
        /// <param name="Backlight">The active-high pin to use for the backlight</param>
        /// <param name="font">The font mode to use</param>
        public Hd44780(WriteOnlyParallelInterface iface, int Columns, int Rows, DigitalOut Backlight = null,
            FontMode font = FontMode.Font_5x8) : base(Columns, Rows)
        {
            if (iface.Width == 8)
                bits = BitMode.EightBit;
            else if (iface.Width == 4)
                bits = BitMode.FourBit;
            else
                throw new ArgumentException("The IParallelInterface bus must be either 4 or 8 bits wide.", "iface");

            if (Rows == 1)
                lines = LinesMode.OneLine;
            else
                lines = LinesMode.TwoOrMoreLines;

            this.font = font;
            this.iface = iface;
            iface.DelayMicroseconds = 50;
            iface.Enabled = true;
            byte cmd;

            if (bits == BitMode.FourBit)
            {
                Task.Run(() => iface.WriteCommandAsync(new uint[] {0x03})).Wait();
                Task.Run(() => Task.Delay(10)).Wait();
                Task.Run(() => iface.WriteCommandAsync(new uint[] {0x03})).Wait();
                Task.Run(() => Task.Delay(10)).Wait();
                Task.Run(() => iface.WriteCommandAsync(new uint[] {0x03})).Wait();
                Task.Run(() => Task.Delay(10)).Wait();
                Task.Run(() => iface.WriteCommandAsync(new uint[] {0x02})).Wait();
            }


            cmd = (byte) ((byte) Command.FunctionSet | (byte) bits | (byte) lines | (byte) font);
            Task.Run(() => writeCommand(cmd)).Wait();

            Display = true;
            Task.Run(Clear).Wait();

            if (Backlight != null)
            {
                backlight = Backlight;
                Task.Run(backlight.MakeDigitalPushPullOutAsync).Wait();
            }


            this.Backlight = true;
        }

        /// <summary>
        ///     Enable or disable the backlight
        /// </summary>
        public bool Backlight
        {
            get { return backlight?.DigitalValue ?? false; }
            set
            {
                if (backlight == null) return;
                backlight.DigitalValue = value;
            }
        }

        /// <summary>
        ///     Enable or disable the display
        /// </summary>
        public bool Display
        {
            get { return display; }
            set
            {
                if (display == value) return;
                display = value;
                Task.Run(updateDisplayControl).Wait();
            }
        }

        /// <summary>
        ///     Enable or disable the cursor
        /// </summary>
        public bool Cursor
        {
            get { return cursor; }
            set
            {
                if (cursor == value) return;
                cursor = value;
                Task.Run(updateDisplayControl).Wait();
            }
        }

        /// <summary>
        ///     Enable or disable cursor blinking
        /// </summary>
        public bool Blink
        {
            get { return blink; }
            set
            {
                if (blink == value) return;
                blink = value;
                updateDisplayControl().Wait();
            }
        }

        private Task updateDisplayControl()
        {
            var cmd = (byte) ((byte) Command.DisplayControl |
                              (Display ? (byte) DisplayState.DisplayOn : 0) |
                              (Cursor ? (byte) CursorState.CursorOn : 0) |
                              (Blink ? (byte) BlinkState.BlinkOn : 0));

            return writeCommand(cmd);
        }

        /// <summary>
        ///     Clear the display
        /// </summary>
        /// <returns>An awaitable task that completes when finished</returns>
        protected override async Task clear()
        {
            await writeCommand(Command.ClearDisplay).ConfigureAwait(false);
            await Task.Delay(10).ConfigureAwait(false);
        }

        /// <summary>
        ///     Update the cursor position
        /// </summary>
        /// <returns>An awaitable task that completes when finished</returns>
        protected override Task updateCursorPosition()
        {
            byte[] row_offsets = {0x00, 0x40, (byte) Columns, (byte) (0x40 + Columns)};
            var data = (byte) (CursorLeft + row_offsets[CursorTop]);

            var cmd = (byte) ((byte) Command.SetDdramAddr | data);
            return writeCommand(cmd);
        }

        /// <summary>
        ///     Write data to the display at the current cursor position
        /// </summary>
        /// <param name="value">The data to write</param>
        /// <returns>An awaitable task that completes when finished</returns>
        protected override Task write(dynamic value)
        {
            string str = value.ToString();
            var data = new byte[str.Length];
            for (var i = 0; i < str.Length; i++)
                data[i] = (byte) str[i];
            return writeData(data);
        }

        private Task writeCommand(Command cmd)
        {
            return writeCommand((byte) cmd);
        }

        private Task writeCommand(byte cmd)
        {
            if (bits == BitMode.EightBit)
                return iface.WriteCommandAsync(new uint[] {cmd});
            return iface.WriteCommandAsync(new[] {(uint) (cmd >> 4), (uint) (cmd & 0x0f)}); // send high nib, then low nib
        }

        private Task writeData(byte[] data)
        {
            uint[] dataToSend;
            if (bits == BitMode.EightBit)
            {
                dataToSend = new uint[data.Length];

                for (var i = 0; i < data.Length; i++)
                    dataToSend[i] = data[i];
            }
            else
            {
                dataToSend = new uint[data.Length * 2];
                for (var i = 0; i < data.Length; i++)
                {
                    // send high nib, then low nib
                    dataToSend[i * 2] = (byte) (data[i] >> 4);
                    dataToSend[i * 2 + 1] = (byte) (data[i] & 0x0f);
                }
            }

            return iface.WriteDataAsync(dataToSend);
        }
    }
}