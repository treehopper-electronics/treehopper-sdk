using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MERT
{
    public class MertTrialSampleModel
    {

        public double TimestampMs;
        public double AirFlowValue;

        public MertTrialSampleModel()
        {
            TimestampMs = 0;
            AirFlowValue = 0;
        }

        public MertTrialSampleModel(double timestampMs, double airFlowvalue)
        {
            TimestampMs = timestampMs;
            AirFlowValue = airFlowvalue;
        }
    }
}
