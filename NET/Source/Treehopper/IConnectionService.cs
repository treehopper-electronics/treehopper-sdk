using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper
{
    interface IConnectionService : INotifyPropertyChanged
    {
        ObservableCollection<TreehopperUsb> Boards { get; }

        Task<TreehopperUsb> First();
    }
}
