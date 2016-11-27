using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    public class Led
    {
        public LedDriver Driver { get { return driver; } }
        public Led(LedDriver driver)
        {
            this.driver = driver;
        }
        public int Channel { get; set; }
        private double brightness = 1.0;
        public double Brightness
        {
            get { return brightness; }
            set
            {
                if (brightness == value) return;
                if (!HasBrightnessControl) return;
                brightness = value;
                driver.LedBrightnessChanged(this);
            }
        }

        public bool HasBrightnessControl { get; set; }


        private bool state = false;

        private LedDriver driver;

        public bool State
        {
            get { return state; }
            set
            {
                if (state == value) return;
                state = value;
                driver.LedStateChanged(this);
            }
        }
    }
}
