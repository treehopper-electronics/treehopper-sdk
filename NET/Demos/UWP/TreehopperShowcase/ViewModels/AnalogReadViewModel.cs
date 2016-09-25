using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Treehopper;
using Treehopper.Mvvm.Messages;
using System.Diagnostics;
using Treehopper.Mvvm.ViewModel;

namespace TreehopperShowcase.ViewModels
{
    public class AnalogReadViewModel : Mvvm.ViewModelBase
    {
        public TreehopperUsb Board { get; set; }

        public ISelectorViewModel Selector { get; set; }
        public AnalogReadViewModel()
        {

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

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Selector = new SelectorDesignTimeViewModel(true, DesignTimeTestData.Analog);
            } else
            {
                Selector = new SelectorViewModel();
            }

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
