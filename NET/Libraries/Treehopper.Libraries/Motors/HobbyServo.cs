using System;
using Treehopper;
using Treehopper.Utilities;

namespace Treehopper.Libraries.Motors
{
    /// <summary>
    /// The HobbyServo library is used to control small DC servos that use pulse-width control.
    /// </summary>
    public class HobbyServo
    {
        Pwm Pwm;

        /// <summary>
        /// Construct a hobby servo motor
        /// </summary>
        /// <param name="pwm">The PWM module to use</param>
        /// <param name="minPulseWidth">The minimum pulse width, in microseconds, corresponding to 0-degree angle</param>
        /// <param name="maxPulseWidth">The maximum pulse width, in microseconds, corresponding to 180-degree angle</param>
        public HobbyServo(Pwm pwm, double minPulseWidth = 500, double maxPulseWidth = 2500)
        {
            Pwm = pwm;
            pwm.Enabled = true;

            MinPulseWidth = minPulseWidth;
            MaxPulseWidth = maxPulseWidth;
            Angle = 90;
        }

        private double minPulseWidth;

        /// <summary>
        /// Gets or sets the minimum pulse width for the motor
        /// </summary>
        public double MinPulseWidth
        {
            get
            {
                return minPulseWidth;
            }
            set
            {
                if (minPulseWidth.CloseTo(value)) return;
                minPulseWidth = value;
                Angle = Angle; // force angle calculations to be updated
            }
        }

        private double maxPulseWidth;

        /// <summary>
        /// Gets or sets the max pulse width of the motor
        /// </summary>
        public double MaxPulseWidth
        {
            get
            {
                return maxPulseWidth;
            }
            set
            {
                if (maxPulseWidth.CloseTo(value)) return;
                maxPulseWidth = value;
                Angle = Angle; // force angle calculations to be updated
            }
        }

        double angle;

        /// <summary>
        /// Gets or sets the angle of the servo
        /// </summary>
        public double Angle
        {
            get { return angle; }
            set
            {
                if (angle.CloseTo(value)) return;
                angle = value;

                if (angle < 0 || angle > 180)
                {
                    throw new Exception("Angle must be between 0 and 180 degrees");
                }

                Pwm.PulseWidth = Numbers.Map(angle, 0, 180, MinPulseWidth, MaxPulseWidth);
            }
        }
    }
}
