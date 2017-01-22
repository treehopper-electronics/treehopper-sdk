namespace Treehopper.Libraries.Displays
{
    using System;
    using System.Collections;
    using System.Threading.Tasks;

    public class Ht16k33 : LedDriver
    {
        private SMBusDevice dev;
        private BitArray data = new BitArray(128);
        private Package package;

        public Ht16k33(I2c i2c, byte address, Package package) : base((int)package, true, false)
        {
            dev = new SMBusDevice(address, i2c);
            dev.WriteByte(0x21);
            Brightness = 1.0;
            this.package = package;
        }

        public Ht16k33(I2c i2c, Package package, bool a0 = false, bool a1 = false, bool a2 = false) : this(i2c, (byte)(0x70 | (a0 ? 1 : 0) | (a1 ? 1 : 0) << 1 | (a2 ? 1 : 0) << 2), package)
        {
        }

        public enum Package
        {
            Sop20_64Led = 64,
            Sop24_96Led = 96,
            Sop28_128Led = 128
        }

        private enum Commands
        {
            Brightness = 0xe0,
            Blink = 0x80
        }

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
