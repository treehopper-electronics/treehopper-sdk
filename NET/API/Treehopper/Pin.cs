using System;
using System.ComponentModel;

namespace Treehopper
{
    /// <summary>
    /// Defines whether a signal is active high (rising-edge) or active low (falling-edge)
    /// </summary>
    public enum PinPolarity
    {
        /// <summary>
        /// The signal is considered active when the signal is high
        /// </summary>
        ActiveHigh,

        /// <summary>
        /// The signal is considered active when the signal is low
        /// </summary>
        ActiveLow
    };

    /// <summary>
    /// This is the delegate prototype used for event-driven reading of digital pins.
    /// </summary>
    /// <param name="sender">The Pin that changed</param>
    /// <param name="value">The new value of the pin</param>
    public delegate void OnDigitalInValueChanged(Pin sender, bool value);

    internal enum PinConfigCommands
    {
        MakeDigitalInput = 0x00,
        MakeDigitalOutput,
        MakeAnalogInput,
        MakeAnalogOutput,
        MakePWMPin,
        SetDigitalValue,
        GetDigitalValue,
        GetAnalogValue,
        SetAnalogValue,
        SetPWMValue
    }

    internal enum PinState { ReservedPin, DigitalInput, DigitalOutput, AnalogInput, AnalogOutput, PWM };
    
    /// <summary>
    /// The interrupt mode of the pin.
    /// </summary>
    public enum PinInterruptMode {
        /// <summary>
        /// Interrupts disabled.
        /// </summary>
        NoInterrupt, 
        
        /// <summary>
        /// Interrupt occurs when pin transitions from low to high.
        /// </summary>
        Rising, 
        
        /// <summary>
        /// Interrupt occurs when pin transitions from high to low
        /// </summary>
        Falling, 
        
        /// <summary>
        /// Interrupt occurs when pin transitions from either low to high, or from high to low.
        /// </summary>
        RisingFalling 
    };

    /// <summary>
    /// Used to send VoltageChanged events from the AnalogIn pin.
    /// </summary>
    /// <param name="sender">The AnalogIn pin that sent that message</param>
    /// <param name="voltage">The new voltage of the AnalogIn pin</param>
    public delegate void OnAnalogInVoltageChanged(Pin sender, double voltage);

    /// <summary>
    /// Used to send ValueChanged events from the AnalogIn pin.
    /// </summary>
    /// <param name="sender">The AnalogIn pin that sent that message</param>
    /// <param name="value">The new voltage of the AnalogIn pin</param>
    public delegate void OnAnalogInValueChanged(Pin sender, int value);

    /// <summary>
    /// Pin is the base class for all Treehopper pins. It provides core digital I/O (GPIO) functionality.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class is useful when only core GPIO functions are needed, and the user wishes to make code portable
    /// between pins. While it cannot be instantiated directly by the user, Pin variables can reference existing pins.
    /// </para>
    /// </remarks>
    public class Pin : INotifyPropertyChanged
    {
        protected string ioName;
        /// <summary>
        /// The PIC16F1459 pin name
        /// </summary>
        public string IOName { get { return ioName; } }

        private TreehopperUSB board;
        /// <summary>
        /// This returns a reference to the Treehopper board this pin belongs to.
        /// </summary>
        public TreehopperUSB Board { get { return board; } }

        PinInterruptMode interruptMode;
        bool digitalValue;
        internal PinState State {get; set;}

        /// <summary>
        /// Occurs when the input on the pin changes.
        /// </summary>
        /// <remarks>
        /// This event will only fire when the pin is configured as a digital input.
        /// </remarks>
        public event OnDigitalInValueChanged DigitalValueChanged;

        /// <summary>
        /// The pin number, from 1-14, of the pin.
        /// </summary>
        public int PinNumber { get; set; }

        /// <summary>
        /// The SoftPwm functions associated with this pin.
        /// </summary>
        public SoftPwm SoftPwm { get; set;}
        
        internal Pin(TreehopperUSB board, byte pinNumber)
        {
            this.board = board;
            this.PinNumber = pinNumber;
            interruptMode = PinInterruptMode.NoInterrupt;
            SoftPwm = new SoftPwm(Board, this);
        }

        /// <summary>
        /// The interrupt mode of the pin.
        /// </summary>
        /// <remarks>
        /// <para>Since the board always polls Treehopper (USB forbids devices from initiating communication), enabling an interrupt does not provide
        /// performance or timing improvements. However, it is useful when you wish to detect extremely short pulses that may occur faster than the
        /// pin refresh rate (which is system-dependent, but usually on the order of 1-5 kHz).</para>
        /// <para>
        /// Not all pins support interrupts. If a pin does not support an interrupt, it will override this property as read-only, and return <see cref="PinInterruptMode.NoInterrupt"/>.
        /// </para>
        /// </remarks>
        public virtual PinInterruptMode InterruptMode
        {
            get
            {
                return interruptMode;
            }
            set
            {
                if(value != interruptMode) // We have to re-issue the MakeDigitalInput command
                {
                    interruptMode = value;
                    RaisePropertyChanged("InterruptMode");
                    if(State == PinState.DigitalInput) // People are stupid and might change the interrupt mode of the pin while it's not an input!
                    {
                        SendCommand(new byte[] { (byte)PinConfigCommands.MakeDigitalInput, (byte)interruptMode });
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the digital value of the pin.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Setting a value to a pin that is configured as an input will automatically make the pin an output before 
        /// writing the value to it. 
        /// </para>
        /// <para>
        /// The value retrieved from this pin will read as "0" when then pin is being used for other purposes.
        /// </para>
        /// </remarks>
        public bool DigitalValue
        {
            get
            {
                return digitalValue;
            }
            set
            {
                digitalValue = value;
                if (State != PinState.DigitalOutput)
                    MakeDigitalOutput();
                byte byteVal = (byte)(digitalValue ? 0x01 : 0x00);
                SendCommand(new byte[] { (byte)PinConfigCommands.SetDigitalValue, byteVal});
            }
        }

        /// <summary>
        /// Toggles the output value of the pin.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Calling this function on a pin that is configured as an input will automatically make the pin an output
        /// before writing the value to it.
        /// </para>
        /// </remarks>
        /// <example>
        /// In this example, an LED attached to pin 4 is made to blink.
        /// <code>
        /// Pin led = myTreehopperBoard.Pin4; // create a reference to Pin4 to keep code concise.
        /// led.MakeDigitalOutput();
        /// while(true)
        /// {
        ///     led.Toggle();
        ///     Thread.Sleep(500);
        /// }
        /// </code>
        /// </example>
        public void ToggleOutput()
        {
            if (State != PinState.DigitalOutput)
                MakeDigitalOutput();
            DigitalValue = !DigitalValue;
        }

        internal void RaiseDigitalInValueChanged()
        {
            if (DigitalValueChanged != null)
            {
                DigitalValueChanged(this, digitalValue);
            }
        }

        /// <summary>
        /// Configures the pin as a digital input.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this function is called on a pin used for another purpose (such as AnalogIn, 
        /// AnalogOut, or communications), this alternative functionality will be disabled before
        /// the pin is set as an input.
        /// </para></remarks>
        public virtual void MakeDigitalInput()
        {
            if (State == PinState.DigitalInput)
                return;
            SendCommand(new byte[] { (byte)PinConfigCommands.MakeDigitalInput, (byte)interruptMode });
            State = PinState.DigitalInput;
        }

        /// <summary>
        /// Configures the pin as a digital output.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this function is called on a pin used for another purpose (such as AnalogIn, 
        /// AnalogOut, or communications), this alternative functionality will be disabled before
        /// the pin is set as an output.
        /// </para></remarks>
        public virtual void MakeDigitalOutput()
        {
            if (State == PinState.DigitalOutput)
                return;
            SendCommand(new byte[] { (byte)PinConfigCommands.MakeDigitalOutput });
            State = PinState.DigitalOutput;
        }

        internal void SendCommand(byte[] cmd)
        {
            byte[] data = new byte[64];
            data[0] = (byte)DeviceCommands.PinConfig;
            data[1] = (byte)PinNumber;
            cmd.CopyTo(data, 2);
            Board.sendPinConfigPacket(data);
        }

        internal virtual void UpdateValue(byte highByte, byte lowByte)
        {
            if(State == PinState.DigitalInput)
            {
                bool newVal = highByte > 0;
                if (digitalValue != newVal) // we have a new value!
                {
                    digitalValue = newVal;
                    RaiseDigitalInValueChanged();
                    RaisePropertyChanged("DigitalValue");
                }
            } else if(State == PinState.AnalogInput)
            {

            }
            
        }

        public virtual void MakeAnalogInput()
        {
            if (State == PinState.AnalogInput)
                return;
            SendCommand(new byte[] { (byte)PinConfigCommands.MakeAnalogInput });
            State = PinState.AnalogInput;
        }


                /// <summary>
        /// Occurs when an analog voltage is changed.
        /// </summary>
        /// <remarks>
        /// The Changed event is raised when the 10-bit ADC value obtained is different from the previous reading.
        /// </remarks>
        public event OnAnalogInVoltageChanged AnalogVoltageChanged;

        /// <summary>
        /// Occurs when an analog value is changed.
        /// </summary>
        /// <remarks>
        /// The Changed event is raised when the 10-bit ADC value obtained is different from the previous reading.
        /// </remarks>
        public event OnAnalogInValueChanged AnalogValueChanged;
        double adcVoltage;
        int adcValue;


        /// <summary>
        /// Retrieve the last value obtained from the ADC. 
        /// </summary>
        /// <remarks>
        /// Treehopper has a 12-bit ADC, so ADC values will range from 0-4096.
        /// </remarks>
        public int AdcValue
        {
            get
            {
                return this.adcValue;
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
        public double AdcVoltage
        {
            get
            {
                return this.adcVoltage;
            }
        }

        /// <summary>
        /// Retrieve the last reading from the Adc, expressed as a percent of VREF
        /// </summary>
        /// <remarks>
        /// </remarks>
        public double AdcPercent
        {
            get
            {
                return this.AdcPercent;
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

            if(adcValue != val) // compare the actual ADC values, not the floating-point conversions.
            {
                adcValue = val;
                this.adcVoltage = analogVal;
                if(AnalogValueChanged != null)
                {
                    AnalogValueChanged(this, this.adcValue);
                }
                
                if(AnalogVoltageChanged != null)
                {
                    AnalogVoltageChanged(this, this.adcVoltage);
                }

                RaisePropertyChanged("AdcValue");
                RaisePropertyChanged("AdcVoltage");
                RaisePropertyChanged("AdcPercent");
            }
        }


        /// <summary>
        /// This event fires whenever a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        internal void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
