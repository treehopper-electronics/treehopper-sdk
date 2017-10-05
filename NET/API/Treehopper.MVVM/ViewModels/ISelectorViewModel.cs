using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight.Command;

namespace Treehopper.Mvvm.ViewModels
{
    /// <summary>
    ///     A delegate called when the selected board changes
    /// </summary>
    /// <param name="sender">The caller</param>
    /// <param name="e">The new selected board</param>
    public delegate void SelectedBoardChangedEventHandler(object sender, SelectedBoardChangedEventArgs e);

    /// <summary>
    ///     A delegate called when the selected board has connected
    /// </summary>
    /// <param name="sender">The caller</param>
    /// <param name="e">The connected board</param>
    public delegate void BoardConnectedEventHandler(object sender, BoardConnectedEventArgs e);

    /// <summary>
    ///     A delegate called when the selected board has disconnected
    /// </summary>
    /// <param name="sender">The caller</param>
    /// <param name="e">The disconnected board</param>
    public delegate void BoardDisconnectedEventHandler(object sender, BoardDisconnectedEventArgs e);

    /// <summary>
    ///     Base interface for the selector view model
    /// </summary>
    public interface ISelectorViewModel : INotifyPropertyChanged
    {
        /// <summary>
        ///     Get the collection of boards
        /// </summary>
        ObservableCollection<TreehopperUsb> Boards { get; }

        /// <summary>
        ///     Whether the board selection can be changed
        /// </summary>
        bool CanChangeBoardSelection { get; set; }

        /// <summary>
        ///     The currently selected board
        /// </summary>
        TreehopperUsb SelectedBoard { get; set; }

        /// <summary>
        ///     Occurs when the window closes
        /// </summary>
        RelayCommand WindowClosing { get; set; }

        /// <summary>
        ///     Occurs when the connection occurs
        /// </summary>
        RelayCommand ConnectCommand { get; set; }

        /// <summary>
        ///     Connect button text
        /// </summary>
        string ConnectButtonText { get; set; }

        /// <summary>
        ///     Close command
        /// </summary>
        RelayCommand CloseCommand { get; set; }

        /// <summary>
        ///     Board selection changed
        /// </summary>
        event SelectedBoardChangedEventHandler OnSelectedBoardChanged;

        /// <summary>
        ///     Board connected
        /// </summary>
        event BoardConnectedEventHandler OnBoardConnected;

        /// <summary>
        ///     Board disconnected
        /// </summary>
        event BoardDisconnectedEventHandler OnBoardDisconnected;
    }
}