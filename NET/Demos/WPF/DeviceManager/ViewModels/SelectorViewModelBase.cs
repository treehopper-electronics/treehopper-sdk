using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Treehopper.Mvvm.Messages;

namespace Treehopper.Mvvm.ViewModels
{
    /// <summary>
    ///     Base class to implement a Selector view model
    /// </summary>
    public abstract class SelectorViewModelBase : ViewModelBase, ISelectorViewModel
    {
        private readonly IConnectionService connectionService;

        private bool isConnected;


        private TreehopperUsb selectedBoard;

        /// <summary>
        ///     Construct a SelectorViewModelBase
        /// </summary>
        /// <param name="connectionService">The connection service to use for this selector ViewModel</param>
        public SelectorViewModelBase(IConnectionService connectionService)
        {
            this.connectionService = connectionService;
            ConnectButtonText = "Connect";
            CanChangeBoardSelection = true;
            ConnectCommand = new RelayCommand(ConnectCommandExecute, ConnectCommandCanExecute);
            CloseCommand = new RelayCommand(CloseCommandExecute, CloseCommandCanExecute);
            WindowClosing = new RelayCommand(WindowClosingExecute);
            // This allows us to automatically close the device when the window closes
            //Application.Current.MainWindow.Closing += MainWindow_Closing;

            Boards.CollectionChanged += Boards_CollectionChanged;
        }

        /// <summary>
        ///     Autoconnect serial number
        /// </summary>
        public string AutoConnectSerialNumber { get; set; }

        /// <summary>
        ///     Bind to this property to get an updated list of boards
        /// </summary>
        public ObservableCollection<TreehopperUsb> Boards => connectionService.Boards;

        /// <summary>
        ///     Bind the IsEnabled property of your control to this property to prevent changing the selected board once it's
        ///     connected.
        /// </summary>
        public bool CanChangeBoardSelection { get; set; }

        /// <summary>
        ///     Bind this property to the Selected property of your control to it.
        /// </summary>
        public TreehopperUsb SelectedBoard
        {
            get { return selectedBoard; }
            set
            {
                if (selectedBoard == value)
                    return;

                selectedBoard = value;
                RaisePropertyChanged("SelectedBoard");
                OnSelectedBoardChanged?.Invoke(this, new SelectedBoardChangedEventArgs {Board = selectedBoard});
                if (selectedBoard != null)
                    ConnectCommand.RaiseCanExecuteChanged();


                else
                    Disconnect();
            }
        }

        /// <summary>
        ///     Occurs when the window closes
        /// </summary>
        public RelayCommand WindowClosing { get; set; }

        /// <summary>
        ///     Bind this command to your "Connect" button.
        /// </summary>
        public RelayCommand ConnectCommand { get; set; }

        /// <summary>
        ///     Bind this text string to your "Connect" button's label to have it automatically change between "Connect" and
        ///     "Disconnect" when appropriate.
        /// </summary>
        public string ConnectButtonText { get; set; }

        /// <summary>
        ///     Call this to close and clean up
        /// </summary>
        public RelayCommand CloseCommand { get; set; }

        /// <summary>
        ///     Event occurs when selected board changed
        /// </summary>
        public event SelectedBoardChangedEventHandler OnSelectedBoardChanged;

        /// <summary>
        ///     Occurs when a board is connected
        /// </summary>
        public event BoardConnectedEventHandler OnBoardConnected;

        /// <summary>
        ///     Occurs when a board is disconnected
        /// </summary>
        public event BoardDisconnectedEventHandler OnBoardDisconnected;

        private void WindowClosingExecute()
        {
            CloseCommand.Execute(this);
            if (SelectedBoard != null)
                SelectedBoard.Dispose();
        }

        /// <summary>
        ///     Occurs when the board collection has changed
        /// </summary>
        /// <param name="sender">The caller</param>
        /// <param name="e">The new collection</param>
        protected abstract void Boards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e);

        private bool CloseCommandCanExecute()
        {
            return isConnected;
        }

        private void CloseCommandExecute()
        {
            Debug.WriteLine("Closing...");
            if (isConnected)
            {
                isConnected = false;
                Disconnect();
            }
        }

        private bool ConnectCommandCanExecute()
        {
            return SelectedBoard != null;
        }

        private void ConnectCommandExecute()
        {
            if (isConnected)
                Disconnect();
            else
                Connect().ConfigureAwait(false);
        }

        private async Task Connect()
        {
            isConnected = true;
            ConnectButtonText = "Disconnect";
            RaisePropertyChanged("ConnectButtonText");
            CanChangeBoardSelection = false;
            RaisePropertyChanged("CanChangeBoardSelection");
            await SelectedBoard.ConnectAsync().ConfigureAwait(false);
            OnBoardConnected?.Invoke(this, new BoardConnectedEventArgs {Board = SelectedBoard});
            Messenger.Default.Send(new BoardConnectedMessage {Board = SelectedBoard});
        }

        private void Disconnect()
        {
            isConnected = false;
            ConnectButtonText = "Connect";
            RaisePropertyChanged("ConnectButtonText");

            CanChangeBoardSelection = true;
            RaisePropertyChanged("CanChangeBoardSelection");
            if (SelectedBoard != null)
                SelectedBoard.Disconnect();
            OnBoardDisconnected?.Invoke(this, new BoardDisconnectedEventArgs {Board = SelectedBoard});
            Messenger.Default.Send(new BoardDisconnectedMessage {Board = SelectedBoard});
        }
    }
}