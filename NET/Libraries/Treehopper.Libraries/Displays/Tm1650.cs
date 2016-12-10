using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    /// Represents a TM1650 LED driver designed for driving up to 4 seven-segment digits, or 32 individual common-anode LEDs
    /// </summary>
    public class Tm1650 : LedDriver
    {
        private I2c i2c;

        private static readonly byte ControlBase = 0x24;
        private static readonly byte DisplayBase = 0x34;

        private byte[] oldValues = new byte[4];
        private byte[] newValues = new byte[4];
        private bool enable;

        /// <summary>
        /// Construct a new TM1650 with a given I2c interface
        /// </summary>
        /// <param name="i2c">The I2c interface to use</param>
        public Tm1650(I2c i2c) : base(32, false, false)
        {
            this.i2c = i2c;
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
            byte controlByte = (byte)(enable ? 0x01 : 0x00);
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

        internal override void setBrightness(double brightness)
        {
            
        }
    }
}
