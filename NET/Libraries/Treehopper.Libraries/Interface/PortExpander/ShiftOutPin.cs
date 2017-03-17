using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface.PortExpander
{
    /// <summary>
    /// A digital output pin part of a shift register output
    /// </summary>
    public class ShiftOutPin : DigitalOutPin
    {
        internal ShiftOutPin(ShiftOut controller, int pinNumber)
        {
            BitNumber = pinNumber;
            this.controller = controller;
        }


        bool digitalValue;

        private ShiftOut controller;

        /// <summary>
        /// The bit number, 0-n, of the n-bit output.
        /// </summary>
        public int BitNumber { get; protected set; }

        /// <summary>
        /// Gets or sets the digital value of the pin.
        /// </summary>
        /// <remarks>
        /// Since this is an output-only pin, getting its value will return the current state of the output.
        /// </remarks>
        public bool DigitalValue
        {
            get { return digitalValue; }
            set
            {
                if (digitalValue == value) return;

                digitalValue = value;
                controller.UpdateOutput(this);
            }
        }

        /// <summary>
        /// Make the pin a push-pull output. This function has no effect, since all ShiftOut pins are always push-pull outputs.
        /// </summary>
        public async Task MakeDigitalPushPullOut()
        {
            // nothing to do here; all pins are always outputs
        }

        /// <summary>
        /// Toggle the output
        /// </summary>
        public async Task ToggleOutputAsync()
        {
            DigitalValue = !DigitalValue;
        }
    }
}
