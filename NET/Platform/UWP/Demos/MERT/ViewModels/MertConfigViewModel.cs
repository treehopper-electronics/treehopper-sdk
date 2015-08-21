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

        public int WarmupTimeSec
        {
            get
            {
                return ConfigModel.WarmupTimeSec;
            }
            set
            {
                ConfigModel.WarmupTimeSec = value;
                NotifyPropertyChanged("WarmupTimeSec");
            }
        }

        public double MaxAirFlowValue
        {
            get
            {
                return ConfigModel.MaxAirFlowValue;
            }
            set
            {
                ConfigModel.MaxAirFlowValue = value;
                NotifyPropertyChanged("MaxAirFlowValue");
            }
        }

        public double PlottingWindowWidthPx
        {
            get
            {
                return ConfigModel.PlottingWindowWidthPx;
            }
            set
            {
                ConfigModel.PlottingWindowWidthPx = value;
                NotifyPropertyChanged("PlottingWindowWidthPx");
            }
        }


        public double PlottingWindowHeightPx
        {
            get
            {
                return ConfigModel.PlottingWindowHeightPx;
            }
            set
            {
                ConfigModel.PlottingWindowHeightPx = value;
                NotifyPropertyChanged("PlottingWindowHeightPx");
            }
        }


        public double DisplayOffsetY
        {
            get
            {
                return ConfigModel.DisplayOffsetY;
            }
            set
            {
                ConfigModel.DisplayOffsetY = value;
                NotifyPropertyChanged("DisplayOffsetY");
            }
        }

        public double TargetValueScaled
        {
            get
            {
                double tvs =  ConfigModel.PlottingWindowHeightPx * (1 - ConfigModel.TargetValue / ConfigModel.MaxAirFlowValue);
                return tvs;
            }

        }

        public double TargetValue
        {
            get
            {
                return ConfigModel.TargetValue;
            }
            set
            {
                ConfigModel.TargetValue = value;
                NotifyPropertyChanged("TargetValue");
                NotifyPropertyChanged("TargetValueScaled");
            }
        }

        public double DisplayScaleFactorY
        {
            get { return ConfigModel.DisplayScaleFactorY; }

            set
            {
                ConfigModel.DisplayScaleFactorY = value;
                NotifyPropertyChanged("DisplayScaleFactorY");
            }
        }

        public double DisplayScaleFactorX
        {
            get { return ConfigModel.DisplayScaleFactorX; }

            set
            {
                ConfigModel.DisplayScaleFactorX = value;
                NotifyPropertyChanged("DisplayScaleFactorX");
            }
        }




            public MertConfigViewModel()
        {
            ConfigModel = new MertConfigModel();
        }

        public int SamplingIntervalMs
        {
            get
            {
                return Convert.ToInt32( 1000 / ConfigModel.SamplingRateHz);
            }
            set
            {
                ConfigModel.SamplingRateHz = 1000/value;
                NotifyPropertyChanged("SamplingIntervalMs");
                NotifyPropertyChanged("SamplingRateHz");
            }
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
                NotifyPropertyChanged("SamplingIntervalMs");
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
