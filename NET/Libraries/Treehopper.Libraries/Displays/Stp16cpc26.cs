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
    /// <summary>
    /// A STP16CPC26 LED driver, which is a 16-bit constant-current shift register sink.
    /// </summary>
    public class Stp16cpc26 : ChainableShiftRegisterOutput, ILedDriver
    {
        DigitalOutPin oe;
        Pwm oePwm;

        /// <summary>
        /// Construct an STP16CPC26 attached directly to a board SPI module
        /// </summary>
        /// <param name="SpiModule">The board's SPI module</param>
        /// <param name="LatchPin">The pin to use for latches</param>
        /// <param name="OutputEnablePin">The output enable pin, if any, to use.</param>
        public Stp16cpc26(Spi SpiModule, SpiChipSelectPin LatchPin, DigitalOutPin OutputEnablePin = null) : base(SpiModule, LatchPin, 2)
        {
            oe = OutputEnablePin;
            Start();
        }

        /// <summary>
        /// Construct an STP16CPC26 attached directly to a board SPI module
        /// </summary>
        /// <param name="SpiModule">The board's SPI module</param>
        /// <param name="LatchPin">The pin to use for latches</param>
        /// <param name="OutputEnablePin">The PWM pin to use, allowing controllable global brightness.</param>
        public Stp16cpc26(Spi SpiModule, SpiChipSelectPin LatchPin, Pwm OutputEnablePin) : base(SpiModule, LatchPin, 2)
        {
            this.oePwm = OutputEnablePin;
            Start();
        }

        /// <summary>
        /// Construct an STP16CPC26 attached to the output of another shift register
        /// </summary>
        /// <param name="upstreamDevice">The upstream device this shift register is attached to</param>
        /// <param name="OutputEnablePin">The digital pin to use, if any, to control the display state</param>
        public Stp16cpc26(ChainableShiftRegisterOutput upstreamDevice, DigitalOutPin OutputEnablePin = null) : base(upstreamDevice, 2)
        {
            oe = OutputEnablePin;
            Start();
        }

        /// <summary>
        /// Construct an STP16CPC26 attached to the output of another shift register
        /// </summary>
        /// <param name="upstreamDevice">The upstream device this shift register is attached to</param>
        /// <param name="OutputEnablePin">The PWM pin to use, if any, to control the display brightness</param>
        public Stp16cpc26(ChainableShiftRegisterOutput upstreamDevice, Pwm OutputEnablePin) : base(upstreamDevice, 2)
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
            HasIndividualBrightnessControl = false;
            Brightness = 1.0; // turn on the display
        }

        double brightness;

        /// <summary>
        /// Gets or sets the brightness of this driver
        /// </summary>
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

        /// <summary>
        /// Gets whether this driver has global brightness
        /// </summary>
        /// <remarks>
        /// <para>
        /// The state of this value depends if the board was constructed with a <see cref="Pwm"/> pin or not. 
        /// </para>
        /// </remarks>
        public bool HasGlobalBrightnessControl { get; private set; }

        /// <summary>
        /// Gets whether this driver has individual brightness control. This parameter will always return false.
        /// </summary>
        public bool HasIndividualBrightnessControl { get; private set; }

        /// <summary>
        /// Gets the LEDs belonging to this controller
        /// </summary>
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

        /// <summary>
        /// Clear the display
        /// </summary>
        /// <returns>An awaitable task that completes when finished</returns>
        public Task Clear()
        {
            return Write(0);
        }

        /// <summary>
        /// Update the LED states from the current value
        /// </summary>
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
