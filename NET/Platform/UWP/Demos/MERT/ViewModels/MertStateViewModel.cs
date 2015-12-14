using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;

namespace MERT
{





    public class MertStateViewModel: MertBaseViewModel
    {
        MertStateModel StateModel;


        ObservableCollection<MertTrialSampleViewModel> _TrialSamples;
        public ObservableCollection<MertTrialSampleViewModel> TrialSamples
        {
            get
            {
                return _TrialSamples;
            }
            set
            {
                _TrialSamples = value;
                NotifyPropertyChanged("TrialSamples");
                NotifyPropertyChanged("MinY");
                NotifyPropertyChanged("MaxY");
                NotifyPropertyChanged("RangeY");
            }
        }

        private void TrialSamples_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("TrialSamples");
            NotifyPropertyChanged("CurrentValue");
            NotifyPropertyChanged("MinY");
            NotifyPropertyChanged("MaxY");
            NotifyPropertyChanged("RangeY");
        }

        public double RangeY
        {
            get
            {
                return MaxY - MinY;
            }
        }

        public double MinY
        {
            get
            {
                if(_TrialSamples.Count ==0)
                {
                    return 0;
                }
                double minY = _TrialSamples[0].Y;
                for (int i=1; i<_TrialSamples.Count; i++)
                {
                    double y = _TrialSamples[i].Y;
                    if(y < minY)
                    {
                        minY = y;
                    }
                }
                return minY;
            }
        }

        public double MaxY
        {
            get
            {
                if (_TrialSamples.Count == 0)
                {
                    return 0;
                }
                double maxY =_TrialSamples[0].Y;
                for (int i = 1; i < _TrialSamples.Count; i++)
                {
                    double y = _TrialSamples[i].Y;
                    if (y > maxY)
                    {
                        maxY = y;
                    }
                }
                return maxY;
            }
        }


        public String CurrentUserMessage
        {
            get
            {
                return StateModel.CurrentUserMessage;
            }
            set
            {
                StateModel.CurrentUserMessage = value;
                NotifyPropertyChanged("CurrentUserMessage");
            }
        }

        public Point CurrentValue
        {
            get
            {
                if (_TrialSamples.Count > 0)
                {
                    MertTrialSampleViewModel lastSample = _TrialSamples.Last<MertTrialSampleViewModel>();
                    return new Point(lastSample.X, lastSample.Y);
                }
                // By default return zero :)
                return new Point(0, 0);
            }
        }

        public int TrialProgressMs
        {
            get {
                return StateModel.TrialProgressMs;
            }
            set
            {
                StateModel.TrialProgressMs = value;
                NotifyPropertyChanged("TrialProgressMs");
                NotifyPropertyChanged("TrialProgressPerc");
            }
        }

        public int TrialDurationMs
        {
            get
            {
                return StateModel.TrialDurationMs;
            }
            set
            {
                StateModel.TrialDurationMs = value;
                NotifyPropertyChanged("TrialDurationMs");
            }
        }

        public double TrialProgressPerc
        {
            get
            {
                if(TrialDurationMs == 0)
                {
                    return 0;
                }
                return (100*StateModel.TrialProgressMs/StateModel.TrialDurationMs);
            }
        }

       public int TrialNum
        {
            get
            {
                return StateModel.TrialNum;
            }
            set
            {
                StateModel.TrialNum = value;
                NotifyPropertyChanged("TrialNum");
            }
        }

        public bool SamplingInProgress
        {
            get
            {
                return StateModel.SamplingInProgress;
            }
            set
            {
                StateModel.SamplingInProgress = value;
                NotifyPropertyChanged("SamplingInProgress");
            }
        }

        public Boolean DeviceConnected {
            get {
                return StateModel.DeviceConnected;
            }
            set
            {
                StateModel.DeviceConnected = value;
                NotifyPropertyChanged("DeviceConnected");
            }
        }

        public MertStateViewModel(MertStateModel StateModel)
        {
            this.StateModel = StateModel;
            init();
        }

        public MertStateViewModel()
        {
            StateModel = new MertStateModel();
            init();
        }

        protected void init()
        {
            _TrialSamples = new ObservableCollection<MertTrialSampleViewModel>();
            _TrialSamples.CollectionChanged += TrialSamples_CollectionChanged;
        }

    }
}
