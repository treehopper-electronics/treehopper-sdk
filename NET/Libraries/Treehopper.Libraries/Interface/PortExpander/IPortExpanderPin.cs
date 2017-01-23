using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface.PortExpander
{
    public interface IPortExpanderPin : DigitalIOPin
    {
        PortExpanderPinMode Mode { get; set; }

        /// <summary>
        /// Gets or sets the output value of the pin when an output, or get the current value of the pin when an input
        /// </summary>
        bool DigitalValue { get; set; }

        int PinNumber { get; }

        /// <summary>
        /// Event occurs whenever a digital input value has changed
        /// </summary>
        event OnDigitalInValueChanged DigitalValueChanged;

        /// <summary>
        /// Wait for the digital input to change
        /// </summary>
        /// <returns>The new digital value (when the wait completes)</returns>
        Task<bool> AwaitDigitalValueChange();

        /// <summary>
        /// Make the pin a digital input
        /// </summary>
        void MakeDigitalIn();

        /// <summary>
        /// Toggle the pin's output value
        /// </summary>
        void ToggleOutput();

        /// <summary>
        /// Make the pin a digital output
        /// </summary>
        void MakeDigitalPushPullOut();
    }
}
