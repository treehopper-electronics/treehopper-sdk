using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Treehopper;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace MERT
{
    public class MertViewModel: MertBaseViewModel
    {

        TreehopperUsb MertDevice;

        public MertStateViewModel State { get; set; }

        MertConfigViewModel _Config;
        public MertConfigViewModel Config
        {
            get { return _Config;  }
            set {
                _Config = value;
                _Config.PropertyChanged += Config_PropertyChanged;
        }
    }

        public delegate void MertDeviceConnectedHandler(MertViewModel sender);
        public event MertDeviceConnectedHandler MertDeviceConnected;

        public ICommand MakeAnAttemptCommand { get; set; }

        DispatcherTimer dispatcherTimer;

        public MertViewModel()
        {
            State = new MertStateViewModel();
            Config = new MertConfigViewModel();

            TreehopperUsb.BoardAdded += TreehopperUsb_BoardAdded;
            TreehopperUsb.BoardRemoved += TreehopperUsb_BoardRemoved;

            MakeAnAttemptCommand = new RelayCommand((object o) => {
                StartTrial();
            });

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(1000 / _Config.SamplingRateHz);
            dispatcherTimer.Tick += DispatcherTimer_Tick;

        }

        private void Config_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "SamplingRateHz")
            {
                dispatcherTimer.Interval = TimeSpan.FromMilliseconds(1000/_Config.SamplingRateHz);
            }
        }

        async void StartTrial()
        {
            for(int i=Config.WarmupTimeSec; i>0; i--)
            {
                State.CurrentUserMessage = "Trial starts in " + i + "...";
                await Task.Delay(TimeSpan.FromSeconds(1));  
            }
            State.CurrentUserMessage = "";
            StartSampling();
        }

        void StartSampling()
        {
            State.TrialProgressMs = 0;
            State.TrialDurationMs = Config.TrialDurationSec * 1000;
            State.TrialSamples.Clear();
            State.SamplingInProgress = true;
            dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object sender, object e)
        {
            double sampledVoltage = MertDevice.Pin3.AdcVoltage;




            // Create a model
            MertTrialSampleModel newSample = new MertTrialSampleModel(State.TrialProgressMs, sampledVoltage);


            State.TrialProgressMs += Config.SamplingIntervalMs;

            if (State.TrialProgressMs >= State.TrialDurationMs)
            {
                Debug.WriteLine("Trial is done!");
                dispatcherTimer.Stop();
                State.SamplingInProgress = false;
            }

            State.TrialSamples.Add(new MertTrialSampleViewModel(Config, newSample));
        }

        private void TreehopperUsb_BoardRemoved(TreehopperUsb BoardRemoved)
        {
            State.DeviceConnected = false;
            Debug.WriteLine("MertDevice disconnected");
        }

        private void TreehopperUsb_BoardAdded(TreehopperUsb BoardAdded)
        {
            MertDevice = BoardAdded;
            MertDevice.Open();

            MertDevice.Pin3.ReferenceLevel = AdcReferenceLevel.VREF_1V65;
            MertDevice.Pin3.MakeAnalogInput();

            Debug.WriteLine("MertDevice connected");
            State.DeviceConnected = true;
            if (MertDeviceConnected != null)
            {
                MertDeviceConnected(this);
            }
        }
    }
}
