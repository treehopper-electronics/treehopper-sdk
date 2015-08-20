using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MERT
{
    public class MertTrialSampleModel
    {
        public double Timestamp;
        public double Value;

        public MertTrialSampleModel()
        {
            Timestamp = 0;
            Value = 0;
        }

        public MertTrialSampleModel(double timestamp, double value)
        {
            Timestamp = timestamp;
            Value = value;
        }
    }
}
