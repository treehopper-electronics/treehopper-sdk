using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace MERT
{
    public class MertStateModel
    {
        public bool DeviceConnected;
        public bool SamplingInProgress;
        public int TrialProgressMs;
        public int TrialDurationMs;
        public int TrialNum;
        public List<MertTrialSampleModel> TrialSamples;
        public String CurrentUserMessage;

        public MertStateModel()
        {
            DeviceConnected = false;
            SamplingInProgress = false;
            TrialDurationMs = 0;
            TrialProgressMs = 0;
            TrialNum = 0;
            TrialSamples = new List<MertTrialSampleModel>();
            CurrentUserMessage = "";
        }
    }
}
