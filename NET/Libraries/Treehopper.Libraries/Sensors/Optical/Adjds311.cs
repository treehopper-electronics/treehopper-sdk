using System;
using System.ComponentModel;

namespace Treehopper.Libraries.Sensors.Optical
{
    /// <summary>
    ///     Avago ADJD-S311 ambient light sensor
    /// </summary>
    [Supports("Avago", "ADJD-S311")]
    public class Adjds311 : SMBusDevice, INotifyPropertyChanged
    {
        private readonly I2C _I2C;
        private readonly byte CAP_BLUE = 0x08;
        private readonly byte CAP_CLEAR = 0x09;
        private readonly byte CAP_GREEN = 0x07;

        private readonly byte CAP_RED = 0x06;
        private readonly byte DATA_BLUE_HI = 0x45;
        private readonly byte DATA_BLUE_LO = 0x44;
        private readonly byte DATA_CLEAR_HI = 0x47;
        private readonly byte DATA_CLEAR_LO = 0x46;
        private readonly byte DATA_GREEN_HI = 0x43;
        private readonly byte DATA_GREEN_LO = 0x42;
        private readonly byte DATA_RED_HI = 0x41;

        private readonly byte DATA_RED_LO = 0x40;
        private readonly byte INT_BLUE_HI = 0x0F;
        private readonly byte INT_BLUE_LO = 0x0E;
        private readonly byte INT_CLEAR_HI = 0x11;
        private readonly byte INT_CLEAR_LO = 0x10;
        private readonly byte INT_GREEN_HI = 0x0D;
        private readonly byte INT_GREEN_LO = 0x0C;
        private readonly byte INT_RED_HI = 0x0B;

        private readonly byte INT_RED_LO = 0x0A;
        private readonly DigitalOut led;
        private byte CONFIG = 0x01;
        private byte CTRL = 0x00;

        /// <summary>
        ///     Create a ADJDS311 device
        /// </summary>
        /// <param name="I2CDevice">The I2c port to use</param>
        /// <param name="LedPin">The pin attached to the led</param>
        public Adjds311(I2C I2CDevice, DigitalOut LedPin) : base(0x74, I2CDevice)
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
        ///     Red data
        /// </summary>
        public ushort Red { get; set; }

        /// <summary>
        ///     Green data
        /// </summary>
        public ushort Green { get; set; }

        /// <summary>
        ///     Blue data
        /// </summary>
        public ushort Blue { get; set; }

        /// <summary>
        ///     Occurs when any property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Get new color data
        /// </summary>
        public async void UpdateColor()
        {
            led.DigitalValue = true;
            await WriteByteData(0, 1);
            while ((await ReadByteData(0) & 0x01) != 0) ;
            led.DigitalValue = false;

            var red = BitConverter.ToUInt16(new[] {await ReadByteData(DATA_RED_LO), await ReadByteData(DATA_RED_HI)},
                0);
            var green = BitConverter.ToUInt16(
                new[] {await ReadByteData(DATA_GREEN_LO), await ReadByteData(DATA_GREEN_HI)}, 0);
            var blue = BitConverter.ToUInt16(new[] {await ReadByteData(DATA_BLUE_LO), await ReadByteData(DATA_BLUE_HI)},
                0);
            var clear = BitConverter.ToUInt16(
                new[] {await ReadByteData(DATA_CLEAR_LO), await ReadByteData(DATA_CLEAR_HI)}, 0);

            Red = (byte) (red / 4);
            Green = (byte) (green / 4);
            Blue = (byte) (blue / 4);

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Red"));
                PropertyChanged(this, new PropertyChangedEventArgs("Green"));
                PropertyChanged(this, new PropertyChangedEventArgs("Blue"));
            }
        }
    }
}