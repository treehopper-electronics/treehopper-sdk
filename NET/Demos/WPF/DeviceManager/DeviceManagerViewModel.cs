using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Treehopper;
using Treehopper.Mvvm.ViewModel;
using Treehopper.Mvvm.Messages;

namespace DeviceManager.ViewModels
{
    public class DeviceManagerViewModel : ViewModelBase
    {
        public ISelectorViewModel Selector { get; set; }

        public bool CanEdit
        {
            get { return board != null; }
        }

        private string name = "";
        public string Name { get { return name; } set { Set(ref name, value); } }

        private string firmwareString = "";
        public string FirmwareString { get { return firmwareString; } set { Set(ref firmwareString, value); } }

        private TreehopperUsb board;
        public TreehopperUsb SelectedBoard
        {
            get { return board; }
            set {
                board = value;
                RaisePropertyChanged("Board"); 
                RaisePropertyChanged("CanEdit");
                UpdateNameCommand.RaiseCanExecuteChanged();
                UpdateSerialCommand.RaiseCanExecuteChanged();
                UpdateFirmwareFromEmbeddedImage.RaiseCanExecuteChanged();
                FirmwareString = "Current firmware: " + board.VersionString;

                if (board != null)
                    Name = board.Name;
            }
        }

        private RelayCommand updateNameCommand;

        /// <summary>
        /// Gets the UpdateNameCommand.
        /// </summary>
        public RelayCommand UpdateNameCommand
        {
            get
            {
                return updateNameCommand
                    ?? (updateNameCommand = new RelayCommand(
                    async () =>
                    {
                        await SelectedBoard.UpdateDeviceName(Name);
                        await SelectedBoard.UpdateSerialNumber(Utility.RandomString(8));
                        SelectedBoard.Reboot();
                    },
                    () => CanEdit));
            }
        }

        private RelayCommand updateSerialCommand;

        /// <summary>
        /// Gets the UpdateSerialCommand.
        /// </summary>
        public RelayCommand UpdateSerialCommand
        {
            get
            {
                return updateSerialCommand
                    ?? (updateSerialCommand = new RelayCommand(
                    async () =>
                    {
                        await SelectedBoard.UpdateSerialNumber(Utility.RandomString(8));
                        SelectedBoard.Reboot();
                    },
                    () => CanEdit));
            }
        }

        private RelayCommand updateFirmwareFromEmbeddedImage;

        private bool isUpdating = false;
        public RelayCommand UpdateFirmwareFromEmbeddedImage
        {
            get
            {
                return updateFirmwareFromEmbeddedImage ?? (updateFirmwareFromEmbeddedImage = new RelayCommand(
                    async () =>
                    {
                        isUpdating = true;
                        UpdateFirmwareFromEmbeddedImage.RaiseCanExecuteChanged();
                        Progress = 1;
                        SelectedBoard.RebootIntoBootloader();
                        Progress = 10;
                        await Task.Delay(1000);
                        Progress = 20;
                        await Task.Delay(1000);
                        Progress = 30;
                        await Task.Delay(1000);
                        Progress = 40;
                        await Task.Delay(1000);
                        Progress = 50;

                        var updater = new FirmwareUpdater(new FirmwareConnection());
                        updater.ProgressChanged += (sender, args) => { Progress = args.ProgressPercentage / 2.0 + 50.0; };
                        if(await updater.ConnectAsync())
                        {
                            await updater.LoadAsync();
                        } else
                        {
                            FirmwareString = "Could not connect";
                        }
                        isUpdating = false;
                        Progress = 0;
                        UpdateFirmwareFromEmbeddedImage.RaiseCanExecuteChanged();




                    },
                    () => SelectedBoard != null && isUpdating == false));
            }
        }


        private double progress = 0;
        public double Progress
        {
            get
            {
                return progress;
            }
            set
            {
                Set(ref progress, value);
                FirmwareString = Math.Round(progress).ToString() + "%";
            }
        }

        public DeviceManagerViewModel()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                Selector = new SelectorDesignTimeViewModel(true);
            else
                Selector = new SelectorViewModel();

            Messenger.Default.Register<BoardConnectedMessage>(this, (msg) => { SelectedBoard = msg.Board; });
        }
    }
}
