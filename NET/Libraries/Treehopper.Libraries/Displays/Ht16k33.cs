namespace Treehopper.Libraries.Displays
{
    using System;
    using System.Collections;
    using System.Threading.Tasks;
    using Utilities;

    /// <summary>
    /// Library for the HT16K33 16x8 LED driver, commonly used on Adafruit matrix displays
    /// </summary>
    public class Ht16k33 : LedDriver
    {
        private SMBusDevice dev;
        private BitArray data = new BitArray(128);
        private Package package;

        /// <summary>
        /// Construct a new HT16K33 16x8 LED driver
        /// </summary>
        /// <param name="i2c">The I2c port to use with this display.</param>
        /// <param name="address">The 7-bit I2c address to use</param>
        /// <param name="package">Which package is used</param>
        public Ht16k33(I2c i2c, byte address, Package package) : base((int)package, true, false)
        {
            dev = new SMBusDevice(address, i2c);
            dev.WriteByte(0x21);
            Brightness = 1.0;
            this.package = package;
        }

        /// <summary>
        /// Construct a new HT16K33 16x8 LED driver
        /// </summary>
        /// <param name="i2c">The I2c port to use with this display.</param>
        /// <param name="package">Which package to use</param>
        /// <param name="a0">The state of the A0 address pin</param>
        /// <param name="a1">The state of the A1 address pin</param>
        /// <param name="a2">The state of the A2 address pin</param>
        public Ht16k33(I2c i2c, Package package, bool a0 = false, bool a1 = false, bool a2 = false) : this(i2c, (byte)(0x70 | (a0 ? 1 : 0) | (a1 ? 1 : 0) << 1 | (a2 ? 1 : 0) << 2), package)
        {
        }

        /// <summary>
        /// The package types of the HT16K33. Larger packages support more LEDs.
        /// </summary>
        public enum Package
        {
            /// <summary>
            /// The SOP-20 package, supporting 64 (8x8) LEDs 
            /// </summary>
            Sop20_64Led = 64,

            /// <summary>
            /// The SOP-24 package, supporting 96 (12x8) LEDs
            /// </summary>
            Sop24_96Led = 96,

            /// <summary>
            /// The SOP-28 package, supporting 128 (16x8) LEDs
            /// </summary>
            Sop28_128Led = 128
        }

        private enum Commands
        {
            Brightness = 0xe0,
            Blink = 0x80
        }

        /// <summary>
        /// Flush the LED states to the HT16K33 driver
        /// </summary>
        /// <param name="force">Whether to force an update, even if no data appears to have changed</param>
        /// <returns></returns>
        public override Task Flush(bool force = false)
        {
            return dev.WriteBufferData(0x00, data.GetBytes());
        }

        internal override void LedBrightnessChanged(Led led)
        {
        }

        internal override void LedStateChanged(Led led)
        {
            int index = (16 * (led.Channel / ((int)package / 8))) + (led.Channel % ((int)package / 8));

            data.Set(index, led.State);
            if (AutoFlush)
            {
                // if we're autoflushing, only send the LED we need to update
                byte address = (byte)(index / 8);
                dev.WriteByteData(address, data.GetBytes()[address]).Wait();
            }
        }

        internal override void SetGlobalBrightness(double brightness)
        {
            if (brightness > 0)
            {
                dev.WriteByte((byte)((byte)Commands.Brightness | ((int)Math.Round(brightness * 16) - 1))).Wait();
                dev.WriteByte((byte)Commands.Blink | 0x01).Wait();
            }
            else
            {
                dev.WriteByte((byte)Commands.Blink | 0x00).Wait();
            }
        }
    }
}
