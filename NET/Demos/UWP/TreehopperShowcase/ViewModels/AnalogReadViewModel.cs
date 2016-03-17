using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Treehopper;
using Treehopper.Mvvm.Messages;
using System.Diagnostics;

namespace TreehopperShowcase.ViewModels
{
    public class AnalogReadViewModel : Mvvm.ViewModelBase
    {
        public TreehopperUsb Board { get; set; }

        public double AdcValue { get; set; }

        public AnalogReadViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Board = new TreehopperUsb(new DesignTimeConnection());
            }
                Messenger.Default.Register<BoardConnectedMessage>(this, 
                (msg) => 
                {
                    Board = msg.Board;
                    RaisePropertyChanged("Board");
                    Start();
                });

            Messenger.Default.Register<BoardDisconnectedMessage>(this, 
                (msg) => 
                {
                    Stop();
                });
        }

        bool IsRunning = false;

        public async void Start()
        {
            Board.Pin1.Mode = PinMode.AnalogInput;
            IsRunning = true;
            while(IsRunning)
            {
                AdcValue = await Board.Pin1.AwaitAdcValueChange();
                Debug.WriteLine(AdcValue);
                RaisePropertyChanged("AdcValue");
            }
        }

        public void Stop()
        {
            IsRunning = false;
        }
    }
}
