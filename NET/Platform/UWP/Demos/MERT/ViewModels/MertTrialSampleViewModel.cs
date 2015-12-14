using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace MERT
{
    public class MertTrialSampleViewModel: MertBaseViewModel
    {

        MertTrialSampleModel Model;
        MertConfigViewModel Config;

        public double X
        {
            get
            {
                //                return Model.TimestampMs;
                double x = Config.PlottingWindowWidthPx *   TimestampSec/ Config.TrialDurationSec;
                return x;
            }
        }

        public double Y
        {
            get
            {
                return Config.PlottingWindowHeightPx * (1-AirFlowValue / Config.MaxAirFlowValue);
            }
        }

        public double TimestampSec
        {
            get
            {
                return Model.TimestampMs / 1000;
            }
            set
            {
                Model.TimestampMs = value * 1000;
                NotifyPropertyChanged("TimestampMs");
                NotifyPropertyChanged("TimestampSec");
            }
        }

        public double TimestampMs
        {
            get 
            {
                return Model.TimestampMs;
            }
            set
            {
                Model.TimestampMs = value;
                NotifyPropertyChanged("TimestampMs");
                NotifyPropertyChanged("TimestampSec");
            }
        }


        public double AirFlowValue
        {
            get
            {
                return Model.AirFlowValue;
            }
            set
            {
                Model.AirFlowValue = value;
                NotifyPropertyChanged("AirFlowValue");
            }
        }

        public MertTrialSampleViewModel(MertConfigViewModel configViewModel)
        {
            Model = new MertTrialSampleModel();
            Config = configViewModel;
        }

        public MertTrialSampleViewModel(MertConfigViewModel configViewModel, Point p)
        {
            this.Model = new MertTrialSampleModel(p.X, p.Y);
            Config = configViewModel;
        }

        public MertTrialSampleViewModel(MertConfigViewModel configViewModel, MertTrialSampleModel Model)
        {
            this.Model = Model;
            Config = configViewModel;
        }
    }
}
