using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Utilities;

namespace Treehopper.Libraries.Sensors.Optical
{
    /// <summary>
    /// Renesas ISL29125 RGB light sensor
    /// </summary>
    /// The Renesas ISL29125 is a light sensor that outputs 12-bit or 16-bit red, green, and blue lux readings. The sensor has two ranges — 5.7 mlux to 375 lux, and 0.152 lux to 10,000 lux.
    /// You can access the values of each color component through the #Red, #Green, and #Blue properties. These have already been converted from raw ADC values into actual lux.
    /// 
    /// By default, the driver will operate in 16-bit mode in the 0. 152 to 10,000 lux range, sampling all three channels. The per-channel 16-bit sample time is more than 100 ms, so it can take almost a third of a second to get an RGB reading from the sensor in its default configuration. To increase the sampling rate, you can change the #SampleDepth property to decrease the sampling resolution to 12 bits; this will decrease the per-color sample time to less than 7 ms. You can also change the #Mode property to only sample a subset of color channels.
    /// 
    /// While the interrupt pin is optional, we recommend using it to avoid needlessly polling the(relatively slow-to-update) sensor. Pass in a Pin reference to the constructor, disable #AutoUpdateWhenPropertyRead, and attach a callback to  \link Isl29125.InterruptReceived InterruptReceived\endlink. By default, your callback will be invoked whenever a full conversion has completed; otherwise, you can set #LowThreshold, #HighThreshold, and #InterruptSelection to fine-tune when interrupts are triggered.

    [Supports("Renesas", "ISL29125")]
    public partial class Isl29125 : IPollable
    {
        private Isl29125Registers registers;

        /// <summary>
        /// An EventArgs that contains new data received by the sensor
        /// </summary>
        public class InterruptReceivedEventArgs
        {
            /// <summary>
            /// Gets the amount of red, expressed in lux, measured by the sensor.
            /// </summary>
            public double Red { get; set; }

            /// <summary>
            /// Gets the amount of green, expressed in lux, measured by the sensor.
            /// </summary>
            public double Green { get; set; }

            /// <summary>
            /// Gets the amount of blue, expressed in lux, measured by the sensor.
            /// </summary>
            public double Blue { get; set; }
        }

        /// <summary>
        /// An event handler delegate that can be used with the InterruptReceived event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void OnInterruptReceived(object sender, InterruptReceivedEventArgs e);

        /// <summary>
        /// Fires whenever new color data has been received in response to an interrupt
        /// </summary>
        public event OnInterruptReceived InterruptReceived;
        /// <summary>
        /// Construct a new ISL29125 object
        /// </summary>
        /// <param name="i2c">The I2C port to use</param>
        /// <param name="interrupt">The (optional) DigitalIn to use for the INT pin</param>
        /// <param name="rate">The I2C communication rate (defaults to 100 kHz)</param>
        public Isl29125(I2C i2c, DigitalIn interrupt = null, int rate = 100)
        {
            registers = new Isl29125Registers(new SMBusRegisterManagerAdapter(new SMBusDevice(0x44, i2c, rate)));

            // check device ID
            registers.deviceId.read();
            if(registers.deviceId.value != 0x7D)
            {
                Utility.Error("No ISL29125 found.", true);
            }

            // reset device
            registers.deviceReset.value = 0x46;
            registers.deviceReset.write();

            registers.config1.setMode(Modes.GreenRedBlue);
            registers.config1.setRange(Ranges.Lux_10000);
            registers.config1.write();

            // optional interrupt setup
            if(interrupt != null)
            {
                Task.Run(interrupt.MakeDigitalInAsync).Wait();
                interrupt.DigitalValueChanged += Interrupt_DigitalValueChanged;

                InterruptSelection = InterruptSelections.Green;

                // set the default thresholds to 0 so we get an interrupt on every ADC read by default
                LowThreshold = 0;
                HighThreshold = 0;

                // clear the (potentially pending) interrupt
                registers.status.read();
            }
        }

        private async void Interrupt_DigitalValueChanged(object sender, DigitalInValueChangedEventArgs e)
        {
            if(e.NewValue == false) // interrupts occur on the falling edge
            {
                await UpdateAsync().ConfigureAwait(false);
                InterruptReceived?.Invoke(this, new InterruptReceivedEventArgs() { Red = this.Red, Green = this.Green, Blue = this.Blue });
            }
                
        }

        /// <summary>
        /// Gets or sets the operating mode and channel selection for the sensor. Defaults to RGB.
        /// </summary>
        public Modes Mode
        {
            get { return registers.config1.getMode(); }
            set { registers.config1.setMode(value); registers.config1.write(); }
        }

        /// <summary>
        /// Gets or sets the sample depth (12-bit or 16-bit) to use for the sensor. Defaults to 16-bit.
        /// </summary>
        public SampleDepths SampleDepth
        {
            get { return registers.config1.getSampleDepth(); }
            set { registers.config1.setSampleDepth(value); registers.config1.write(); }
        }

        /// <summary>
        /// Gets or sets the lux range to use for the sensor. Defaults to 10,000 lux.
        /// </summary>
        public Ranges Range
        {
            get { return registers.config1.getRange(); }
            set { registers.config1.setRange(value); registers.config1.write(); }
        }

        /// <summary>
        /// Gets or sets the high threshold to use with the interrupt pin. Defaults to 0.
        /// </summary>
        public int HighThreshold
        {
            get { return registers.highThreshold.value; }
            set { 
                registers.highThreshold.value = value;
                registers.highThreshold.write();
            }
        }

        /// <summary>
        /// Gets or sets the low threshold to use with the interrupt pin. Defaults to 0.
        /// </summary>
        public int LowThreshold
        {
            get { return registers.lowThreshold.value; }
            set
            {
                registers.lowThreshold.value = value;
                registers.lowThreshold.write();
            }
        }

        /// <summary>
        /// Gets or sets the channel to use for the interrupt threshold comparrison. Defaults to green.
        /// </summary>
        public InterruptSelections InterruptSelection
        {
            get { return registers.config3.getInterruptSelection(); }
            set { 
                registers.config3.setInterruptSelection(value);
                registers.config3.write();
            }
        }

        private double ValueToLux(double value)
        {
            if (registers.config1.getRange() == Ranges.Lux_10000)
                value *= 10000;
            else
                value *= 375;

            if (registers.config1.getSampleDepth() == SampleDepths.Bits_12)
                value /= 4095;
            else
                value /= 65535;

            return value;
        }

        /// <summary>
        /// Gets the amount of red, expressed in lux, measured by the sensor.
        /// </summary>
        public double Red
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    registers.redData.read();

               return ValueToLux(registers.redData.value);
            }
        }

        /// <summary>
        /// Gets the amount of green, expressed in lux, measured by the sensor.
        /// </summary>
        public double Green
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    registers.greenData.read();

                return ValueToLux(registers.greenData.value);
            }
        }

        /// <summary>
        /// Gets the amount of blue, expressed in lux, measured by the sensor.
        /// </summary>
        public double Blue
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    registers.blueData.read();

                return ValueToLux(registers.blueData.value);
            }
        }

        /// <summary>
        /// Gets or sets whether the sensor should be read over the bus when the Red, Green, or Blue properties are accessed. Defaults to true.
        /// </summary>
        public bool AutoUpdateWhenPropertyRead { get; set; } = true;

        /// <summary>
        /// Fires whenever new color data has been received
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Fetches data from the sensor and updates the Red, Green, and Blue properties.
        /// </summary>
        /// <returns>An awaitable Task</returns>
        public async Task UpdateAsync()
        {
            await registers.readRange(registers.status, registers.blueData).ConfigureAwait(false);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Red"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Green"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Blue"));
        }
    }
}
