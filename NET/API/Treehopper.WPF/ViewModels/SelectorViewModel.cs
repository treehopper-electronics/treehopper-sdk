using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using Treehopper.Desktop;

namespace Treehopper.Mvvm.ViewModel
{
    /// <summary>
    ///     WPF SelectorViewModel implementation
    /// </summary>
    public class SelectorViewModel : SelectorViewModelBase
    {
        /// <summary>
        ///     Construct a SelectorViewModel
        /// </summary>
        /// <param name="service">Connection service to use</param>
        public SelectorViewModel(IConnectionService service) : base(service)
        {
            Application.Current.MainWindow.Closing += MainWindow_Closing;
            ;
        }

        /// <summary>
        ///     Construct a SelectorViewModel using the default ConnectionService
        /// </summary>
        public SelectorViewModel() : this(ConnectionService.Instance)
        {
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            WindowClosing.Execute(this);
        }

        /// <summary>
        ///     Occurs when the board collection changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void Boards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var board in Boards)
                if (board.SerialNumber == AutoConnectSerialNumber)
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SelectedBoard = board;
                        ConnectCommand.Execute(null);
                    });
        }
    }
}