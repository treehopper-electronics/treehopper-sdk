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
        public int TrialProgressPerc;
        public int TrialNum;
        public ObservableCollection<Point> TrialSamples;

        public MertStateModel()
        {
            DeviceConnected = false;
            SamplingInProgress = false;
            TrialProgressPerc = 0;
            TrialNum = 0;
            TrialSamples = new ObservableCollection<Point>();
        }
    }
}
