using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;

namespace Treehopper.Libraries.Displays
{
    public class Hd44780 : CharacterDisplay
    {
        public enum BitMode
        {
            FourBit = 0x00,
            EightBit = 0x10
        }

        public enum FontMode
        {
            Font_5x8 = 0x00,
            Font_5x10 = 0x04
        }

        public enum LinesMode
        {
            OneLine = 0x00,
            TwoLine = 0x08
        }

        public enum DisplayState
        {
            DisplayOff = 0x00,
            DisplayOn = 0x04
        }

        public enum CursorState
        {
            CursorOff = 0x00,
            CursorOn = 0x02
        }

        public enum BlinkState
        {
            BlinkOff = 0x00,
            BlinkOn = 0x01
        }

        public enum Command
        {
            ClearDisplay = 0x01,
            ReturnHome = 0x02,
            EntryModeSet = 0x04,
            DisplayControl = 0x08,
            CursorShift = 0x10,
            FunctionSet = 0x20,
            SetCgramAddr = 0x40,
            SetDdramAddr = 0x80
        }

        private BitMode bits;
        private LinesMode lines;
        private FontMode font;
        private WriteOnlyParallelInterface iface;
        private bool display;
        private bool cursor;
        private bool blink;
        private DigitalOutPin backlight;

        public Hd44780(WriteOnlyParallelInterface iface, int Columns, int Rows, DigitalOutPin Backlight = null, FontMode font = FontMode.Font_5x8) : base(Columns, Rows)
        {
            if (iface.Width == 8)
                this.bits = BitMode.EightBit;
            else if (iface.Width == 4)
                this.bits = BitMode.FourBit;
            else
                throw new ArgumentException("The IParallelInterface bus must be either 4 or 8 bits wide.", "iface");

            if (Rows == 1)
                this.lines = LinesMode.OneLine;
            else
                this.lines = LinesMode.TwoLine;

            this.font = font;
            this.iface = iface;
            iface.DelayMicroseconds = 50;
            iface.Enabled = true;
            byte cmd;

            if (bits == BitMode.FourBit)
            {
                iface.WriteCommand(new uint[] { 0x03 }).Wait();
                Task.Delay(10).Wait();
                iface.WriteCommand(new uint[] { 0x03 }).Wait();
                Task.Delay(10).Wait();
                iface.WriteCommand(new uint[] { 0x03 }).Wait();
                Task.Delay(10).Wait();
                iface.WriteCommand(new uint[] { 0x02 }).Wait();
            }


            cmd = (byte)((byte)Command.FunctionSet | (byte)bits | (byte)lines | (byte)font);
            writeCommand(cmd).Wait();

            Display = true;
            Clear().Wait();

            if (Backlight != null)
            {
                this.backlight = Backlight;
                backlight.MakeDigitalPushPullOut();
            }


            this.Backlight = true;
        }

        public bool Backlight
        {
            get
            {
                return backlight?.DigitalValue ?? false;
            }
            set
            {
                if (backlight == null) return;
                backlight.DigitalValue = value;
            }
        }

        public bool Display
        {
            get { return display; }
            set
            {
                if (display == value) return;
                display = value;
                updateDisplayControl().Wait();
            }
        }

        public bool Cursor
        {
            get { return cursor; }
            set
            {
                if (cursor == value) return;
                cursor = value;
                updateDisplayControl().Wait();
            }
        }

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
            byte cmd = (byte)((byte)Command.DisplayControl |
                (Display ? (byte)DisplayState.DisplayOn : 0) |
                (Cursor ? (byte)CursorState.CursorOn : 0) |
                (Blink ? (byte)BlinkState.BlinkOn : 0));

            return writeCommand(cmd);
        }

        protected override async Task clear()
        {
            await writeCommand(Command.ClearDisplay).ConfigureAwait(false);
            await Task.Delay(10).ConfigureAwait(false);
        }

        protected override Task updateCursorPosition()
        {
            byte[] row_offsets = new byte[] { 0x00, 0x40, (byte)Columns, (byte)(0x40 + Columns) };
            byte data = (byte)(CusorLeft + row_offsets[CusorTop]);

            byte cmd = (byte)((byte)Command.SetDdramAddr | data);
            return writeCommand(cmd);
        }

        protected override Task write(dynamic value)
        {
            string str = value.ToString();
            var data = new byte[str.Length];
            for(int i=0;i<str.Length;i++)
            {
                data[i] = (byte)str[i];
            }
            return writeData(data);
        }

        protected Task writeCommand(Command cmd)
        {
            return writeCommand((byte)cmd);
        }
        protected Task writeCommand(byte cmd)
        {
            if(bits == BitMode.EightBit)
                return iface.WriteCommand(new uint[] { cmd });
            else
            {
                return iface.WriteCommand(new uint[] { (uint)(cmd >> 4), (uint)(cmd & 0x0f) }); // send high nib, then low nib
            }
        }

        protected Task writeData(byte[] data)
        {
            uint[] dataToSend;
            if (bits == BitMode.EightBit)
            {
                dataToSend = new uint[data.Length];

                for (int i = 0; i < data.Length; i++)
                    dataToSend[i] = data[i];
            }
            else
            {
                dataToSend = new uint[data.Length * 2];
                for(int i=0;i<data.Length;i++)
                {
                    // send high nib, then low nib
                    dataToSend[i * 2] = (byte)(data[i] >> 4);
                    dataToSend[i * 2 + 1] = (byte)(data[i] & 0x0f);
                }
            }

            return iface.WriteData(dataToSend);
        }


        
    }
}
