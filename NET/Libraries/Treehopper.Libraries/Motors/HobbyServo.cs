﻿using System;
using Treehopper;

namespace Treehopper.Libraries.Motors
{
    /// <summary>
    /// The HobbyServo library is used to control small DC servos that use pulse-width control.
    /// </summary>
    public class HobbyServo
    {
        SoftPwm Pwm;

        public HobbyServo(Pin pin, double minPulseWidth = 500, double maxPulseWidth = 2500)
        {
            this.Pwm = pin.SoftPwm;

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

        private bool enabled;

        public bool Enabled
        {
            get { 
                return enabled; 
            }
            set { 
                enabled = value;
                Pwm.Enabled = value;
            }
        }

        double angle;

        public double Angle
        {
            get { return angle; }
            set
            {
                angle = value;
                if(Enabled)
                {
                    if (angle < 0 || angle > 180)
                    {
                        throw new Exception("Angle must be between 0 and 180 degrees");
                    }

                    Pwm.PulseWidth = Utilities.Map(angle, 0, 180, MinPulseWidth, MaxPulseWidth);
                }
            }
        }
    }
}
