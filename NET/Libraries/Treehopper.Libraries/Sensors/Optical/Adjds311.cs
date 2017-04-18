using System;
using System.ComponentModel;

namespace Treehopper.Libraries.Sensors.Optical
{
    /// <summary>
    /// Avago ADJD-S311 ambient light sensor
    /// </summary>
    public class Adjds311 : SMBusDevice, INotifyPropertyChanged
    {
        byte CTRL          = 0x00;
        byte CONFIG        = 0x01;

        readonly byte CAP_RED       = 0x06;
        readonly byte CAP_GREEN     = 0x07;
        readonly byte CAP_BLUE      = 0x08;
        readonly byte CAP_CLEAR     = 0x09;

        readonly byte INT_RED_LO    = 0x0A;
        readonly byte INT_RED_HI    = 0x0B;
        readonly byte INT_GREEN_LO  = 0x0C;
        readonly byte INT_GREEN_HI  = 0x0D;
        readonly byte INT_BLUE_LO   = 0x0E;
        readonly byte INT_BLUE_HI   = 0x0F;
        readonly byte INT_CLEAR_LO  = 0x10;
        readonly byte INT_CLEAR_HI  = 0x11;

        readonly byte DATA_RED_LO   = 0x40;
        readonly byte DATA_RED_HI   = 0x41;
        readonly byte DATA_GREEN_LO = 0x42;
        readonly byte DATA_GREEN_HI = 0x43;
        readonly byte DATA_BLUE_LO  = 0x44;
        readonly byte DATA_BLUE_HI  = 0x45;
        readonly byte DATA_CLEAR_LO = 0x46;
        readonly byte DATA_CLEAR_HI = 0x47;

        readonly I2c _I2C;
        readonly DigitalOut led;

        /// <summary>
        /// Create a ADJDS311 device
        /// </summary>
        /// <param name="I2CDevice">The I2c port to use</param>
        /// <param name="LedPin">The pin attached to the led</param>
        public Adjds311(I2c I2CDevice, DigitalOut LedPin) : base(0x74, I2CDevice)
        {
            _I2C = I2CDevice;
            _I2C.Enabled = true;

            led = LedPin;
            led.MakeDigitalPushPullOut();

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
        /// <summary>
        /// Red data
        /// </summary>
        public ushort Red { get; set; }

        /// <summary>
        /// Green data
        /// </summary>
        public ushort Green { get; set; }

        /// <summary>
        /// Blue data
        /// </summary>
        public ushort Blue { get; set; }

        /// <summary>
        /// Get new color data
        /// </summary>
        public async void UpdateColor()
        {
            led.DigitalValue = true;
            await WriteByteData(0, 1);
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

        /// <summary>
        /// Occurs when any property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
