using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;

namespace TreehopperShowcase.ViewModels
{
    public class DeviceManagerViewModel : Mvvm.ViewModelBase
    {
        public DeviceManagerViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                SelectedBoard = Boards[1];
            }
        }

        public ObservableCollection<TreehopperUsb> Boards { get { return ConnectionService.Instance.Boards; } }

        string newName;
        public string NewName {
            get {
                return newName;
            } set {
                Set(ref newName, value);
            }
        }

        TreehopperUsb selectedBoard;
        public TreehopperUsb SelectedBoard {
            get {
                return selectedBoard;
            } set {
                if (selectedBoard != null)
                    selectedBoard.Disconnect();
                Set(ref selectedBoard, value);
                if(selectedBoard != null)
                {
                    selectedBoard.ConnectAsync().ConfigureAwait(false);
                    NewName = selectedBoard.Name;
                    RaisePropertyChanged("UpdateName");
                    RaisePropertyChanged("GenerateSerial");
                    RaisePropertyChanged("UpdateFirmwareFromEmbeddedImage");
                }
                
            }
        }

        private RelayCommand generateSerial;

        public RelayCommand GenerateSerial
        {
            get
            {
                return generateSerial
                    ?? (generateSerial = new RelayCommand(
                    () =>
                    {
                        SelectedBoard.UpdateSerialNumber(RandomString(8));
                    },
                    () => SelectedBoard != null));
            }
        }

        private RelayCommand updateName;

        public RelayCommand UpdateName
        {
            get
            {
                return updateName
                    ?? (updateName = new RelayCommand(
                    async () =>
                    {
                        await SelectedBoard.UpdateDeviceName(newName);
                        await SelectedBoard.UpdateSerialNumber(RandomString(8));
                        SelectedBoard.Reboot();
                    },
                    () => SelectedBoard != null));
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

                        var updater = new FirmwareUpdater(new FirmwareConnection());
                        updater.ProgressChanged += (sender, args) => { Progress = args.ProgressPercentage/2.0 + 50.0; };
                        await updater.ConnectAsync();
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

        static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
