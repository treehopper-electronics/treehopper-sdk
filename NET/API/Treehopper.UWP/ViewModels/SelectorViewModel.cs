using Windows.UI.Core;
using Windows.UI.Xaml;
using System;
using System.Collections.Specialized;
using Treehopper.Uwp;

namespace Treehopper.Mvvm.ViewModel
{
    public class SelectorViewModel : SelectorViewModelBase
    {
        public SelectorViewModel(IConnectionService service) : base(service)
        {
            Window.Current.Closed += WindowClosed;
        }

        private void WindowClosed(object sender, CoreWindowEventArgs e)
        {
            WindowClosing.Execute(this);
        }

        public SelectorViewModel() : this(ConnectionService.Instance)
        {

        }

        protected async override void Boards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var board in Boards)
            {
                if (board.SerialNumber == AutoConnectSerialNumber)
                {
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        SelectedBoard = board;
                        ConnectCommand.Execute(null);
                    });
                }
            }
        }
    }
}
