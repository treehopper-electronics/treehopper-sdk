using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using Treehopper.Mvvm.Messages;
using Treehopper;

namespace Treehopper.Mvvm.ViewModel
{
    public class TreehopperSelectorViewModel : ViewModelBase
    {
        /// <summary>
        /// Bind to this property to get an updated list of boards
        /// </summary>
        public ObservableCollection<TreehopperUsb> Boards { get { return ConnectionService.Instance.Boards; } }
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
                selectedBoard = value;
                if(selectedBoard != null)
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

        //private TreehopperManager manager;

        public TreehopperSelectorViewModel()
        {
            ConnectButtonText = "Connect";
            CanChangeBoardSelection = true;
            ConnectCommand = new RelayCommand(async () => await ConnectCommandExecute(), ConnectCommandCanExecute);
            CloseCommand = new RelayCommand(CloseCommandExecute, CloseCommandCanExecute);

            Boards.CollectionChanged += Boards_CollectionChanged;
        }

        // If the collection changed, we may have lost our board
        void Boards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        private bool CloseCommandCanExecute()
        {
            return isConnected;
        }

        private void CloseCommandExecute()
        {
            Debug.WriteLine("Closing...");
            if(isConnected)
            {
                isConnected = false;
                Disconnect();
            }
        }

        private bool ConnectCommandCanExecute()
        {
            return (SelectedBoard != null);
        }

        private async Task ConnectCommandExecute()
        {
            if (isConnected)
            {
                Disconnect();
            } else
            {
                await Connect();
            }
        }

        private async Task Connect()
        {
            isConnected = true;
            ConnectButtonText = "Disconnect";
            RaisePropertyChanged("ConnectButtonText");
            CanChangeBoardSelection = false;
            RaisePropertyChanged("CanChangeBoardSelection");
            await SelectedBoard.Connect();
            Messenger.Default.Send(new BoardConnectedMessage() { Board = SelectedBoard });
        }

        private void Disconnect()
        {
            isConnected = false;
            ConnectButtonText = "Connect";
            RaisePropertyChanged("ConnectButtonText");

            CanChangeBoardSelection = true;
            RaisePropertyChanged("CanChangeBoardSelection");
            if(SelectedBoard != null)
            {
                SelectedBoard.Disconnect();
            }
                
            Messenger.Default.Send(new BoardDisconnectedMessage() { Board = SelectedBoard });
        }
    }
}
