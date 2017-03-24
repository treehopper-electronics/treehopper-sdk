using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Libraries.Sensors;

namespace Treehopper.Libraries.Interface.PortExpander
{
    /// <summary>
    /// A DigitalInPin-only implementation for the C and Z buttons
    /// </summary>
    public class DigitalInPeripheralPin : DigitalIn
    {
        private IPollable parent;
        TaskCompletionSource<bool> digitalSignal = new TaskCompletionSource<bool>();
        private bool digitalValue;
        internal DigitalInPeripheralPin(IPollable parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Gets the digital state of the button. "0" for "unpressed" and "1" for "pressed"
        /// </summary>
        public bool DigitalValue
        {
            get
            {
                if (parent.AutoUpdateWhenPropertyRead) parent.Update().Wait();
                return digitalValue;
            }

            internal set
            {
                if (digitalValue == value) return;
                digitalValue = value;
                DigitalValueChanged?.Invoke((DigitalIn)this, new DigitalInValueChangedEventArgs(digitalValue));
                digitalSignal.TrySetResult(digitalValue);
            }
        }

        /// <summary>
        /// Fires when the button state changes
        /// </summary>
        public event OnDigitalInValueChanged DigitalValueChanged;

        /// <summary>
        /// Wait for the button to change state
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the parent Nunchuk's <see cref="WiiNunchuk.AutoUpdateWhenPropertyRead"/> is "true", executing this function will start a Task that will poll the Nunchuk at an update rate specified by <see cref="WiiNunchuk.AwaitPollingInterval"/> until the button's state changes. Otherwise, this will return an awaitable (empty) task that will complete when the button state changes (the user is required to call <see cref="WiiNunchuk.Update()"/> periodically).
        /// </para>
        /// </remarks>
        /// <returns>The new state of the button</returns>
        public Task<bool> AwaitDigitalValueChange()
        {
            if (parent.AutoUpdateWhenPropertyRead)
            {
                return Task.Run(async () =>
                {
                    bool oldValue = digitalValue;
                    // poll the device
                    while (digitalValue == oldValue)
                    {
                        await parent.Update().ConfigureAwait(false);
                        await Task.Delay(parent.AwaitPollingInterval);
                    }

                    return digitalValue;
                });
            }
            else
            {
                digitalSignal = new TaskCompletionSource<bool>();
                return digitalSignal.Task;
            }
        }

        /// <summary>
        /// Unused. Stub for DigitalInPin compliance.
        /// </summary>
        public void MakeDigitalIn()
        {
        }
    }
}
