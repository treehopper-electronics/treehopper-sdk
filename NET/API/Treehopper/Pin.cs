using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Treehopper.Utilities;

namespace Treehopper
{
    /// <summary>
    ///     Represents an I/O pin on Treehopper; it provides core digital I/O (GPIO) and ADC functionality.
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public class Pin : INotifyPropertyChanged, DigitalIn, DigitalOut, AdcPin, SpiChipSelectPin
    {
        private int _adcValue;
        private TaskCompletionSource<int> _adcValueSignal = new TaskCompletionSource<int>();
        private TaskCompletionSource<double> _analogValueSignal = new TaskCompletionSource<double>();
        private TaskCompletionSource<double> _analogVoltageSignal = new TaskCompletionSource<double>();
        private TaskCompletionSource<bool> _digitalSignal = new TaskCompletionSource<bool>();
        private bool _digitalValue;
        private PinMode _mode = PinMode.Unassigned;
        private int _prevAdcValue;
        private double _prevAnalogValue;
        private double _prevAnalogVoltage;
        private AdcReferenceLevel _referenceLevel;
        private double _referenceLevelVoltage;

        internal Pin(TreehopperUsb board, byte pinNumber)
        {
            Board = board;
            PinNumber = pinNumber;
            SoftPwm = new SoftPwm(Board, this);
            ReferenceLevel = AdcReferenceLevel.Vref_3V3;
            Name = "Pin " + pinNumber; // default name
        }

        /// <summary>
        ///     Get or set the mode of the pin.
        /// </summary>
        public PinMode Mode
        {
            get { return _mode; }

            set
            {
                if (value == _mode)
                    return;
                if (_mode == PinMode.Reserved && value != PinMode.Unassigned)
                    throw new Exception(
                        "This pin is reserved; you must disable the peripheral using it before interacting with it");

                _mode = value;

                switch (_mode)
                {
                    case PinMode.AnalogInput:
                        if (TreehopperUsb.Settings.PropertyWritesReturnImmediately)
                            MakeAnalogIn().Forget();
                        else
                            MakeAnalogIn().Wait();
                        break;

                    case PinMode.DigitalInput:
                        if (TreehopperUsb.Settings.PropertyWritesReturnImmediately)
                            MakeDigitalIn().Forget();
                        else
                            MakeDigitalIn().Wait();
                        break;

                    case PinMode.OpenDrainOutput:
                        if (TreehopperUsb.Settings.PropertyWritesReturnImmediately)
                            MakeDigitalOpenDrainOut().Forget();
                        else
                            MakeDigitalOpenDrainOut().Wait();

                        _digitalValue = false; // set initial state
                        break;

                    case PinMode.PushPullOutput:
                        if (TreehopperUsb.Settings.PropertyWritesReturnImmediately)
                            MakeDigitalPushPullOut().Forget();
                        else
                            MakeDigitalPushPullOut().Wait();

                        _digitalValue = false;
                        break;
                }
            }
        }

        /// <summary>
        ///     This returns a reference to the Treehopper board this pin belongs to.
        /// </summary>
        public TreehopperUsb Board { get; }

        /// <summary>
        ///     Gets the name of the pin
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        ///     The SoftPwm functions associated with this pin.
        /// </summary>
        public SoftPwm SoftPwm { get; internal set; }

        /// <summary>
        ///     Sets the ADC reference value used
        /// </summary>
        public AdcReferenceLevel ReferenceLevel
        {
            get { return _referenceLevel; }

            set
            {
                _referenceLevel = value;
                switch (_referenceLevel)
                {
                    case AdcReferenceLevel.Vref_1V65:
                        _referenceLevelVoltage = 1.65;
                        break;
                    case AdcReferenceLevel.Vref_1V8:
                        _referenceLevelVoltage = 1.8;
                        break;
                    case AdcReferenceLevel.Vref_2V4:
                        _referenceLevelVoltage = 2.4;
                        break;
                    case AdcReferenceLevel.Vref_3V3:
                        _referenceLevelVoltage = 3.3;
                        break;
                    case AdcReferenceLevel.Vref_3V3Derived:
                        _referenceLevelVoltage = 3.3;
                        break;
                    case AdcReferenceLevel.Vref_3V6:
                        _referenceLevelVoltage = 3.6;
                        break;
                }

                // if we're already an analog input, re-send the command to set the new reference level
                if (Mode == PinMode.AnalogInput)
                    SendCommand(new[] {(byte) PinConfigCommands.MakeAnalogInput, (byte) ReferenceLevel});
            }
        }

        /// <summary>
        ///     Occurs when an analog voltage is changed, according to the set threshold.
        /// </summary>
        /// <remarks>
        ///     The Changed event is raised when the 12-bit ADC value obtained is different from the previous reading
        ///     by at least the value specified by <see cref="AnalogVoltageChangedThreshold" />.
        /// </remarks>
        public event OnAnalogVoltageChanged AnalogVoltageChanged;

        /// <summary>
        ///     Occurs when an analog value is changed, according to the set threshold.
        /// </summary>
        /// <remarks>
        ///     The Changed event is raised when the 10-bit ADC value obtained is different from the previous reading
        ///     by at least the value specified by <see cref="AdcValueChangedThreshold" />
        /// </remarks>
        public event OnAnalogValueChanged AnalogValueChanged;

        /// <summary>
        ///     Occurs when the normalized analog value is changed, according to the set threshold.
        /// </summary>
        /// <remarks>
        ///     The Changed event is raised when the 10-bit ADC value obtained is different from the previous reading.
        /// </remarks>
        public event OnAdcValueChanged AdcValueChanged;

        /// <summary>
        ///     Gets or sets the voltage threshold required to fire the AnalogVoltageChanged event.
        /// </summary>
        public double AnalogVoltageChangedThreshold { get; set; } = 0.05;

        /// <summary>
        ///     Gets or sets the value threshold required to fire the AdcValueChanged event.
        /// </summary>
        public int AdcValueChangedThreshold { get; set; } = 10;

        /// <summary>
        ///     Gets or sets the value threshold required to fire the AnalogValueChanged event.
        /// </summary>
        public double AnalogValueChangedThreshold { get; set; } = 0.01;

        /// <summary>
        ///     Retrieve the last value obtained from the ADC.
        /// </summary>
        /// <remarks>
        ///     Treehopper has a 12-bit ADC, so ADC values will range from 0-4095.
        /// </remarks>
        public int AdcValue
        {
            get
            {
                if (Mode != PinMode.AnalogInput)
                    Debug.WriteLine(
                        $"NOTICE: Attempting to read AdcValue from Pin {PinNumber}, which is configured for {Mode}. This call will always return 0");

                return _adcValue;
            }
        }

        /// <summary>
        ///     Retrieve the last voltage reading from the ADC.
        /// </summary>
        public double AnalogVoltage => Math.Round(AdcValue * (_referenceLevelVoltage / 4092.0), 4);

        /// <summary>
        ///     Retrieve the last reading from the ADC, expressed on a unit range (0.0 - 1.0)
        /// </summary>
        /// <remarks>
        /// </remarks>
        public double AnalogValue => Math.Round(AdcValue / 4092.0, 4);

        /// <summary>
        ///     Make the pin an analog input.
        /// </summary>
        public Task MakeAnalogIn()
        {
            _mode = PinMode.AnalogInput;
            return SendCommand(new[] {(byte) PinConfigCommands.MakeAnalogInput, (byte) ReferenceLevel});
        }

        /// <summary>
        ///     Occurs when the input on the pin changes.
        /// </summary>
        /// <remarks>
        ///     This event will only fire when the pin is configured as a digital input.
        /// </remarks>
        public event OnDigitalInValueChanged DigitalValueChanged;

        /// <summary>
        ///     Gets or sets the digital value of the pin.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Setting a value to a pin that is configured as an input will automatically make the pin an output before
        ///         writing the value to it.
        ///     </para>
        ///     <para>
        ///         The value retrieved from this pin will read as "0" when then pin is being used for other purposes.
        ///     </para>
        /// </remarks>
        public bool DigitalValue
        {
            get
            {
                if (Mode == PinMode.Reserved || Mode == PinMode.AnalogInput)
                    Debug.WriteLine(
                        $"NOTICE: Pin {PinNumber} must be in digital I/O mode to read from. This call will return 0 always.");

                return _digitalValue;
            }

            set
            {
                if (_digitalValue == value) return;
                _digitalValue = value;
                if (TreehopperUsb.Settings.PropertyWritesReturnImmediately)
                    WriteDigitalValueAsync(_digitalValue).Forget(); // send off the request and move on.
                else
                    WriteDigitalValueAsync(_digitalValue).Wait(); // wait for it to complete
            }
        }

        /// <summary>
        ///     Wait for the digital input value of the pin to change
        /// </summary>
        /// <returns>An awaitable bool, indicating the pin's state</returns>
        public Task<bool> AwaitDigitalValueChange()
        {
            _digitalSignal = new TaskCompletionSource<bool>();
            return _digitalSignal.Task;
        }

        /// <summary>
        ///     Make the pin a digital input.
        /// </summary>
        public Task MakeDigitalIn()
        {
            _mode = PinMode.DigitalInput;
            return SendCommand(new byte[] {(byte) PinConfigCommands.MakeDigitalInput, 0});
        }

        /// <summary>
        ///     Toggles the output value of the pin.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Calling this function on a pin that is configured as an input will automatically make the pin an output
        ///         before writing the value to it.
        ///     </para>
        /// </remarks>
        /// <example>
        ///     In this example, an LED attached to pin 4 is made to blink.
        ///     <code>
        /// Pin led = myTreehopperBoard.Pin4; // create a reference to Pin4 to keep code concise.
        /// led.MakeDigitalOutput();
        /// while(true)
        /// {
        ///     led.Toggle();
        ///     Thread.Sleep(500);
        /// }
        /// </code>
        /// </example>
        public Task ToggleOutputAsync()
        {
            return WriteDigitalValueAsync(!DigitalValue);
        }

        /// <summary>
        ///     Make the pin a push-pull output.
        /// </summary>
        public Task MakeDigitalPushPullOut()
        {
            _mode = PinMode.PushPullOutput;
            return SendCommand(new byte[] {(byte) PinConfigCommands.MakePushPullOutput, 0});
        }

        /// <summary>
        ///     This event fires whenever a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     The pin number of the pin.
        /// </summary>
        public int PinNumber { get; internal set; }

        /// <summary>
        ///     Gets the Spi module that can use this pin for chip-select duties
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Since the SPI chip-select functionality is done in-hardware, the SPI module must check to ensure the pin
        ///         you're using for chip-select actually belongs to the same board as the SPI module does (as you may have
        ///         multiple boards attached).
        ///     </para>
        /// </remarks>
        public Spi SpiModule => Board.Spi;

        /// <summary>
        ///     Write a value to a pin
        /// </summary>
        /// <param name="value">The value to write</param>
        /// <returns>An awaitable task that completes upon success</returns>
        /// <remarks>
        ///     <para>
        ///         This method is functionally equivalent of calling "<see cref="DigitalValue" />=value", however, it provides
        ///         finer-grained control over asynchronous behavior. You may choose to await it, block it synchronously, or
        ///         "forget" it (continue execution without waiting at all).
        ///     </para>
        /// </remarks>
        public async Task WriteDigitalValueAsync(bool value)
        {
            _digitalValue = value;
            if (!(Mode == PinMode.PushPullOutput || Mode == PinMode.OpenDrainOutput))
                await MakeDigitalPushPullOut().ConfigureAwait(false); // assume they want push-pull

            var byteVal = (byte) (_digitalValue ? 0x01 : 0x00);
            await SendCommand(new[] {(byte) PinConfigCommands.SetDigitalValue, byteVal}).ConfigureAwait(false);
        }

        /// <summary>
        ///     Wait for the pin's ADC value to change.
        /// </summary>
        /// <returns>An awaitable int, in the range of 0-4095, of the pin's ADC value.</returns>
        public Task<int> AwaitAdcValueChange()
        {
            _adcValueSignal = new TaskCompletionSource<int>();
            return _adcValueSignal.Task;
        }

        /// <summary>
        ///     Wait for the pin's analog voltage to change.
        /// </summary>
        /// <returns>An awaitable double of the pin's analog voltage, measured in volts.</returns>
        public Task<double> AwaitAnalogVoltageChange()
        {
            _analogVoltageSignal = new TaskCompletionSource<double>();
            return _analogVoltageSignal.Task;
        }

        /// <summary>
        ///     Wait for the pin's analog value to change.
        /// </summary>
        /// <returns>An awaitable double of the analog value, normalized from 0-1.</returns>
        public Task<double> AwaitAnalogValueChange()
        {
            _analogValueSignal = new TaskCompletionSource<double>();
            return _analogValueSignal.Task;
        }

        /// <summary>
        ///     Make the pin a push-pull output.
        /// </summary>
        public Task MakeDigitalOpenDrainOut()
        {
            _mode = PinMode.OpenDrainOutput;
            return SendCommand(new byte[] {(byte) PinConfigCommands.MakeOpenDrainOutput, 0});
        }

        /// <summary>
        ///     Gets a string representation of the pin's current state
        /// </summary>
        /// <returns>the pin's current state</returns>
        public override string ToString()
        {
            if (SoftPwm.Enabled)
                return Name + ": " + SoftPwm;
            switch (Mode)
            {
                case PinMode.AnalogInput:
                    return Name + ": " + $"Analog input, {AnalogVoltage:0.00} volts";
                case PinMode.DigitalInput:
                    return Name + ": " + $"Digital input, {DigitalValue}";
                case PinMode.OpenDrainOutput:
                    return Name + ": " + $"Open-drain output, {DigitalValue}";
                case PinMode.PushPullOutput:
                    return Name + ": " + $"Push-pull output, {DigitalValue}";
                case PinMode.Reserved:
                    return Name + ": In use by peripheral";

                default:
                    return Name + ": Unassigned";
            }
        }

        internal virtual void UpdateValue(byte highByte, byte lowByte)
        {
            if (Mode == PinMode.DigitalInput)
            {
                var newVal = highByte > 0;
                if (_digitalValue != newVal)
                {
                    // we have a new value!
                    _digitalValue = newVal;
                    RaiseDigitalInValueChanged();
                    RaisePropertyChanged("DigitalValue");
                }
            }
            else if (Mode == PinMode.AnalogInput)
            {
                var val = highByte << 8;
                val |= lowByte;
                _adcValue = val;
                RaiseAnalogInChanged();
            }
        }

        internal void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        internal void RaiseDigitalInValueChanged()
        {
            DigitalValueChanged?.Invoke(this, new DigitalInValueChangedEventArgs(_digitalValue));

            _digitalSignal.TrySetResult(_digitalValue);
        }

        internal void RaiseAnalogInChanged()
        {
            if (!_prevAdcValue.CloseTo(_adcValue, AdcValueChangedThreshold))
            {
                _prevAdcValue = _adcValue;
                AdcValueChanged?.Invoke(this, new AdcValueChangedEventArgs(_adcValue));

                _adcValueSignal.TrySetResult(_adcValue);

                RaisePropertyChanged("AdcValue");
            }

            if (!_prevAnalogVoltage.CloseTo(AnalogVoltage, AnalogVoltageChangedThreshold))
            {
                _prevAnalogVoltage = AnalogVoltage;
                AnalogVoltageChanged?.Invoke(this, new AnalogVoltageChangedEventArgs(AnalogVoltage));

                _analogVoltageSignal.TrySetResult(AnalogVoltage);

                RaisePropertyChanged("AnalogVoltage");
            }

            if (!_prevAnalogValue.CloseTo(AnalogValue, AnalogValueChangedThreshold))
            {
                _prevAnalogValue = AnalogValue;
                AnalogValueChanged?.Invoke(this, new AnalogValueChangedEventArgs(AnalogValue));

                _analogValueSignal.TrySetResult(AnalogValue);

                RaisePropertyChanged("AnalogValue");
            }
        }

        internal Task SendCommand(byte[] cmd)
        {
            var data = new byte[6];
            data[0] = (byte) PinNumber;
            cmd.CopyTo(data, 1);
            return Board.SendPinConfigPacket(data);
        }

        internal enum PinConfigCommands
        {
            Reserved = 0,
            MakeDigitalInput,
            MakePushPullOutput,
            MakeOpenDrainOutput,
            MakeAnalogInput,
            SetDigitalValue
        }
    }
}