using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface
{
    public enum PinMode
    {
        DigitalOutput,
        DigitalInput
    }
    public class PortExpanderPin : DigitalIOPin
    {
        private int pinNumber;
        private PortExpander portExpander;
        private PinMode mode;
        public PinMode Mode
        {
            get { return mode; }
            set
            {
                if (mode == value) return;
                mode = value;
                portExpander.OutputModeChanged(this);
            }
        }

        private bool digitalValue;
        public bool DigitalValue
        {
            get
            {
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

        internal void UpdateInputValue(bool value)
        {
            if (digitalValue == value) return;
            digitalValue = value;
            DigitalValueChanged?.Invoke((DigitalInPin)this, new DigitalInValueChangedEventArgs(digitalValue));
            digitalSignal.TrySetResult(digitalValue);
        }
        internal PortExpanderPin(PortExpander portExpander, int pinNumber)
        {
            this.portExpander = portExpander;
            this.pinNumber = pinNumber;
        }

        TaskCompletionSource<bool> digitalSignal = new TaskCompletionSource<bool>();

        public event OnDigitalInValueChanged DigitalValueChanged;

        public Task<bool> AwaitDigitalValueChange()
        {
            digitalSignal = new TaskCompletionSource<bool>();
            return digitalSignal.Task;
        }

        public void MakeDigitalIn()
        {
            Mode = PinMode.DigitalInput;
        }

        public void ToggleOutput()
        {
            MakeDigitalPushPullOut();
            DigitalValue = !DigitalValue;
        }

        public void MakeDigitalPushPullOut()
        {
            Mode = PinMode.DigitalOutput;
        }
    }
}
