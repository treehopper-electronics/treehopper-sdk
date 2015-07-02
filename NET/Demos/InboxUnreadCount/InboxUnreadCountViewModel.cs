using System;
using Treehopper;
using Treehopper.Libraries; // Provides SevenSegSPI
using Treehopper.WPF.Message;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using AE.Net.Mail;
using System.Windows.Controls;

namespace InboxUnreadCount
{
    public class InboxUnreadCountViewModel : ViewModelBase
    {
        TreehopperBoard board;
        SevenSegSpi display;
        System.Timers.Timer timer;
        public InboxUnreadCountViewModel()
        {
            // Register a message handler so as soon as we're connected we setup the app
            Messenger.Default.Register<BoardConnectedMessage>(this, (response) => SetupApp(response.Board));

            // and when they disconnect, we should stop the app
            Messenger.Default.Register<BoardDisconnectedMessage>(this, (response) => StopRunning());

            // Initialize our SavePasswordCommand relay agent.
            StartAppCommand = new RelayCommand<PasswordBox>(i => StartAppExecute(i), CanStartApp);

            StartAppBtnName = "Start App";

            // Initialize the timer events. We won't start the timer until a board is connected.
            timer = new System.Timers.Timer(60000);
            timer.Elapsed += timer_Elapsed;
        }

        // This is where all the magic happens.
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            StatusBarText = "Connecting to server...";
            try
            {
                ImapClient ic = new ImapClient(ServerName, Username, Password, AuthMethods.Login, 993, true);
                ic.SelectMailbox("INBOX");
                Lazy<MailMessage>[] messages = ic.SearchMessages(SearchCondition.Unseen(), true, false);
                display.printNumber(messages.Length);
                StatusBarText = "Number of unread messages: " + messages.Length;
                ic.Disconnect();
                ic.Dispose();
            }
            catch (Exception ex)
            {
                StatusBarText = ex.Message;
                StopRunning();
            }
        }

        private bool appRunning;

        public bool AppRunning
        {
            get { return appRunning; }
            set { appRunning = value; }
        }
        

        // These are public properties the UI will bind to.
        private string statusBarText;
        public string StatusBarText
        {
            get { return statusBarText; }
            set { statusBarText = value; RaisePropertyChanged("StatusBarText");  }
        }

        private string username;

        public string Username
        {
            get { return username; }
            set { username = value; StartAppCommand.RaiseCanExecuteChanged(); }
        }

        

        public string Password { get; set; }

        private string serverName;

        public string ServerName
        {
            get { return serverName; }
            set { serverName = value; StartAppCommand.RaiseCanExecuteChanged(); }
        }


        private string startAppBtnName;
        public string StartAppBtnName
        {
            get { return startAppBtnName; }
            set { startAppBtnName = value; RaisePropertyChanged("StartAppBtnName"); }
        }

        private bool isConnected;

        public bool IsConnected
        {
            get { return isConnected; }
            set { isConnected = value; RaisePropertyChanged("IsConnected"); } //  StartAppCommand.RaiseCanExecuteChanged();
        }



        // We can't directly bind to the Password box (for stupid reasons), so we have to have a "Save" command which retrieves the password directly
        public RelayCommand<PasswordBox> StartAppCommand { get; set; }


        private bool CanStartApp(PasswordBox arg)
        {
            return IsConnected && (ServerName.Length > 0) && (Username.Length > 0);
        }

        // When the user hits "Store", this will retrieve the password
        private void StartAppExecute(PasswordBox parameter)
        {
            if(!AppRunning)
            {
                this.Password = parameter.Password;
                StartRunning();
            }
            else
            {
                StopRunning();
            }
                
        }

        private void SetupApp(TreehopperBoard treehopperBoard)
        {
            board = treehopperBoard;
            display = new SevenSegSpi(board.SPI, board.Pin1);
            IsConnected = true;
        }

        private void StartRunning()
        {
            if (!IsConnected) return;

            StartAppBtnName = "Stop App";
            // Update it immediately, and then start the timer.
            timer_Elapsed(null, null);
            timer.Start();
            AppRunning = true;
        }

        private void StopRunning()
        {
            if (!IsConnected) return;

            StartAppBtnName = "Start App";
            timer.Stop();
            AppRunning = false;
        }
    }
}
