using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Motors
{
    public class MotorizedFader : AnalogFeedbackServo
    {
        public MotorizedFader(Pin analogIn, ISpeedController Controller) : base(analogIn, Controller)
        {

        }

        public void RecallPosition(double position)
        {
            this.GoalPosition = position;
            this.Enabled = true;

        }
    }
}
