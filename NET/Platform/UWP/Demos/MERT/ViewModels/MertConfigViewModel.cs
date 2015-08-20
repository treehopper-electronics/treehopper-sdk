using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MERT
{
    public class MertConfigViewModel: MertBaseViewModel
    {

        MertConfigModel ConfigModel;

        public MertConfigViewModel(MertConfigModel ConfigModel)
        {
            this.ConfigModel = ConfigModel;
        }

        public MertConfigViewModel()
        {
            ConfigModel = new MertConfigModel();
        }

        public double SamplingRateHz
        {
            get
            {
                return ConfigModel.SamplingRateHz;
            }
            set
            {
                ConfigModel.SamplingRateHz = value;
                NotifyPropertyChanged("SamplingRateHz");
            }
        }
        public int TrialDurationSec {
            get {
                return ConfigModel.TrialDurationSec;
            }
            set
            {
                ConfigModel.TrialDurationSec = value;
                NotifyPropertyChanged("TrialDurationSec");
            }
        }

        public int NumTrials
        {
            get
            {
                return ConfigModel.NumTrials;
            }
            set
            {
                ConfigModel.NumTrials = value;
                NotifyPropertyChanged("NumTrials");
            }
        }

        
    }
}
