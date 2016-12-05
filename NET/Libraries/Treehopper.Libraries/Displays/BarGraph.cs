using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    public class BarGraph
    {
        private IList<Led> leds;
        private Collection<ILedDriver> drivers = new Collection<ILedDriver>();
        public BarGraph(IList<Led> Leds)
        {
            this.leds = Leds;
            foreach (var led in Leds)
            {
                if (!drivers.Contains(led.Driver))
                    drivers.Add(led.Driver);
            }

            foreach(var driver in drivers)
            {
                driver.AutoFlush = false; // disable autoflush to speed things up
            }
        }

        private double val = 0;
        public double Value
        {
            get { return val; }
            set
            {
                if (val == value) return;
                if (val < 0 || val > 1)
                    throw new ArgumentOutOfRangeException("Value must be between 0 and 1");

                val = value;

                Flush();
            }
        }

        private void Flush()
        {
            int number = (int)Math.Round(val * leds.Count);
            for(int i=0;i<leds.Count;i++)
            {
                if ((fill & i < number) || i == (number - 1))
                    leds[i].State = true;
                else
                    leds[i].State = false;
            }
            foreach(var driver in drivers)
            {
                if (!driver.AutoFlush)
                    driver.Flush().Wait();
            }
        }

        private bool fill = true;
        public bool Fill
        {
            get { return fill; }
            set
            {
                if (fill == value) return;
                fill = value;

                Flush();
            }
        }
    }
}
