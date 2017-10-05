using System;
using System.Collections.Specialized;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Treehopper.Uwp;

namespace Treehopper.Mvvm.ViewModels
{
    public class SelectorViewModel : SelectorViewModelBase
    {
        public SelectorViewModel(IConnectionService service) : base(service)
        {
            Window.Current.Closed += WindowClosed;
        }

        public SelectorViewModel() : this(ConnectionService.Instance)
        {
        }

        private void WindowClosed(object sender, CoreWindowEventArgs e)
        {
            WindowClosing.Execute(this);
        }

        protected override async void Boards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var board in Boards)
                if (board.SerialNumber == AutoConnectSerialNumber)
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        () =>
                        {
                            SelectedBoard = board;
                            ConnectCommand.Execute(null);
                        });
        }
    }
}