using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Motors
{
    public class HBridge : ISpeedController
    {
        IPwm enablePwm;
        IDigitalOutPin enable;
        IDigitalOutPin A;
        IDigitalOutPin B;       
        public HBridge(IDigitalOutPin A, IDigitalOutPin B, IPwm Enable)
        {
            Enable.Enabled = true;
            Enable.DutyCycle = 0;
            Enable.Enabled = true;
            A.MakeDigitalPushPullOut();
            B.MakeDigitalPushPullOut();

            enablePwm = Enable;
            this.A = A;
            this.B = B;

        }

        public HBridge(IDigitalOutPin A, IDigitalOutPin B, IDigitalOutPin Enable)
        {
            Enable.DigitalValue = false;
            Enable.MakeDigitalPushPullOut();
            A.MakeDigitalPushPullOut();
            B.MakeDigitalPushPullOut();

            enable = Enable;
            this.A = A;
            this.B = B;
        }
        private bool enabled;
        public bool Enabled
        {
            get
            {
                return enabled;
            }

            set
            {
                if (enabled == value) return;
                enabled = value;
                if (enabled == false)
                    setEnabled(0);
            }
        }

        private void setEnabled(double value)
        {
            if (enablePwm != null)
            {
                enablePwm.DutyCycle = value;
            } else
            {
                if (value > 0.5)
                    enable.DigitalValue = true;
                else
                    enable.DigitalValue = false;
            }
        }
        public bool BrakeOnZeroSpeed { get; set; }

        public double speed;
        public double Speed
        {
            get
            {
                return speed;
            }

            set
            {
                if (value < -1.0 || value > 1.0)
                    throw new ArgumentOutOfRangeException("Speed must be between -1.0 and 1.0");
                speed = value;

                if (speed > -0.01 && speed < 0.01) // brake
                {
                    if(BrakeOnZeroSpeed)
                    {
                        setEnabled(1.0);
                        A.DigitalValue = false;
                        B.DigitalValue = false;
                    } else
                    {
                        setEnabled(0.0);
                    }
                } else if (speed > 0)
                {
                    A.DigitalValue = true;
                    B.DigitalValue = false;
                    setEnabled(Math.Abs(speed));
                } else
                {
                    A.DigitalValue = false;
                    B.DigitalValue = true;
                    setEnabled(Math.Abs(speed));
                }
            }
        }
    }
}
