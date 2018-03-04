using System.Threading.Tasks;

namespace Treehopper.Libraries.IO.PortExpander
{
    /// <summary>
    ///     Represents a digital I/O pin's mode
    /// </summary>
    public enum PortExpanderPinMode
    {
        /// <summary>
        ///     Digital output mode
        /// </summary>
        DigitalOutput,

        /// <summary>
        ///     Digital input mode
        /// </summary>
        DigitalInput
    }

    /// <summary>
    ///     Construct a new Port Expander pin
    /// </summary>
    public class PortExpanderPin : IPortExpanderPin
    {
        private TaskCompletionSource<bool> digitalSignal = new TaskCompletionSource<bool>();

        private bool digitalValue;

        /// <summary>
        ///     Pin mode for this pin
        /// </summary>
        protected PortExpanderPinMode mode = PortExpanderPinMode.DigitalInput;

        /// <summary>
        ///     Pin (bit) number
        /// </summary>
        protected int pinNumber;

        /// <summary>
        ///     Port expander this pin belongs to
        /// </summary>
        protected IPortExpanderParent portExpander;

        internal PortExpanderPin(IPortExpanderParent portExpander, int pinNumber)
        {
            this.portExpander = portExpander;
            this.pinNumber = pinNumber;
        }

        /// <summary>
        ///     The mode of the pin
        /// </summary>
        public PortExpanderPinMode Mode
        {
            get { return mode; }
            set
            {
                if (mode == value)
                    return;

                if (mode == PortExpanderPinMode.DigitalOutput)
                    Task.Run(MakeDigitalPushPullOut).Wait();
                else
                    Task.Run(MakeDigitalIn).Wait();
            }
        }

        /// <summary>
        ///     Gets or sets the output value of the pin when an output, or get the current value of the pin when an input
        /// </summary>
        public bool DigitalValue
        {
            get
            {
                if (mode == PortExpanderPinMode.DigitalInput && portExpander.AutoUpdateWhenPropertyRead)
                    Task.Run(portExpander.UpdateAsync).Wait();
                return digitalValue;
            }

            set
            {
                Task.Run(MakeDigitalPushPullOut).Wait(); // if we try to write to the pin, set it as an output
                if (digitalValue == value)
                    return;

                digitalValue = value;
                Task.Run(() => portExpander.OutputValueChanged(this)).Wait();
            }
        }

        /// <summary>
        ///     The pin number of this port expander pin
        /// </summary>
        public int PinNumber => pinNumber;

        /// <summary>
        ///     Event occurs whenever a digital input value has changed
        /// </summary>
        public event OnDigitalInValueChanged DigitalValueChanged;

        /// <summary>
        ///     Wait for the digital input to change
        /// </summary>
        /// <returns>The new digital value (when the wait completes)</returns>
        public Task<bool> AwaitDigitalValueChange()
        {
            if (portExpander.AutoUpdateWhenPropertyRead)
                return Task.Run(async () =>
                {
                    var oldValue = DigitalValue;
                    // poll the device
                    while (DigitalValue == oldValue)
                    {
                        await portExpander.UpdateAsync().ConfigureAwait(false);
                        await Task.Delay(portExpander.AwaitPollingInterval).ConfigureAwait(false);
                    }

                    return DigitalValue;
                });
            // The app is updating
            digitalSignal = new TaskCompletionSource<bool>();
            return digitalSignal.Task;
        }

        /// <summary>
        ///     Make the pin a digital input
        /// </summary>
        public Task MakeDigitalIn()
        {
            return portExpander.OutputModeChanged(this);
        }

        /// <summary>
        ///     Toggle the pin's output value
        /// </summary>
        public async Task ToggleOutputAsync()
        {
            await MakeDigitalPushPullOut().ConfigureAwait(false);
            DigitalValue = !DigitalValue;
        }

        /// <summary>
        ///     Make the pin a digital output
        /// </summary>
        public Task MakeDigitalPushPullOut()
        {
            this.mode = PortExpanderPinMode.DigitalOutput;
            return portExpander.OutputModeChanged(this);
        }

        internal void UpdateInputValue(bool value)
        {
            if (digitalValue == value) return;
            digitalValue = value;
            DigitalValueChanged?.Invoke(this, new DigitalInValueChangedEventArgs(digitalValue));
            digitalSignal.TrySetResult(digitalValue);
        }

        /// <summary>
        ///     Gets a string representation of the pin's current state
        /// </summary>
        /// <returns>the pin's current state</returns>
        public override string ToString()
        {
            switch (Mode)
            {
                case PortExpanderPinMode.DigitalInput:
                    return $"Pin {pinNumber}: Digital input, {DigitalValue}";
                case PortExpanderPinMode.DigitalOutput:
                    return $"Pin {pinNumber}: Digital output, {DigitalValue}";

                default:
                    return $"Pin {pinNumber}: Unknown";
            }
        }
    }
}