using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Uwp;

namespace TreehopperShowcase.ViewModels
{
    public class BlinkViewModel : Mvvm.ViewModelBase
    {
        public ObservableCollection<LedBlinkerViewModel> LedPanels { get; set; }
        public BlinkViewModel()
        {
            LedPanels = new ObservableCollection<LedBlinkerViewModel>();
            ConnectionService.Instance.Boards.CollectionChanged += Boards_CollectionChanged;
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                LedPanels.Add(new LedBlinkerViewModel(new TreehopperUsb(new DesignTimeConnection())));
                LedPanels.Add(new LedBlinkerViewModel(new TreehopperUsb(new DesignTimeConnection())));
                LedPanels.Add(new LedBlinkerViewModel(new TreehopperUsb(new DesignTimeConnection())));
                LedPanels.Add(new LedBlinkerViewModel(new TreehopperUsb(new DesignTimeConnection())));
            } else
            {
                foreach (var ledPanel in ConnectionService.Instance.Boards)
                {
                    LedPanels.Add(new LedBlinkerViewModel(ledPanel));
                }
            }

        }
        LedBlinkerViewModel selectedBoard;
        LedBlinkerViewModel SelectedBoard { get { return selectedBoard; } set { Set(ref selectedBoard, value); } }

        private void Boards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (var newBoard in e.NewItems)
                {
                    LedPanels.Add(new LedBlinkerViewModel((TreehopperUsb)newBoard));
                }
            }

            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (var deletedBoard in e.OldItems)
                {
                    LedPanels.Where(board => board.Board == deletedBoard).ToList().All(i =>
                    {
                        i.Board.Disconnect();
                        LedPanels.Remove(i);
                        return true;
                    });
                }
            }
        }
    }

    public class LedBlinkerViewModel : Mvvm.ViewModelBase
    {
        CancellationTokenSource cts;
        public LedBlinkerViewModel(TreehopperUsb board)
        {
            this.board = board;
        }



        private TreehopperUsb board;
        public TreehopperUsb Board
        {
            get
            {
                return board;
            }
            set
            {
                if (board == value)
                    return;
                Set(ref board, value);
                RaisePropertyChanged("StartStopButton");

            }
        }

        private RelayCommand startStopButton;

        /// <summary>
        /// Gets the StartStopButton.
        /// </summary>
        public RelayCommand StartStopButton
        {
            get
            {
                return startStopButton
                    ?? (startStopButton = new RelayCommand(
                    async () =>
                    {
                        IsRunning = !IsRunning;
                        if (IsRunning)
                        {
                            ButtonText = "Stop";
                            await Board.ConnectAsync();
                            cts = new CancellationTokenSource();
                            blinkingTask();
                        }
                        else
                        {
                            ButtonText = "Start";
                            if (cts != null)
                                cts.Cancel();
                        }
                    },
                    () => Board != null));
            }
        }

        public async void blinkingTask()
        {
            while (true)
            {
                if (cts.IsCancellationRequested)
                {
                    Board.Disconnect();
                    return;
                }

                Board.Led = true;
                await Task.Delay(OnTime);
                Board.Led = false;
                await Task.Delay(Period);
            }
        }

        string buttonText = "start";
        public string ButtonText { get { return buttonText; } set { Set(ref buttonText, value); } }

        int onTime = 500;
        public int OnTime { get { return onTime; } set { Set(ref onTime, value); } }

        int period = 500;
        public int Period { get { return period; } set { Set(ref period, value); } }

        bool isRunning;
        public bool IsRunning { get { return isRunning; } set { Set(ref isRunning, value); } }
    }
}
