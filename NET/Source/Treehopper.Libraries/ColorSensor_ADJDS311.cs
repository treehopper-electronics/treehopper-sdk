using System;
using System.ComponentModel;

namespace Treehopper.Libraries
{
    public class ColorSensor_ADJDS311 : SMBusDevice, INotifyPropertyChanged
    {
        byte CTRL          = 0x00;
        byte CONFIG        = 0x01;

        byte CAP_RED       = 0x06;
        byte CAP_GREEN     = 0x07;
        byte CAP_BLUE      = 0x08;
        byte CAP_CLEAR     = 0x09;

        byte INT_RED_LO    = 0x0A;
        byte INT_RED_HI    = 0x0B;
        byte INT_GREEN_LO  = 0x0C;
        byte INT_GREEN_HI  = 0x0D;
        byte INT_BLUE_LO   = 0x0E;
        byte INT_BLUE_HI   = 0x0F;
        byte INT_CLEAR_LO  = 0x10;
        byte INT_CLEAR_HI  = 0x11;

        byte DATA_RED_LO   = 0x40;
        byte DATA_RED_HI   = 0x41;
        byte DATA_GREEN_LO = 0x42;
        byte DATA_GREEN_HI = 0x43;
        byte DATA_BLUE_LO  = 0x44;
        byte DATA_BLUE_HI  = 0x45;
        byte DATA_CLEAR_LO = 0x46;
        byte DATA_CLEAR_HI = 0x47;

        I2c _I2C;
        Pin led;

        public ColorSensor_ADJDS311(I2c I2CDevice, Pin LedPin) : base(0x74, I2CDevice)
        {
            _I2C = I2CDevice;
            _I2C.Enabled = true;

            led = LedPin;
            led.Mode = PinMode.PushPullOutput;

            WriteByteData(CAP_RED, 15);
            WriteByteData(CAP_GREEN, 15);
            WriteByteData(CAP_BLUE, 15);
            WriteByteData(CAP_CLEAR, 15);

            WriteByteData(INT_RED_LO, 0x00);
            WriteByteData(INT_RED_HI, 0x4);
            WriteByteData(INT_GREEN_LO, 0x00);
            WriteByteData(INT_GREEN_HI, 0x5);
            WriteByteData(INT_BLUE_LO, 0x00);
            WriteByteData(INT_BLUE_HI, 0x9);
            WriteByteData(INT_CLEAR_LO, 0x00);
            WriteByteData(INT_CLEAR_HI, 0x2);
            
            

        }

        public ushort Red { get; set; }
        public ushort Green { get; set; }
        public ushort Blue { get; set; }

        public async void updateColor()
        {
            led.DigitalValue = true;
            WriteByteData(0, 1);
            while ((await ReadByteData(0) & 0x01) != 0) ;
            led.DigitalValue = false;

            ushort red = BitConverter.ToUInt16(new byte[] { await ReadByteData(DATA_RED_LO), await ReadByteData(DATA_RED_HI) }, 0);
            ushort green = BitConverter.ToUInt16(new byte[] { await ReadByteData(DATA_GREEN_LO), await ReadByteData(DATA_GREEN_HI) }, 0);
            ushort blue = BitConverter.ToUInt16(new byte[] { await ReadByteData(DATA_BLUE_LO), await ReadByteData(DATA_BLUE_HI) }, 0);
            ushort clear = BitConverter.ToUInt16(new byte[] { await ReadByteData(DATA_CLEAR_LO), await ReadByteData(DATA_CLEAR_HI) }, 0);

            Red = (byte)(red / 4);
            Green = (byte)(green / 4);
            Blue = (byte)(blue / 4);

            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Red"));
                PropertyChanged(this, new PropertyChangedEventArgs("Green"));
                PropertyChanged(this, new PropertyChangedEventArgs("Blue"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
