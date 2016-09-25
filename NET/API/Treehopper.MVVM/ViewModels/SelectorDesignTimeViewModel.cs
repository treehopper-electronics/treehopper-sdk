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
    public class SelectorDesignTimeViewModel : SelectorViewModelBase
    {
        public SelectorDesignTimeViewModel(bool selectBoard = true, DesignTimeTestData testData = DesignTimeTestData.None) : base(new DesignTimeConnectionService())
        {
            if(selectBoard)
            {
                SelectedBoard = Boards[0];
                ConnectCommand.Execute(this);
            }

            switch(testData)
            {
                case DesignTimeTestData.Analog:
                    SelectedBoard.GenerateAnalogDemoData();
                    break;
                case DesignTimeTestData.Digital:
                    SelectedBoard.GenerateDigitalTestData();
                    break;
                    
            }
                
        }

        protected override void Boards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            
        }
    }

    public enum DesignTimeTestData
    {
        None,
        Analog,
        Digital
    }
}
