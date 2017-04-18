using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Displays
{
    /// <summary>
    /// An LED fader effect
    /// </summary>
    /// <remarks>
    /// <para>
    /// To fade an LED, construct an instance of this class, then set <see cref="Led" /> to the LED you wish to fade. Set the <see cref="Duration" /> of the fade, and then call <see cref="RunAsync(double, double)"/> to actually run the fade. You can switch the fader to use a different LED at any time — there's no need to construct multiple instances of this class if only one LED needs to be faded at any given time.
    /// </para>
    /// <para>
    /// The fade is performed using a linear interpolation; since <see cref="Led.Brightness"/> is linearly perceptual, this will create a linear change in brightness during the course of the fade.
    /// </para>
    /// </remarks>
    public class LedFadeAnimation
    {
        /// <summary>
        /// Create a new LED fade animation
        /// </summary>
        /// <param name="usePrecisionTimer">Whether to use the (CPU-heavy) precision timer</param>
        public LedFadeAnimation(bool usePrecisionTimer = false)
        {
            this.usePrecisionTimer = usePrecisionTimer;
        }

        readonly Stopwatch sw = new Stopwatch();
        private readonly bool usePrecisionTimer;

        /// <summary>
        /// Gets or sets the LED to fade.
        /// </summary>
        public Led Led { get; set; }

        /// <summary>
        /// Gets or sets the duration of the fade, in milliseconds.
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        /// Run the animation
        /// </summary>
        /// <param name="from">The starting value</param>
        /// <param name="to">The finished value</param>
        /// <returns>An awaitable task</returns>
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
