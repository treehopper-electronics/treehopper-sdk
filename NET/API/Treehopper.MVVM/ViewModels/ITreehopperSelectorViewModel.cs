using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Treehopper.Mvvm.ViewModel
{
    public interface ITreehopperSelectorViewModel : INotifyPropertyChanged
    {
        ObservableCollection<TreehopperUsb> Boards { get; }
        bool CanChangeBoardSelection { get; set; }
        TreehopperUsb SelectedBoard { get; set; }
        RelayCommand WindowClosing { get; set; }
        RelayCommand ConnectCommand { get; set; }
        string ConnectButtonText { get; set; }
        RelayCommand CloseCommand { get; set; }
    }
}