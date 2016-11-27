using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Motors
{
    public class AnalogFeedbackServo : IDisposable
    {
        protected Pin analogIn;
        protected ISpeedController controller;

        public AnalogFeedbackServo(Pin analogIn, ISpeedController Controller) 
        {
            this.analogIn = analogIn;
            this.controller = Controller;
            analogIn.Mode = PinMode.AnalogInput;
            isRunning = true;
            controlLoopTask = new Task(async() =>
            {
                while (isRunning)
                {
                    var oldPosition = ActualPosition;
                    var newPosition = analogIn.AnalogValue;
                    ActualPosition = newPosition;

                    var error = GoalPosition - ActualPosition;

                    if (Math.Abs(error) > ErrorThreshold)
                        controller.Speed = Utilities.Constrain(K * error, -1.0, 1.0);
                    else
                    {
                        controller.Speed = 0;
                        //Debug.WriteLine("goal achieved");
                    }
                    await Task.Delay(10);
                }
            });
            controlLoopTask.Start();
        }

        private bool isRunning = false;

        private Task controlLoopTask;

        public double K { get; set; } = 2;
        public double GoalPosition { get; set; }
        public double ActualPosition { get; private set; }

        public double ErrorThreshold = 0.01;

        public bool Enabled
        {
            get { return controller.Enabled; }
            set { controller.Enabled = value; }
        }

        public void Dispose()
        {
            isRunning = false;
            controlLoopTask.Wait();
        }
    }
}
