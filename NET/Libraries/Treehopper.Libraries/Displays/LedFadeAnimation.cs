using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    public class LedFadeAnimation
    {
        public LedFadeAnimation(bool usePrecisionTimer = false)
        {
            this.usePrecisionTimer = usePrecisionTimer;
        }
        Stopwatch sw = new Stopwatch();
        private bool usePrecisionTimer;
        public Led Led { get; set; }
        public double Duration { get; set; }

        public Task RunAsync(double from = 0, double to = 1)
        {
            return Task.Run(() =>
            {
                int steps;
                if(usePrecisionTimer) // let's shoot for 10ms steps
                {
                    steps = (int)Math.Round(Duration / 10.0);
                }
                else
                { // 20ms steps
                    steps = (int)Math.Round(Duration / 20.0);
                }
                // amount to step
                double amount = (to - from) / steps;

                // actual delay
                double ms = Duration / steps;

                for(int i=0;i<steps;i++)
                {
                    Led.Brightness = from + amount * i;

                    // delay
                    if (!usePrecisionTimer)
                    {
                        Task.Delay((int)Math.Round(ms)).Wait();
                    }
                    else
                    {
                        sw.Restart();
                        while (sw.Elapsed.TotalMilliseconds < ms) ;
                        sw.Stop();
                    }
                }

                // because of rounding errors, we might not actually hit the target, so update it here
                Led.Brightness = to;


            
            });
        }
    }


}
