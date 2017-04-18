using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Utilities;

namespace Treehopper.Libraries.Motors
{
    /// <summary>
    /// A servo motor controlled by an analog potentiometer
    /// </summary>
    public class AnalogFeedbackServo : IDisposable
    {
        Pin analogIn;
        readonly MotorSpeedController controller;

        /// <summary>
        /// Construct a new analog feedback servo from an analog pin and a speed controller
        /// </summary>
        /// <param name="analogIn">The analog in pin to use for position feedback</param>
        /// <param name="Controller">The speed controller to use to control the motor</param>
        public AnalogFeedbackServo(Pin analogIn, MotorSpeedController Controller) 
        {
            this.analogIn = analogIn;
            controller = Controller;
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
                        controller.Speed = Numbers.Constrain(K * error, -1.0, 1.0);
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

        private readonly Task controlLoopTask;

        /// <summary>
        /// The K value to use in the proportional control loop
        /// </summary>
        public double K { get; set; } = 2;

        /// <summary>
        /// The goal position
        /// </summary>
        public double GoalPosition { get; set; }

        /// <summary>
        /// The actual position of the motor
        /// </summary>
        public double ActualPosition { get; private set; }

        /// <summary>
        /// The error threshold
        /// </summary>
        public double ErrorThreshold = 0.01;

        /// <summary>
        /// Whether to enable or disable the servo
        /// </summary>
        public bool Enabled
        {
            get { return controller.Enabled; }
            set { controller.Enabled = value; }
        }

        /// <summary>
        /// Dispose the servo object
        /// </summary>
        public void Dispose()
        {
            isRunning = false;
            controlLoopTask.Wait();
        }
    }
}
