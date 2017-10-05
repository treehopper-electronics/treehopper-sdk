using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using Treehopper;
using Treehopper.Mvvm.ViewModels;
using Treehopper.Mvvm.Messages;
using Treehopper.Utilities;
using Treehopper.Firmware;

namespace TreehopperShowcase.ViewModels
{
    public class DeviceManagerViewModel : ViewModelBase
    {
        public ISelectorViewModel Selector { get; set; }

        public bool CanEdit => board != null;

        private string name = "";
        public string Name { get { return name; } set { Set(ref name, value); } }

        private TreehopperUsb board;
        public TreehopperUsb SelectedBoard
        {
            get { return board; }
            set {
                Set(ref board, value);
                RaisePropertyChanged("CanEdit");
                UpdateNameCommand.RaiseCanExecuteChanged();
                UpdateSerialCommand.RaiseCanExecuteChanged();
                UpdateFirmwareFromEmbeddedImage.RaiseCanExecuteChanged();

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

                        var updater = FirmwareUpdater.Boards[0];
                        updater.ProgressChanged += (sender, args) => { Progress = args.ProgressPercentage / 2.0 + 50.0; };
                        await updater.LoadAsync();

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
