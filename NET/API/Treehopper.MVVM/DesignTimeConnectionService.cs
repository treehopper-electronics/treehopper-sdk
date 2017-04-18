using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Treehopper
{
    /// <summary>
    /// A DesignTime connection service useful for developing GUIs
    /// </summary>
    public class DesignTimeConnectionService : IConnectionService
    {
        private static readonly IConnectionService instance = new DesignTimeConnectionService();

        /// <summary>
        /// get a static connection instance
        /// </summary>
        public static IConnectionService Instance => instance;

        /// <summary>
        /// Create a new DesignTime connection service with three connected boards
        /// </summary>
        public DesignTimeConnectionService()
        {
            Boards = new ObservableCollection<TreehopperUsb>();
            Boards.Add(new TreehopperUsb(new DesignTimeConnection()));
            Boards.Add(new TreehopperUsb(new DesignTimeConnection()));
            Boards.Add(new TreehopperUsb(new DesignTimeConnection()));
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Boards"));
        }

        /// <summary>
        /// Get a collection of boards
        /// </summary>
        public ObservableCollection<TreehopperUsb> Boards { get; set; }

        /// <summary>
        /// Fires when any property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Get the first device
        /// </summary>
        /// <returns>An awaitable task that completes when the first board is found</returns>
        public async Task<TreehopperUsb> GetFirstDeviceAsync()
        {
            return Boards[0];
        }

        /// <summary>
        /// Dispose of this connection
        /// </summary>
        public void Dispose()
        {
            
        }
    }
}
