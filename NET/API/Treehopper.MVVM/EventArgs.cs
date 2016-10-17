using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Mvvm
{
    public class SelectedBoardChangedEventArgs : EventArgs
    {
        public TreehopperUsb Board { get; set; }
    }

    public class BoardConnectedEventArgs : EventArgs
    {
        public TreehopperUsb Board { get; set; }
    }

    public class BoardDisconnectedEventArgs : EventArgs
    {
        public TreehopperUsb Board { get; set; }
    }
}
