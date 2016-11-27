using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;

namespace Treehopper.Libraries.Displays
{
    public class Ssd1306 : GraphicDisplay
    {
        private II2c i2c;
        private byte address;

        public enum DisplaySize
        {
            Pixels128x32,
            Pixels128x64,
            Pixels96x16
        }

        public enum VccMode
        {
            External,
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

        private SMBusDevice dev;

        public bool[,] BoolBuffer { get; set; }

        public Ssd1306(II2c I2c, int width = 128, int height = 32, byte address = 0x3C, VccMode mode = VccMode.SwitchCap) : base(width, height)
        {
            if (!((Width == 128 && Height == 32) || (Width == 128 && Height == 64) || (Width == 96 && Height == 16)))
                throw new ArgumentException("The only supported display sizes are 128x32, 128x64, and 96x16");

            this.dev = new SMBusDevice(address, I2c, 400);

            RawBuffer = new byte[Width * Height / 8 + Width];
            BoolBuffer = new bool[Width, Height];
            sendCommand(Command.DisplayOff);
            sendCommand(Command.SetDisplayClockDiv);
            sendCommand(0x80);

            if (Width == 128 && Height == 32)
            {
                sendCommand(Command.SetComPins);
                sendCommand(0x02);
                sendCommand(Command.SetContrast);
                sendCommand(0x8F);
            }
            else if (Width == 128 && Height == 64)
            {

                sendCommand(Command.SetComPins);
                sendCommand(0x12);
                sendCommand(Command.SetContrast);
                sendCommand(0x9F);
            }
            else
            {
                sendCommand(Command.SetComPins);
                sendCommand(0x02);
                sendCommand(Command.SetContrast);
                sendCommand(0x10);
            }

            sendCommand(Command.SetMultiplex);
            sendCommand((byte)(Height - 1));

            sendCommand(Command.DisplayOffset);
            sendCommand((byte)0x00);

            sendCommand(Command.SetStartLine | 0x0);

            sendCommand(Command.ChargePump);
            if (mode == VccMode.External)
                sendCommand(0x10);
            else
                sendCommand(0x14);
            sendCommand(Command.MemoryMode);
            sendCommand((byte)0x00);

            sendCommand((byte)((byte)Command.SegRemap | 0x1));
            sendCommand(Command.ComScanDec);

            sendCommand(Command.SetPrecharge);
            if (mode == VccMode.External)
                sendCommand(0x22);
            else
                sendCommand(0xF1);

            sendCommand(Command.SetVcomDetect);
            sendCommand(0x40);
            sendCommand(Command.DisplayAllOn_Resume);
            sendCommand(Command.NormalDisplay);
            sendCommand(Command.DeactivateScroll);
            sendCommand(Command.DisplayOn);
            Clear();
            flush();
        }

        private void sendCommand(Command cmd)
        {
            sendCommand((byte)cmd);
        }
        private void sendCommand(byte cmd)
        {
            var dat = new byte[] { 0x00, cmd };
            dev.WriteData(dat).Wait();
        }

        public void Clear()
        {
            //for (int i = 0; i < Width * Height/8; i++)
            //    RawBuffer[i] = 0x00;
            var val = false;
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    BoolBuffer[i, j] = val;
                    //val = !val;
                }
            }
                
                    
        }

        protected override void flush()
        {
            sendCommand(Command.ColumnAddress);
            sendCommand((byte)0);
            sendCommand((byte)(Width - 1));

            sendCommand(Command.PageAddress);
            sendCommand((byte)0);
            switch(Height)
            {
                case 64:
                    sendCommand(7);
                    break;
                case 32:
                    sendCommand(3);
                    break;
                case 16:
                    sendCommand(1);
                    break;
            }

            //for (int i = 0; i < Width * Height / 8; i++)
            //    RawBuffer[i] = 0x00;

            // copy the bool data to the byte buffer
            int k = 0;
            for (int i = 0; i < Width; i++)
            {

                for (int j = 0; j < Height; j++)
                {
                    if (BoolBuffer[i, j])
                        RawBuffer[i + (Width * (j / 8))] |= (byte)(1 << (j % 8));
                    else
                        RawBuffer[i + (Width * (j / 8))] &= (byte)(~(1 << (j % 8)));
                }
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
                dev.WriteData(dat).Wait();
            }

        }

    }
}
