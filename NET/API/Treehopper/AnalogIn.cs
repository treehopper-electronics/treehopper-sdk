using System;

namespace Treehopper
{
    /// <summary>
    /// Used to send VoltageChanged events from the AnalogIn pin.
    /// </summary>
    /// <param name="sender">The AnalogIn pin that sent that message</param>
    /// <param name="voltage">The new voltage of the AnalogIn pin</param>
    public delegate void OnAnalogInVoltageChanged(AnalogIn sender, double voltage);

    /// <summary>
    /// Used to send ValueChanged events from the AnalogIn pin.
    /// </summary>
    /// <param name="sender">The AnalogIn pin that sent that message</param>
    /// <param name="value">The new voltage of the AnalogIn pin</param>
    public delegate void OnAnalogInValueChanged(AnalogIn sender, int value);

    /// <summary>
    /// The AnalogIn class is responsible for analog-to-digital conversion functionality, which allows the user to sample a voltage present on a pin, between 0 and 5V. 
    /// </summary>
    /// <remarks>
    /// The pin voltage value is retrievable either using the Voltage property, or the Value property.
    /// </remarks>
    public class AnalogIn
    {
        /// <summary>
        /// Occurs when an analog voltage is changed.
        /// </summary>
        /// <remarks>
        /// The Changed event is raised when the 10-bit ADC value obtained is different from the previous reading.
        /// </remarks>
        public event OnAnalogInVoltageChanged VoltageChanged;

        /// <summary>
        /// Occurs when an analog value is changed.
        /// </summary>
        /// <remarks>
        /// The Changed event is raised when the 10-bit ADC value obtained is different from the previous reading.
        /// </remarks>
        public event OnAnalogInValueChanged ValueChanged;
        double voltage;
        int value;
        Pin pin;
        bool isEnabled;

        internal AnalogIn(Pin Pin)
        {
            pin = Pin;
        }

        /// <summary>
        /// Get or set the state of the ADC peripheral. 
        /// </summary>
        /// <remarks>
        /// Setting the ADC peripheral to "disabled" will make it a digital input.
        /// </remarks>
        public bool IsEnabled
        {
            get 
            {
                return isEnabled;
            }
            set
            {
                isEnabled = value;
                if(isEnabled)
                {
                    if (pin.State == PinState.AnalogInput)
                        return;
                    pin.SendCommand(new byte[] { (byte)PinConfigCommands.MakeAnalogInput });
                    pin.State = PinState.AnalogInput;
                } else
                {
                    pin.MakeDigitalInput();
                }
            }

        }

        /// <summary>
        /// Retrieve the last value obtained from the ADC. 
        /// </summary>
        /// <remarks>
        /// Treehopper has a 10-bit ADC, so ADC values will range from 0-1023.
        /// </remarks>
        public int Value
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// Retrieve the last voltage reading from the ADC.
        /// </summary>
        /// <remarks>
        /// This property assumes VBUS, the supply voltage, is equivalent to 5V. 
        /// Since the actual VBUS will be higher or lower, the value returned from this property shouldn't be assumed to be exact. 
        /// Since many sensors are ratiometric, and since most users will power these sensors from VBUS, it is safe to assume 
        /// that 0V represents 0%, 2.5V represents 50% and 5V represents 100% readout.
        /// </remarks>
        public double Voltage
        {
            get
            {
                return this.voltage;
            }
        }

        /// <summary>
        /// Internal method used to update the ADC with the latest value. This should only be called by the Treehopper updater
        /// </summary>
        /// <param name="highByte"></param>
        /// <param name="lowByte"></param>
        internal void UpdateAnalogValue(byte highByte, byte lowByte)
        {
            int val = ((int)highByte) << 8;
            val += (int)lowByte;
            double analogVal = Math.Round((float)val / 204.8f, 3);

            if(value != val) // compare the actual ADC values, not the floating-point conversions.
            {
                value = val;
                this.voltage = analogVal;
                if(ValueChanged != null)
                {
                    ValueChanged(this, this.value);
                }
                
                if(VoltageChanged != null)
                {
                    VoltageChanged(this, this.voltage);
                }

                pin.RaisePropertyChanged("AnalogIn");
                // RaisePropertyChanged("AnalogValue");
            }
        }
    }
}
