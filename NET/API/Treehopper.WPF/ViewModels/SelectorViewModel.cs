using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

using Treehopper.Mvvm.Messages;
using System;
using System.Collections.Specialized;

namespace Treehopper.Mvvm.ViewModel
{
    public class SelectorViewModel : SelectorViewModelBase
    {
        public SelectorViewModel() : base(ConnectionService.Instance)
        {

        }

        protected override void Boards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var board in Boards)
            {
                if (board.SerialNumber == AutoConnectSerialNumber)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SelectedBoard = board;
                        ConnectCommand.Execute(null);
                    });
                }
            }
        }
    }
}