﻿namespace Treehopper
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Utilities;

    /// <summary>
    /// Represents an I/O pin on Treehopper; it provides core digital I/O (GPIO) and ADC functionality.
    /// </summary>
    public class Pin : INotifyPropertyChanged, DigitalInPin, DigitalOutPin, AdcPin, SpiChipSelectPin
    {
        private TreehopperUsb board;
        private PinMode mode = PinMode.Unassigned;
        private AdcReferenceLevel referenceLevel;
        private bool digitalValue;
        private double referenceLevelVoltage;
        private int prevAdcValue;
        private double prevAnalogVoltage;
        private double prevAnalogValue;
        private int adcValue;
        private TaskCompletionSource<bool> digitalSignal = new TaskCompletionSource<bool>();
        private TaskCompletionSource<int> adcValueSignal = new TaskCompletionSource<int>();
        private TaskCompletionSource<double> analogValueSignal = new TaskCompletionSource<double>();
        private TaskCompletionSource<double> analogVoltageSignal = new TaskCompletionSource<double>();

        internal Pin(TreehopperUsb board, byte pinNumber)
        {
            this.board = board;
            this.PinNumber = pinNumber;
            SoftPwm = new SoftPwm(Board, this);
            this.ReferenceLevel = AdcReferenceLevel.VREF_3V3;
            Name = "Pin " + pinNumber; // default name
        }

        /// <summary>
        /// Occurs when an analog voltage is changed, according to the set threshold.
        /// </summary>
        /// <remarks>
        /// The Changed event is raised when the 12-bit ADC value obtained is different from the previous reading 
        /// by at least the value specified by <see cref="AnalogVoltageChangedThreshold"/>.
        /// </remarks>
        public event OnAnalogVoltageChanged AnalogVoltageChanged;

        /// <summary>
        /// Occurs when an analog value is changed, according to the set threshold.
        /// </summary>
        /// <remarks>
        /// The Changed event is raised when the 10-bit ADC value obtained is different from the previous reading
        /// by at least the value specified by <see cref="AdcValueChangedThreshold"/>
        /// </remarks>
        public event OnAnalogValueChanged AnalogValueChanged;

        /// <summary>
        /// Occurs when the normalized analog value is changed, according to the set threshold.
        /// </summary>
        /// <remarks>
        /// The Changed event is raised when the 10-bit ADC value obtained is different from the previous reading.
        /// </remarks>
        public event OnAdcValueChanged AdcValueChanged;

        /// <summary>
        /// This event fires whenever a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when the input on the pin changes.
        /// </summary>
        /// <remarks>
        /// This event will only fire when the pin is configured as a digital input.
        /// </remarks>
        public event OnDigitalInValueChanged DigitalValueChanged;

        internal enum PinConfigCommands
        {
            Reserved = 0,
            MakeDigitalInput,
            MakePushPullOutput,
            MakeOpenDrainOutput,
            MakeAnalogInput,
            SetDigitalValue,
        }

        /// <summary>
        /// Get or set the mode of the pin.
        /// </summary>
        public PinMode Mode
        {
            get
            {
                return mode;
            }

            set
            {
                if (value == mode)
                    return;
                if (mode == PinMode.Reserved && value != PinMode.Unassigned)
                {
                    throw new Exception("This pin is reserved; you must disable the peripheral using it before interacting with it");
                }

                mode = value;

                switch (mode)
                {
                    case PinMode.AnalogInput:
                        SendCommand(new byte[] { (byte)PinConfigCommands.MakeAnalogInput, (byte)ReferenceLevel });
                        break;
                    case PinMode.DigitalInput:
                        SendCommand(new byte[] { (byte)PinConfigCommands.MakeDigitalInput, 0x00 });
                        break;
                    case PinMode.OpenDrainOutput:
                        SendCommand(new byte[] { (byte)PinConfigCommands.MakeOpenDrainOutput, 0x00 });
                        break;
                    case PinMode.PushPullOutput:
                        SendCommand(new byte[] { (byte)PinConfigCommands.MakePushPullOutput, 0x00 });
                        break;
                }
            }
        }

        /// <summary>
        /// This returns a reference to the Treehopper board this pin belongs to.
        /// </summary>
        public TreehopperUsb Board
        {
            get { return board; }
        }

        public string Name { get; internal set; }

        /// <summary>
        /// The pin number of the pin.
        /// </summary>
        public int PinNumber { get; internal set; }

        /// <summary>
        /// The SoftPwm functions associated with this pin.
        /// </summary>
        public SoftPwm SoftPwm { get; internal set; }

        /// <summary>
        /// Gets or sets the voltage threshold required to fire the AnalogVoltageChanged event.
        /// </summary>
        public double AnalogVoltageChangedThreshold { get; set; } = 0.05;

        /// <summary>
        /// Gets or sets the value threshold required to fire the AdcValueChanged event.
        /// </summary>
        public int AdcValueChangedThreshold { get; set; } = 10;

        /// <summary>
        /// Gets or sets the value threshold required to fire the AnalogValueChanged event.
        /// </summary>
        public double AnalogValueChangedThreshold { get; set; } = 0.01;

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
                if (Mode == PinMode.Reserved || Mode == PinMode.AnalogInput)
                    Debug.WriteLine(string.Format("NOTICE: Pin {0} must be in digital I/O mode to read from. This call will return 0 always.", PinNumber));

                return digitalValue;
            }

            set
            {
                digitalValue = value;
                if (!(Mode == PinMode.PushPullOutput || Mode == PinMode.OpenDrainOutput))
                    Mode = PinMode.PushPullOutput; // assume they want push-pull
                byte byteVal = (byte)(digitalValue ? 0x01 : 0x00);
                SendCommand(new byte[] { (byte)PinConfigCommands.SetDigitalValue, byteVal });
            }
        }

        /// <summary>
        /// Retrieve the last value obtained from the ADC. 
        /// </summary>
        /// <remarks>
        /// Treehopper has a 12-bit ADC, so ADC values will range from 0-4095.
        /// </remarks>
        public int AdcValue
        {
            get
            {
                if (Mode != PinMode.AnalogInput)
                    Debug.WriteLine(string.Format("NOTICE: Attempting to read AdcValue from Pin {0}, which is configured for {1}. This call will always return 0", PinNumber, Mode));

                return adcValue;
            }
        }

        /// <summary>
        /// Retrieve the last voltage reading from the ADC.
        /// </summary>
        public double AnalogVoltage
        {
            get
            {
                return Math.Round((double)AdcValue * (referenceLevelVoltage / 4092.0), 4);
            }
        }

        /// <summary>
        /// Retrieve the last reading from the ADC, expressed on a unit range (0.0 - 1.0)
        /// </summary>
        /// <remarks>
        /// </remarks>
        public double AnalogValue
        {
            get
            {
                return Math.Round((double)AdcValue / 4092.0, 4);
            }
        }

        /// <summary>
        /// Sets the ADC reference value used 
        /// </summary>
        public AdcReferenceLevel ReferenceLevel
        {
            get
            {
                return referenceLevel;
            }

            set
            {
                referenceLevel = value;
                switch (referenceLevel)
                {
                    case AdcReferenceLevel.VREF_1V65:
                        referenceLevelVoltage = 1.65;
                        break;
                    case AdcReferenceLevel.VREF_1V8:
                        referenceLevelVoltage = 1.8;
                        break;
                    case AdcReferenceLevel.VREF_2V4:
                        referenceLevelVoltage = 2.4;
                        break;
                    case AdcReferenceLevel.VREF_3V3:
                        referenceLevelVoltage = 3.3;
                        break;
                    case AdcReferenceLevel.VREF_3V3_DERIVED:
                        referenceLevelVoltage = 3.3;
                        break;
                    case AdcReferenceLevel.VREF_3V6:
                        referenceLevelVoltage = 3.6;
                        break;
                }

                // if we're already an analog input, re-send the command to set the new reference level
                if (Mode == PinMode.AnalogInput)
                    SendCommand(new byte[] { (byte)PinConfigCommands.MakeAnalogInput, (byte)ReferenceLevel });
            }
        }

        public Spi SpiModule
        {
            get
            {
                return board.Spi;
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
            DigitalValue = !DigitalValue;
        }

        /// <summary>
        /// Wait for the digital input value of the pin to change
        /// </summary>
        /// <returns>An awaitable bool, indicating the pin's state</returns>
        public Task<bool> AwaitDigitalValueChange()
        {
            digitalSignal = new TaskCompletionSource<bool>();
            return digitalSignal.Task;
        }

        /// <summary>
        /// Wait for the pin's ADC value to change.
        /// </summary>
        /// <returns>An awaitable int, in the range of 0-4095, of the pin's ADC value.</returns>
        public Task<int> AwaitAdcValueChange()
        {
            adcValueSignal = new TaskCompletionSource<int>();
            return adcValueSignal.Task;
        }

        /// <summary>
        /// Wait for the pin's analog voltage to change.
        /// </summary>
        /// <returns>An awaitable double of the pin's analog voltage, measured in volts.</returns>
        public Task<double> AwaitAnalogVoltageChange()
        {
            analogVoltageSignal = new TaskCompletionSource<double>();
            return analogVoltageSignal.Task;
        }

        /// <summary>
        /// Wait for the pin's analog value to change.
        /// </summary>
        /// <returns>An awaitable double of the analog value, normalized from 0-1.</returns>
        public Task<double> AwaitAnalogValueChange()
        {
            analogValueSignal = new TaskCompletionSource<double>();
            return analogValueSignal.Task;
        }

        /// <summary>
        /// Make the pin a push-pull output.
        /// </summary>
        public void MakeDigitalPushPullOut()
        {
            Mode = PinMode.PushPullOutput;
        }

        /// <summary>
        /// Make the pin a digital input.
        /// </summary>
        public void MakeDigitalIn()
        {
            Mode = PinMode.DigitalInput;
        }

        /// <summary>
        /// Make the pin an analog input.
        /// </summary>
        public void MakeAnalogIn()
        {
            Mode = PinMode.AnalogInput;
        }

        public override string ToString()
        {
            if (SoftPwm.Enabled)
                return Name + ": " + SoftPwm.ToString();
            switch (Mode)
            {
                case PinMode.AnalogInput:
                    return Name + ": " + string.Format("Analog input, {0:0.00} volts", AnalogVoltage);
                case PinMode.DigitalInput:
                    return Name + ": " + string.Format("Digital input, {0}", DigitalValue);
                case PinMode.OpenDrainOutput:
                    return Name + ": " + string.Format("Open-drain output, {0}", DigitalValue);
                case PinMode.PushPullOutput:
                    return Name + ": " + string.Format("Push-pull output, {0}", DigitalValue);
                case PinMode.Reserved:
                    return Name + ": In use by peripheral";

                default:
                case PinMode.Unassigned:
                    return Name + ": Unassigned";
            }
        }

        internal virtual void UpdateValue(byte highByte, byte lowByte)
        {
            if (Mode == PinMode.DigitalInput)
            {
                bool newVal = highByte > 0;
                if (digitalValue != newVal)
                {
                    // we have a new value!
                    digitalValue = newVal;
                    RaiseDigitalInValueChanged();
                    RaisePropertyChanged("DigitalValue");
                }
            }
            else if (Mode == PinMode.AnalogInput)
            {
                int val = ((int)highByte) << 8;
                val |= (int)lowByte;
                adcValue = val;
                RaiseAnalogInChanged();
            }
        }

        internal void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        internal void RaiseDigitalInValueChanged()
        {
            DigitalValueChanged?.Invoke(this, new DigitalInValueChangedEventArgs(digitalValue));

            digitalSignal.TrySetResult(digitalValue);
        }

        internal void RaiseAnalogInChanged()
        {
            if (!prevAdcValue.CloseTo(adcValue, AdcValueChangedThreshold))
            {
                prevAdcValue = adcValue;
                AdcValueChanged?.Invoke(this, new AdcValueChangedEventArgs(adcValue));

                adcValueSignal.TrySetResult(adcValue);

                RaisePropertyChanged("AdcValue");
            }

            if (!prevAnalogVoltage.CloseTo(AnalogVoltage, AnalogVoltageChangedThreshold))
            {
                prevAnalogVoltage = AnalogVoltage;
                AnalogVoltageChanged?.Invoke(this, new AnalogVoltageChangedEventArgs(AnalogVoltage));

                analogVoltageSignal.TrySetResult(AnalogVoltage);

                RaisePropertyChanged("AnalogVoltage");
            }

            if (!prevAnalogValue.CloseTo(AnalogValue, AnalogValueChangedThreshold))
            {
                prevAnalogValue = AnalogValue;
                AnalogValueChanged?.Invoke(this, new AnalogValueChangedEventArgs(AnalogValue));

                analogValueSignal.TrySetResult(AnalogValue);

                RaisePropertyChanged("AnalogValue");
            }
        }

        internal void SendCommand(byte[] cmd)
        {
            byte[] data = new byte[6];
            data[0] = (byte)PinNumber;
            cmd.CopyTo(data, 1);
            Board.SendPinConfigPacket(data);
        }
    }
}
