using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface.ShiftRegister
{
    public class ShiftOutPin : IDigitalOutPin
    {
        public ShiftOutPin(ShiftOut controller, int pinNumber)
        {
            PinNumber = pinNumber;
            this.controller = controller;
        }


        bool digitalValue;

        private ShiftOut controller;

        public int PinNumber { get; protected set; }
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

        public void MakeDigitalPushPullOut()
        {
            // nothing to do here; all pins are always outputs
        }

        public void ToggleOutput()
        {
            DigitalValue = !DigitalValue;
        }
    }
}
