using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper
{
    public class DesignTimeConnectionService : IConnectionService
    {
        private static readonly IConnectionService instance = new DesignTimeConnectionService();
        public static IConnectionService Instance
        {
            get { return instance; }
        }

        public DesignTimeConnectionService()
        {
            Boards = new ObservableCollection<TreehopperUsb>();
            Boards.Add(new TreehopperUsb(new DesignTimeConnection()));
            Boards.Add(new TreehopperUsb(new DesignTimeConnection()));
            Boards.Add(new TreehopperUsb(new DesignTimeConnection()));
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Boards"));
        }

        public ObservableCollection<TreehopperUsb> Boards { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task<TreehopperUsb> First()
        {
            return Boards[0];
        }
    }
}
