using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

using Treehopper.Mvvm.Messages;


namespace Treehopper.Mvvm.ViewModel
{
    public abstract class SelectorViewModelBase : ViewModelBase, ISelectorViewModel
    {
        private IConnectionService connectionService;

        /// <summary>
        /// Bind to this property to get an updated list of boards
        /// </summary>
        public ObservableCollection<TreehopperUsb> Boards => connectionService.Boards;

        /// <summary>
        /// Bind the IsEnabled property of your control to this property to prevent changing the selected board once it's connected.
        /// </summary>
        public bool CanChangeBoardSelection { get; set; }
        /// <summary>
        /// Bind this property to the Selected property of your control to it.
        /// </summary>
        public TreehopperUsb SelectedBoard
        {
            get
            {
                return selectedBoard;
            }
            set
            {
                if (selectedBoard == value)
                    return;

                selectedBoard = value;
                RaisePropertyChanged("SelectedBoard");
                if (selectedBoard != null)
                    ConnectCommand.RaiseCanExecuteChanged();


                else
                {
                    Disconnect();
                }
            }
        }


        private TreehopperUsb selectedBoard;

        public RelayCommand WindowClosing { get; set; }

        /// <summary>
        /// Bind this command to your "Connect" button.
        /// </summary>
        public RelayCommand ConnectCommand { get; set; }
        /// <summary>
        /// Bind this text string to your "Connect" button's label to have it automatically change between "Connect" and "Disconnect" when appropriate.
        /// </summary>
        public string ConnectButtonText { get; set; }

        /// <summary>
        /// Call this to close and clean up
        /// </summary>
        public RelayCommand CloseCommand { get; set; }

        bool isConnected = false;

        public string AutoConnectSerialNumber { get; set; }

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

        private void WindowClosingExecute()
        {

        }

        // If the collection changed, we may have lost our board
        protected abstract void Boards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e);


        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseCommandExecute();
            if (SelectedBoard != null)
                SelectedBoard.Dispose();
        }

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
            return (SelectedBoard != null);
        }

        private void ConnectCommandExecute()
        {
            if (isConnected)
            {
                Disconnect();
            }
            else
            {
                Connect().ConfigureAwait(false);
            }
        }

        private async Task Connect()
        {
            isConnected = true;
            ConnectButtonText = "Disconnect";
            RaisePropertyChanged("ConnectButtonText");
            CanChangeBoardSelection = false;
            RaisePropertyChanged("CanChangeBoardSelection");
            await SelectedBoard.ConnectAsync();
            Messenger.Default.Send(new BoardConnectedMessage() { Board = SelectedBoard });
        }

        private void Disconnect()
        {
            isConnected = false;
            ConnectButtonText = "Connect";
            RaisePropertyChanged("ConnectButtonText");

            CanChangeBoardSelection = true;
            RaisePropertyChanged("CanChangeBoardSelection");
            if (SelectedBoard != null)
            {
                SelectedBoard.Disconnect();
            }

            Messenger.Default.Send(new BoardDisconnectedMessage() { Board = SelectedBoard });
        }
    }
}
