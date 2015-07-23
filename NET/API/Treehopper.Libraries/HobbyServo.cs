using System;

namespace Treehopper.Libraries
{
    /// <summary>
    /// The HobbyServo library is used to control small DC servos that use pulse-width control.
    /// </summary>
    public class HobbyServo
    {
        SoftPwm Pwm;
        TreehopperUSB Board;

        public HobbyServo(SoftPwm pwm, double minPulseWidth = 0.8, double maxPulseWidth = 2.8)
        {
            this.Pwm = pwm;
            this.Pwm.Period = 25;
            MinPulseWidth = minPulseWidth;
            MaxPulseWidth = maxPulseWidth;
            Angle = 90;
        }

        private double minPulseWidth;

        public double MinPulseWidth
        {
            get
            {
                return minPulseWidth;
            }
            set
            {
                minPulseWidth = value;
                Angle = Angle; // force angle calculations to be updated
            }
        }

        private double maxPulseWidth;

        public double MaxPulseWidth
        {
            get
            {
                return maxPulseWidth;
            }
            set
            {
                maxPulseWidth = value;
                Angle = Angle; // force angle calculations to be updated
            }
        }

        private bool isEnabled;

        public bool IsEnabled
        {
            get { 
                return isEnabled; 
            }
            set { 
                isEnabled = value;
                Pwm.IsEnabled = value;
            }
        }

        double angle;

        public double Angle
        {
            get { return angle; }
            set
            {
                angle = value;
                if(IsEnabled)
                {
                    if (angle < 0 || angle > 180)
                    {
                        throw new Exception("Angle must be between 0 and 180 degrees");
                    }

                    double width = Utilities.Map(angle, 0, 180, MinPulseWidth, MaxPulseWidth);
                    Pwm.PulseWidth = width;
                }
            }
        }
    }
}
