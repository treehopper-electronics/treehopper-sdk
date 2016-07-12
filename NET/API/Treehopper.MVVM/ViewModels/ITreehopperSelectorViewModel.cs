using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;

namespace Treehopper.Mvvm.ViewModel
{
    public interface ITreehopperSelectorViewModel
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