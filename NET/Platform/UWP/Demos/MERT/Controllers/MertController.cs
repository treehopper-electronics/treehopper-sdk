using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Treehopper;
using Windows.UI.Core;

namespace MERT
{
    public class MertController
    {

        TreehopperUsb MertDevice;

        public MertStateViewModel State { get; set; }
        public MertConfigViewModel Config { get; set; }

        public delegate void MertDeviceConnectedHandler(MertController sender);
        public event MertDeviceConnectedHandler MertDeviceConnected;

        public ICommand MakeAnAttemptCommand { get; set; }

        public MertController()
        {
            State = new MertStateViewModel();
            Config = new MertConfigViewModel();

            TreehopperUsb.BoardAdded += TreehopperUsb_BoardAdded;
            TreehopperUsb.BoardRemoved += TreehopperUsb_BoardRemoved;

            MakeAnAttemptCommand = new RelayCommand((object o) => {
                StartSampling();
            });
        }

        async void SamplingJob()
        {
            double sampledVoltage;
            double trialDurationMs = Config.TrialDurationSec * 1000;
            double timePassedMs = 0;
            int samplingDelayMs = Convert.ToInt32(1000 / Config.SamplingRateHz);
            while (timePassedMs < trialDurationMs)
            {
                State.TrialProgressPerc = Convert.ToInt32( 100*timePassedMs/ trialDurationMs);
                sampledVoltage = MertDevice.Pin3.AdcVoltage;

                Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    State.TrialSamples.Add( new Windows.Foundation.Point(timePassedMs, sampledVoltage) );
                }
                );

                

                Debug.WriteLine("Sampled voltage: " + sampledVoltage);
                await Task.Delay(TimeSpan.FromMilliseconds(samplingDelayMs));
                timePassedMs += samplingDelayMs;
            }
            State.TrialProgressPerc = 100;
        }

        public void StartSampling()
        {
            Task SamplingTask = new Task(SamplingJob);
            State.SamplingInProgress = true;
            SamplingTask.Start();
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
