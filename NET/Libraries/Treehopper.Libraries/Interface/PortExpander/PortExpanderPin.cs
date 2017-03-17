using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface.PortExpander
{
    /// <summary>
    /// Represents a digital I/O pin's mode
    /// </summary>
    public enum PortExpanderPinMode
    {
        /// <summary>
        /// Digital output mode
        /// </summary>
        DigitalOutput,

        /// <summary>
        /// Digital input mode
        /// </summary>
        DigitalInput
    }

    /// <summary>
    /// Construct a new Port Expander pin
    /// </summary>
    public class PortExpanderPin : IPortExpanderPin
    {
        /// <summary>
        /// Pin (bit) number
        /// </summary>
        protected int pinNumber;

        /// <summary>
        /// Port expander this pin belongs to
        /// </summary>
        protected IPortExpanderParent portExpander;

        /// <summary>
        /// Pin mode for this pin
        /// </summary>
        protected PortExpanderPinMode mode = PortExpanderPinMode.DigitalInput;

        /// <summary>
        /// The mode of the pin
        /// </summary>
        public PortExpanderPinMode Mode
        {
            get { return mode; }
            set
            {
                if (mode == value) return;
                mode = value;
                if (mode == PortExpanderPinMode.DigitalOutput)
                    MakeDigitalPushPullOut();
                else
                    MakeDigitalIn();
            }
        }

        private bool digitalValue;

        /// <summary>
        /// Gets or sets the output value of the pin when an output, or get the current value of the pin when an input
        /// </summary>
        public bool DigitalValue
        {
            get
            {
                if (mode == PortExpanderPinMode.DigitalInput && portExpander.AutoUpdateWhenPropertyRead) portExpander.Update().Wait();
                return digitalValue;
            }

            set
            {
                MakeDigitalPushPullOut(); // if we try to write to the pin, set it as an output
                if (digitalValue == value) return;
                digitalValue = value;
                portExpander.OutputValueChanged(this);
            }
        }

        /// <summary>
        /// The pin number of this port expander pin
        /// </summary>
        public int PinNumber
        {
            get
            {
                return pinNumber;
            }
        }

        internal void UpdateInputValue(bool value)
        {
            if (digitalValue == value) return;
            digitalValue = value;
            DigitalValueChanged?.Invoke((DigitalInPin)this, new DigitalInValueChangedEventArgs(digitalValue));
            digitalSignal.TrySetResult(digitalValue);
        }
        internal PortExpanderPin(IPortExpanderParent portExpander, int pinNumber)
        {
            this.portExpander = portExpander;
            this.pinNumber = pinNumber;
        }

        TaskCompletionSource<bool> digitalSignal = new TaskCompletionSource<bool>();

        /// <summary>
        /// Event occurs whenever a digital input value has changed
        /// </summary>
        public event OnDigitalInValueChanged DigitalValueChanged;

        /// <summary>
        /// Wait for the digital input to change
        /// </summary>
        /// <returns>The new digital value (when the wait completes)</returns>
        public Task<bool> AwaitDigitalValueChange()
        {
            if(portExpander.AutoUpdateWhenPropertyRead)
            {
                return Task.Run(async() =>
                {
                    bool oldValue = DigitalValue;
                    // poll the device
                    while(DigitalValue == oldValue)
                    {
                        await portExpander.Update().ConfigureAwait(false);
                        await Task.Delay(portExpander.AwaitPollingInterval);
                    }

                    return DigitalValue;

                });
            } else
            {
                // The app is updating
                digitalSignal = new TaskCompletionSource<bool>();
                return digitalSignal.Task;
            }
            
        }

        /// <summary>
        /// Make the pin a digital input
        /// </summary>
        public Task MakeDigitalIn()
        {
            return portExpander.OutputModeChanged(this);
        }

        /// <summary>
        /// Toggle the pin's output value
        /// </summary>
        public async Task ToggleOutputAsync()
        {
            await MakeDigitalPushPullOut().ConfigureAwait(false);
            DigitalValue = !DigitalValue;
        }

        /// <summary>
        /// Make the pin a digital output
        /// </summary>
        public Task MakeDigitalPushPullOut()
        {
            return portExpander.OutputModeChanged(this);
        }
    }
}
