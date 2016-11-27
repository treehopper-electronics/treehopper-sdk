using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;

namespace Treehopper.Libraries.Displays
{
    public class Stp16cpc26 : LedDriver
    {
        Spi spi;
        IDigitalOutPin le;
        IDigitalOutPin oe;
        IPwm oePwm;
        public Stp16cpc26(Spi SpiModule, IDigitalOutPin LatchPin, IDigitalOutPin OutputEnablePin = null) : base(16, true, false)
        {
            this.spi = SpiModule;
            this.le = LatchPin;
            if (OutputEnablePin != null)
            {
                this.oe = OutputEnablePin;
                oe.MakeDigitalPushPullOut();
                oe.DigitalValue = false;
            }

            HasGlobalBrightnessControl = false;
            Start();
        }

        public Stp16cpc26(Spi SpiModule, IDigitalOutPin LatchPin, IPwm OutputEnablePin) : base(16, true, false)
        {
            this.spi = SpiModule;
            this.le = LatchPin;
            this.oePwm = OutputEnablePin;
            oePwm.PulseWidth = 0;
            oePwm.Enabled = true;
            HasGlobalBrightnessControl = true;
            Brightness = 1.0;
            Start();
        }

        private void Start()
        {
            spi.Enabled = true;
            le.MakeDigitalPushPullOut();
            le.DigitalValue = false;
        }

        public override double Brightness
        {
            get
            {
                return base.Brightness;
            }

            set
            {
                if (base.Brightness == value) return;
                base.Brightness = value;
                oePwm.DutyCycle = 1 - base.Brightness;
            }
        }

        private async Task WriteValue()
        {
            byte[] data = new byte[] { (byte)(currentValue >> 8), (byte)(currentValue & 0xff) };
            await spi.SendReceive(data);
            le.DigitalValue = true;
            le.DigitalValue = false;
            await Task.Delay(15);
        }

        ushort currentValue = 0x0000;
        internal override void LedStateChanged(Led led)
        {
            if (led.State)
                currentValue |= (ushort)(1 << led.Channel);
            else
                currentValue &= (ushort)(~(1 << led.Channel));
            if (AutoFlush) WriteValue().Wait();
        }

        internal override void LedBrightnessChanged(Led led)
        {
            
        }

        public override async Task Flush(bool force = false)
        {
            if (AutoFlush && !force) return;
            await WriteValue();
        }
    }
}
