using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Motors
{
    /// <summary>
    /// A motorized fader contains a DC motor, speed controller, and servo functionality to set the fader to a particular position and quickly disabling the drive to allow the user to easily move the fader.
    /// </summary>
    public class MotorizedFader : AnalogFeedbackServo
    {
        /// <summary>
        /// Construct a motorized fader from an analog-in pin and a speed controller
        /// </summary>
        /// <param name="analogIn">The pin to use for analog-in</param>
        /// <param name="Controller">The controller to use to drive the fader</param>
        public MotorizedFader(Pin analogIn, MotorSpeedController Controller) : base(analogIn, Controller)
        {

        }

        /// <summary>
        /// Move the fader to a goal position, then disable the controller to allow easy movement
        /// </summary>
        /// <param name="position">The goal position, from 0.0 to 1.0</param>
        public void RecallPosition(double position)
        {
            GoalPosition = position;
            Enabled = true;

        }
    }
}
