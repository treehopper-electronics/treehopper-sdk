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
        
        public ObservableCollection<Point> TrialSamples
        {
            get
            {
                return StateModel.TrialSamples;
            }
            set
            {
                StateModel.TrialSamples = value;
                NotifyPropertyChanged("TrialSamples");
            }
        }

        public int TrialProgressPerc
        {
            get
            {
                return StateModel.TrialProgressPerc;
            }
            set
            {
                StateModel.TrialProgressPerc = value;
                NotifyPropertyChanged("TrialProgressPerc");
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
            StateModel.TrialSamples.CollectionChanged += TrialSamples_CollectionChanged;
        }

        private void TrialSamples_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("TrialSamples");
        }
    }
}
