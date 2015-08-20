using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MERT
{
    public class MertConfigModel
    {
        public int NumTrials;
        public int TrialDurationSec;
        public double SamplingRateHz;


        public static int DefaultNumTrials = 3;
        public static int DefaultTrialDurationSec = 5;
        public static double DefaultSamplingRateHz = 10;

        public MertConfigModel()
        {
            NumTrials = DefaultNumTrials;
            TrialDurationSec = DefaultTrialDurationSec;
            SamplingRateHz = DefaultSamplingRateHz;
        }
    }
}
