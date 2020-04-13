using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Treehopper.Libraries.IO.Mux
{
    /// <summary>
    ///     Analog mux for ADC expansion
    /// </summary>
    [Supports("Generic", "4052")]
    public class AdcAnalogMux
    {
        private readonly DigitalOut[] pins;
        internal int settlingTime;

        /// <summary>
        ///     Construct an <see cref="AdcAnalogMux" /> using a standard 4052-style two-bit 4:1 mux.
        /// </summary>
        /// <param name="muxedPin">The upstream Adc pin to mux</param>
        /// <param name="settlingTime">The settling time, in ms, to use</param>
        /// <param name="pins">The pin(s) to use to control the mux, starting with the least-significant bit</param>
        public AdcAnalogMux(AdcPin muxedPin, int settlingTime, params DigitalOut[] pins)
        {
            this.pins = pins;
            this.pin = muxedPin;
            this.settlingTime = settlingTime;
            muxedPin.MakeAnalogInAsync();
            foreach (var pin in pins)
                pin.MakeDigitalPushPullOutAsync();
        }

        internal AdcPin pin { get; set; }

        /// <summary>
        /// The analog pins provided by the mux
        /// </summary>
        public Collection<AdcPin> Pins { get; set; }

        /// <summary>
        ///     Set the mux
        /// </summary>
        /// <param name="index">The index to be muxed</param>
        internal void setMux(int index)
        {
            var array = new BitArray(new[] {(byte) index});
            for (var i = 0; i < pins.Length; i++)
                pins[i].DigitalValue = array[i];
        }

        public class AdcAnalogMuxPin : AdcPin
        {
            private readonly int channel;
            private readonly AdcAnalogMux parent;

            internal AdcAnalogMuxPin(AdcAnalogMux parent, int channel)
            {
                this.parent = parent;
                Task.Delay(parent.settlingTime).Wait();
                this.channel = channel;
            }

            /// <summary>
            ///     Get the raw ADC value
            /// </summary>
            public int AdcValue
            {
                get
                {
                    parent.setMux(channel);
                    Task.Delay(parent.settlingTime).Wait();
                    return parent.pin.AdcValue;
                }
            }

            /// <summary>
            ///     Gets or sets the threshold for the AdcValueChanged event
            /// </summary>
            public int AdcValueChangedThreshold
            {
                get { throw new NotImplementedException(); }

                set { throw new NotImplementedException(); }
            }

            /// <summary>
            ///     The analog value (0-1)
            /// </summary>
            public double AnalogValue
            {
                get
                {
                    parent.setMux(channel);
                    Task.Delay(parent.settlingTime).Wait();
                    return parent.pin.AnalogValue;
                }
            }

            /// <summary>
            ///     Gets or sets the threshold for the AnalogValueChanged event
            /// </summary>
            public double AnalogValueChangedThreshold
            {
                get { throw new NotImplementedException(); }

                set { throw new NotImplementedException(); }
            }

            /// <summary>
            ///     The analog voltage
            /// </summary>
            public double AnalogVoltage
            {
                get
                {
                    parent.setMux(channel);
                    Task.Delay(parent.settlingTime).Wait();
                    return parent.pin.AnalogVoltage;
                }
            }

            /// <summary>
            ///     Gets or sets the threshold for the AnalogVoltageChanged event
            /// </summary>
            public double AnalogVoltageChangedThreshold
            {
                get { throw new NotImplementedException(); }

                set { throw new NotImplementedException(); }
            }

            /// <summary>
            ///     This event is not supported.
            /// </summary>
            public event OnAdcValueChanged AdcValueChanged;

            /// <summary>
            ///     This event is not supported.
            /// </summary>
            public event OnAnalogValueChanged AnalogValueChanged;

            /// <summary>
            ///     This event is not supported.
            /// </summary>
            public event OnAnalogVoltageChanged AnalogVoltageChanged;

            /// <summary>
            /// This event is not supported.
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            ///     Make the pin an input (unused)
            /// </summary>
            public Task MakeAnalogInAsync()
            {
                return Task.FromResult<object>(null);
            }
        }
    }
}