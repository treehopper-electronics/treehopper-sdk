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

        public AnalogReadViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Board = DesignTimeConnectionService.Instance.Boards[0];
                Board.CreateAnalogDemoData();
                RaisePropertyChanged("Board");
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
            Board.Connection.UpdateRate = 100; // lower to 100 kHz to prevent GUI from locking up
            foreach (Pin pin in Board.Pins)
                pin.Mode = PinMode.AnalogInput;
        }

        public void Stop()
        {
            IsRunning = false;
        }
    }
}
