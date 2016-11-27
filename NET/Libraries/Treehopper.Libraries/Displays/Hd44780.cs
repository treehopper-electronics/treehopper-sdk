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
        private BitMode bits;
        private LinesMode lines;
        private FontMode font;
        private IWriteOnlyParallelInterface iface;


        public Hd44780(IWriteOnlyParallelInterface iface, int Columns, int Rows, IDigitalOutPin Backlight = null, FontMode font = FontMode.Font_5x8) : base(Columns, Rows)
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

            if(bits == BitMode.FourBit)
            {
                iface.WriteCommand(new uint[] { 0x03});
                Task.Delay(10).Wait();
                iface.WriteCommand(new uint[] { 0x03});
                Task.Delay(10).Wait();
                iface.WriteCommand(new uint[] { 0x03});
                Task.Delay(10).Wait();
                iface.WriteCommand(new uint[] { 0x02});
            }


            cmd = (byte)((byte)Command.FunctionSet | (byte)bits | (byte)lines | (byte)font);
            writeCommand(cmd);

            Display = true;
            Clear();

            if (Backlight != null)
            {
                this.backlight = Backlight;
                backlight.MakeDigitalPushPullOut();
            }
                

            this.Backlight = true;
        }

        public bool Backlight
        {
            get {
                return backlight?.DigitalValue ?? false;
            }
            set {
                if (backlight == null) return;
                backlight.DigitalValue = value;
            }
        }

        public enum BitMode
        {
            FourBit = 0x00,
            EightBit = 0x10
        }

        public enum FontMode
        {
            Font_5x8 = 0x00,
            Font_5x10   = 0x04
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

        private bool display;
        public bool Display
        {
            get { return display; }
            set
            {
                if (display == value) return;
                display = value;
                updateDisplayControl();
            }
        }

        private bool cursor;
        public bool Cursor
        {
            get { return cursor; }
            set
            {
                if (cursor == value) return;
                cursor = value;
                updateDisplayControl();
            }
        }

        private bool blink;
        private IDigitalOutPin backlight;

        public bool Blink
        {
            get { return blink; }
            set
            {
                if (blink == value) return;
                blink = value;
                updateDisplayControl();
            }
        }

        private void updateDisplayControl()
        {
            byte cmd = (byte)((byte)Command.DisplayControl |
                (Display ? (byte)DisplayState.DisplayOn : 0) |
                (Cursor ? (byte)CursorState.CursorOn : 0) |
                (Blink ? (byte)BlinkState.BlinkOn : 0));

            writeCommand(cmd);
        }

        protected override void clear()
        {
            writeCommand(Command.ClearDisplay);
            Task.Delay(10).Wait();
        }

        protected override void updateCursorPosition()
        {
            byte[] row_offsets = new byte[] { 0x00, 0x40, (byte)Columns, (byte)(0x40 + Columns) };
            byte data = (byte)(CusorLeft + row_offsets[CusorTop]);

            byte cmd = (byte)((byte)Command.SetDdramAddr | data);
            writeCommand(cmd);
        }

        protected override void write(dynamic value)
        {
            string str = value.ToString();
            var data = new byte[str.Length];
            for(int i=0;i<str.Length;i++)
            {
                data[i] = (byte)str[i];
            }
            writeData(data);
        }

        protected void writeCommand(Command cmd)
        {
            writeCommand((byte)cmd);
        }
        protected void writeCommand(byte cmd)
        {
            if(bits == BitMode.EightBit)
                iface.WriteCommand(new uint[] { cmd });
            else
            {
                iface.WriteCommand(new uint[] { (uint)(cmd >> 4), (uint)(cmd & 0x0f) }); // send high nib, then low nib
            }
        }

        protected void writeData(byte[] data)
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

            iface.WriteData(dataToSend);
        }


        
    }
}
