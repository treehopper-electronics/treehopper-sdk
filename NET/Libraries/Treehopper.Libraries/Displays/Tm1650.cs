using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    public class Tm1650 : LedDriver
    {
        private II2c i2c;

        private static readonly byte ControlBase = 0x24;
        private static readonly byte DisplayBase = 0x34;

        private byte[] oldValues = new byte[4];
        private byte[] newValues = new byte[4];
        private bool enable;

        public Tm1650(II2c i2c) : base(32, false, false)
        {
            this.i2c = i2c;
            i2c.Enabled = true;
            Enable = true;
        }

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
    }
}
