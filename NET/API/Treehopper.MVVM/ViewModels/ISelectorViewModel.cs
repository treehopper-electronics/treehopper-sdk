using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Treehopper.Mvvm.ViewModel
{
    public delegate void SelectedBoardChangedEventHandler(object sender, SelectedBoardChangedEventArgs e);
    public delegate void BoardConnectedEventHandler(object sender, BoardConnectedEventArgs e);
    public delegate void BoardDisconnectedEventHandler(object sender, BoardDisconnectedEventArgs e);
    public interface ISelectorViewModel : INotifyPropertyChanged
    {
        ObservableCollection<TreehopperUsb> Boards { get; }
        bool CanChangeBoardSelection { get; set; }
        TreehopperUsb SelectedBoard { get; set; }
        RelayCommand WindowClosing { get; set; }
        RelayCommand ConnectCommand { get; set; }
        string ConnectButtonText { get; set; }
        RelayCommand CloseCommand { get; set; }
        event SelectedBoardChangedEventHandler OnSelectedBoardChanged;
        event BoardConnectedEventHandler OnBoardConnected;
        event BoardDisconnectedEventHandler OnBoardDisconnected;
    }
}