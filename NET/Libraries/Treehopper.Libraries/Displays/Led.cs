using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    public class Led
    {
        public ILedDriver Driver { get; private set; }
        public Led(ILedDriver driver, int channel = 0, bool hasBrightnessControl = false)
        {
            Driver = driver;
            HasBrightnessControl = hasBrightnessControl;
            Channel = channel;
        }
        public int Channel { get; private set; }
        private double brightness = 1.0;
        public double Brightness
        {
            get { return brightness; }
            set
            {
                if (brightness == value) return;
                if (!HasBrightnessControl) return;
                brightness = value;
                Driver.LedBrightnessChanged(this);
            }
        }

        public bool HasBrightnessControl { get; private set; }


        private bool state = false;

        public bool State
        {
            get { return state; }
            set
            {
                if (state == value) return;
                state = value;
                Driver.LedStateChanged(this);
            }
        }
    }
}
