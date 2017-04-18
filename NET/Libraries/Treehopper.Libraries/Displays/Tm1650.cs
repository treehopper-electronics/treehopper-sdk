using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    /// Titan Micro TM1650 4x8 LED driver
    /// </summary>
    /// <remarks>
    /// <para>Instead of using standard SMBus-style register/value pairs, this IC's individual registers are all accessed directly from different I2C addresses. Consequently, be aware that this IC uses quite a few I2C addresses — 0x24, 0x25, 0x26, 0x27 (for control), and 0x34, 0x35, 0x36, and 0x37 (for display).</para>
    /// </remarks>
    public class Tm1650 : LedDriver
    {
        private readonly I2c i2c;

        private static readonly byte ControlBase = 0x24;
        private static readonly byte DisplayBase = 0x34;

        private readonly byte[] oldValues = new byte[4];
        private readonly byte[] newValues = new byte[4];
        private bool enable;

        /// <summary>
        /// Construct a new TM1650 with a given I2c interface
        /// </summary>
        /// <param name="i2c">The I2c interface to use</param>
        public Tm1650(I2c i2c) : base(32, true, false)
        {
            this.i2c = i2c;
            brightness = 1.0;
            i2c.Enabled = true;
            Enable = true;
        }

        /// <summary>
        /// Enable or disable the display
        /// </summary>
        public bool Enable
        {
            get { return enable; }
            set
            {
                if (enable == value) return;
                enable = value;
                sendControlUpdate().Wait();
            }
        }

        private async Task sendControlUpdate()
        {
            int bright = (int)(Brightness < 1.0 ? Math.Ceiling(Brightness * 7.0) : 0);
            byte controlByte = (byte)((bright << 4) | ((enable && Brightness > 0) ? 0x01 : 0x00));
            for (int i = 0; i < 4; i++)
                await sendControl(controlByte, i);
        }

        private Task sendControl(byte data, int digit)
        {
            return i2c.SendReceive((byte)(ControlBase + digit), new byte[] { data }, 0);
        }

        private Task sendDisplay(byte data, int digit)
        {
            return i2c.SendReceive((byte)(DisplayBase + digit), new byte[] { data }, 0);
        }

        /// <summary>
        /// Flush data to the driver
        /// </summary>
        /// <param name="force">Whether or not to force all data to be flushed, even if it doesn't appear to have changed</param>
        /// <returns>An awaitable task that completes when finished</returns>
        public override async Task Flush(bool force = false)
        {
            for(int i=0;i<4;i++)
            {
                if (oldValues[i] != newValues[i] || force)
                {
                    await sendDisplay(newValues[i], i);
                    oldValues[i] = newValues[i];
                }
            }
        }

        internal override void LedBrightnessChanged(Led led)
        {
            
        }

        internal override void LedStateChanged(Led led)
        {
            int digit = led.Channel / 8;
            int segment = led.Channel % 8;
            bool value = led.State;

            // set or clear the appropriate bit
            if(led.State)
                newValues[digit] |= (byte)(1 << segment);
            else
                newValues[digit] &= (byte)~(1 << segment);

            if (AutoFlush)
                Flush().Wait();
        }

        internal override void SetGlobalBrightness(double brightness)
        {
            sendControlUpdate().Wait();
        }
    }
}
