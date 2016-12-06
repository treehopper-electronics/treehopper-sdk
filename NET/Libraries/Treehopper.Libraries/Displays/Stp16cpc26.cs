using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries.Interface.ShiftRegister;

namespace Treehopper.Libraries.Displays
{
    public class Stp16cpc26 : ChainableShiftRegisterOutput, ILedDriver
    {
        DigitalOutPin oe;
        IPwm oePwm;

        public Stp16cpc26(Spi SpiModule, Pin LatchPin, DigitalOutPin OutputEnablePin = null) : base(SpiModule, LatchPin, 2)
        {
            oe = OutputEnablePin;
            Start();
        }

        public Stp16cpc26(Spi SpiModule, Pin LatchPin, IPwm OutputEnablePin) : base(SpiModule, LatchPin, 2)
        {
            this.oePwm = OutputEnablePin;
            Start();
        }

        public Stp16cpc26(ChainableShiftRegisterOutput upstreamDevice, DigitalOutPin OutputEnablePin = null) : base(upstreamDevice, 2)
        {
            oe = OutputEnablePin;
            Start();
        }

        public Stp16cpc26(ChainableShiftRegisterOutput upstreamDevice, IPwm OutputEnablePin) : base(upstreamDevice, 2)
        {
            this.oePwm = OutputEnablePin;
            Start();
        }


        private void Start()
        {
            for (int i = 0; i < 16; i++)
                Leds.Add(new Led(this, i));

            if (oe != null)
            {
                oe.MakeDigitalPushPullOut();
                oe.DigitalValue = false;
                HasGlobalBrightnessControl = false;
            } else if(oePwm != null)
            {
                oePwm.PulseWidth = 0;
                oePwm.Enabled = true;
                HasGlobalBrightnessControl = true;
              } else
            {
                HasGlobalBrightnessControl = false;
            }

            Brightness = 1.0; // turn on the display
        }

        double brightness;

        public double Brightness
        {
            get
            {
                return brightness;
            }

            set
            {
                if (brightness == value) return;
                brightness = value;

                if (oePwm != null)
                    oePwm.DutyCycle = 1 - brightness;
                else if (oe != null)
                    oe.DigitalValue = brightness > 0.5 ? false : true;

            }
        }

        public bool HasGlobalBrightnessControl { get; private set; }

        public bool HasIndividualBrightnessControl { get; private set; }

        public IList<Led> Leds { get; private set; } = new Collection<Led>();

        ushort currentValue = 0x0000;

        void ILedDriver.LedStateChanged(Led led)
        {
            if (led.State)
                CurrentValue |= (uint)(1 << led.Channel);
            else
                CurrentValue &= (uint)~(1 << led.Channel);

            FlushIfAutoFlushEnabled().Wait();
        }

        void ILedDriver.LedBrightnessChanged(Led led)
        {
            
        }

        public Task Clear()
        {
            return Write(0);
        }

        protected override void updateFromCurrentValue()
        {
            uint currentValue = CurrentValue; // CurrentValue is an expensive read, so only read it once
            for (int i = 0; i < Leds.Count; i++)
            {
                Leds[i].State = ((currentValue >> i) & 1) == 0x01 ? true : false;
            }
        }
    }
}
