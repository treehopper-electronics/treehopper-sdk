namespace Treehopper.Libraries.Displays
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Treehopper;
    using Interface.PortExpander;
    using Treehopper.Utilities;
    using Utilities;
    /// <summary>
    /// Supports generic 8-bit or 16-bit LED shift register drivers, such as the STP16CPC26, CAT4016, TLC5916, etc
    /// </summary>
    /// <remarks>
    /// <para>This driver provides support for many common 8-channel and 16-channel LED drivers that have an active-low Output Enable (OE) pin that supports global brightness control through PWMing. The following ICs are examples of drivers that are compatible with this library:
    /// <list type="bullet">
    /// <item><term>Texas Instruments TLC591x</term><description>8-channel, up to 120 mA, 20V</description></item>
    /// <item><term>Texas Instruments TLC5928</term><description>16-channel, up to 35 mA, 17V</description></item>
    /// <item><term>Texas Instruments TLC5928x</term><description>16-channel, up to 45 mA, 10V</description></item>
    /// <item><term>Silicon Touch (SiTI) ST2221A</term><description>8-channel, up to 90 mA, 9V</description></item>
    /// <item><term>Silicon Touch (SiTI) ST2221C</term><description>16-channel, up to 90 mA, 9V</description></item>
    /// <item><term>Macroblock MBI502x</term><description>16-channel, up to 90 mA, 17V</description></item>
    /// <item><term>Allegro A6282</term><description>16-channel, up to 50 mA, 12V</description></item>
    /// <item><term>ON Semiconductor CAT4016</term><description>16-channel, up to 100 mA, 5.5V</description></item>
    /// <item><term>ST STP16C596</term><description>16-channel, up to 120 mA, 16V</description></item>
    /// <item><term>ST STP16CP05</term><description>16-channel, up to 100 mA, 20V</description></item>
    /// <item><term>ST STP16CPC26</term><description>16-channel, up to 90 mA, 20V</description></item>
    /// <item><term>ISSI IS31FL3726</term><description>16-channel, up to 60 mA, 4V</description></item>
    /// <item><term>AMS AS1123</term><description>16-channel, up to 40 mA, 5.5V</description></item>
    /// </list>
    /// Note that any other 8-bit or 16-bit shift register can also be used with this library if you want an <see cref="Led"/>-based interface to the shift register. For example, the low-cost (and ubiquitous) 74HC595 is perfectly capable of driving small indicator LEDs, and the TPIC6B595 could be used with this library to drive high-voltage, high-power LEDs (or nixie tubes, etc).
    /// </para>
    /// <para>This library supports three different ways of working with the OE (output enable) pin:
    /// <list type="number" >
    /// <item><term>Unmanaged</term><description>The library doesn't do anything with the OE pin. You can tie it to GND in your circuit, or control it outside the context of this library by yourself.</description></item>
    /// <item><term>GPIO pin</term><description>If you pass a GPIO pin to the constructor, the library will be able to globally enable/disable the display through the Brightness property. Setting a brightness of 0.5 or greater will turn on the display, otherwise, the display will be off.</description></item>
    /// <item><term>PWM pin</term><description>If you pass a PWM pin to the constructor, the library will be able to control the global brightness of the display. Note that CIE perceptual brightness conversion will be performed, so, i.e., setting Brightness to 0.5 will produce a result that appears half as bright as a Brightness of 1.0.</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public class LedShiftRegister : ChainableShiftRegisterOutput, ILedDriver
    {
        readonly DigitalOut oe;
        readonly Pwm oePwm;
        int channels;

        /// <summary>
        /// Construct an LED shift register attached directly to a board SPI module
        /// </summary>
        /// <param name="SpiModule">The board's SPI module</param>
        /// <param name="LatchPin">The pin to use for latches</param>
        /// <param name="ChannelCount">Whether the driver is 16 or 8-channel</param>
        /// <param name="OutputEnablePin">The output enable pin, if any, to use.</param>
        /// <param name="speedMhz">The speed, in MHz, to use when communicating</param>
        public LedShiftRegister(Spi SpiModule, SpiChipSelectPin LatchPin, LedChannelCount ChannelCount = LedChannelCount.SixteenChannel, DigitalOut OutputEnablePin = null, double speedMhz = 6) : base(SpiModule, LatchPin, (int)ChannelCount/8, speedMhz)
        {
            oe = OutputEnablePin;
            Start(ChannelCount);
        }

        /// <summary>
        /// Construct an LED shift register attached directly to a board SPI module
        /// </summary>
        /// <param name="SpiModule">The board's SPI module</param>
        /// <param name="LatchPin">The pin to use for latches</param>
        /// <param name="OutputEnablePin">The PWM pin to use, allowing controllable global brightness.</param>
        /// <param name="ChannelCount">The number of channels this LED shift register has</param>
        /// <param name="speedMhz">The speed, in MHz, to use when communicating</param>
        public LedShiftRegister(Spi SpiModule, SpiChipSelectPin LatchPin, Pwm OutputEnablePin, LedChannelCount ChannelCount = LedChannelCount.SixteenChannel, double speedMhz = 6) : base(SpiModule, LatchPin, (int)ChannelCount / 8, speedMhz)
        {
            oePwm = OutputEnablePin;
            Start(ChannelCount);
        }

        /// <summary>
        /// Construct an LED shift register attached to the output of another shift register
        /// </summary>
        /// <param name="upstreamDevice">The upstream device this shift register is attached to</param>
        /// <param name="OutputEnablePin">The digital pin to use, if any, to control the display state</param>
        /// <param name="ChannelCount">The number of channels this LED shift register has</param>
        public LedShiftRegister(ChainableShiftRegisterOutput upstreamDevice, LedChannelCount ChannelCount = LedChannelCount.SixteenChannel, DigitalOut OutputEnablePin = null) : base(upstreamDevice, (int)ChannelCount / 8)
        {
            oe = OutputEnablePin;
            Start(ChannelCount);
        }

        /// <summary>
        /// Construct an LED shift register attached to the output of another shift register
        /// </summary>
        /// <param name="upstreamDevice">The upstream device this shift register is attached to</param>
        /// <param name="OutputEnablePin">The PWM pin to use, if any, to control the display brightness</param>
        /// <param name="ChannelCount">The number of channels this LED shift register has</param>
        public LedShiftRegister(ChainableShiftRegisterOutput upstreamDevice, Pwm OutputEnablePin, LedChannelCount ChannelCount = LedChannelCount.SixteenChannel) : base(upstreamDevice, (int)ChannelCount / 8)
        {
            oePwm = OutputEnablePin;
            Start(ChannelCount);
        }


        private void Start(LedChannelCount channelCount)
        {
            channels = (int)channelCount;
            currentValue = new BitArray(channels);
            for (int i = 0; i < channels; i++)
                Leds.Add(new Led(this, i));

            if (oe != null)
            {
                oe.MakeDigitalPushPullOut();
                oe.DigitalValue = false;
                HasGlobalBrightnessControl = false;
            } else if(oePwm != null)
            {
                oePwm.Enabled = true;
                oePwm.PulseWidth = 0;
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
                if (value.CloseTo(brightness)) return;
                brightness = value;

                if (oePwm != null)
                    oePwm.DutyCycle = 1 - Utility.BrightnessToCieLuminance(brightness);
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

        // Use a BitArray to hold onto our current values
        BitArray currentValue;
        void ILedDriver.LedStateChanged(Led led)
        {
            currentValue.Set(led.Channel, led.State);
            CurrentValue = currentValue.GetBytes();
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
            return Write(new byte[channels / 8]);
        }

        /// <summary>
        /// Update the LED states from the current value
        /// </summary>
        protected override void updateFromCurrentValue()
        {
            for (int i = 0; i < Leds.Count; i++)
            {
                Leds[i].State = currentValue.Get(i);
            }
        }

        /// <summary>
        /// The number of channels of this shift register
        /// </summary>
        public enum LedChannelCount
        {
            /// <summary>
            /// A 16-channel LED shift register
            /// </summary>
            SixteenChannel = 16,

            /// <summary>
            /// An 8-channel LED shift register
            /// </summary>
            EightChannel = 8
        }
    }
}
