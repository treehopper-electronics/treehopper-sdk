using System.ComponentModel;
using System.Threading.Tasks;
using Treehopper.Utilities;

namespace Treehopper
{
    /// <summary>
    /// This class implements a fake Treehopper connection 
    /// with a plausible name and serial number. 
    /// </summary>
    /// <remarks>
    /// This class is useful when building GUIs
    /// </remarks>
    public class DesignTimeConnection : IConnection
    {
        /// <summary>
        /// Construct a new DesignTime connection with a randomly-named board
        /// </summary>
        public DesignTimeConnection()
        {
            Serial = Utility.RandomString(16);
            Name = "MyTreehopper "+ Utility.RandomString(2);
            
        }

        /// <summary>
        /// The name of the board
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The serial number of the board
        /// </summary>
        public string Serial { get; private set;  }
        
        /// <summary>
        /// The device path of the board
        /// </summary>
        public string DevicePath { get; set; }

        /// <summary>
        /// The update rate to use
        /// </summary>
        public int UpdateRate { get; set; }

        /// <summary>
        /// The version of the board.
        /// </summary>
        public short Version { get; set; }

        /// <summary>
        /// An event that fires when pin data is received. This event never fires.
        /// </summary>
        public event PinEventData PinEventDataReceived;

        /// <summary>
        /// An event that fires when any property changes. This event never fires.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Close the DesignTime connection. Note that this function has no effect.
        /// </summary>
        public void Close()
        {
            
        }

        /// <summary>
        /// Open the connection. Note that this function has no effect.
        /// </summary>
        /// <returns>An awaitable task that always returns true</returns>
        public async Task<bool> OpenAsync()
        {
            return true;
        }

        /// <summary>
        /// Send peripheral data over the DesignTime connection. Note that this function has no effect.
        /// </summary>
        /// <param name="data">The data to send</param>
        public async Task SendDataPeripheralChannel(byte[] data)
        {
            
        }

        /// <summary>
        /// Send pin config data over the DesignTime connection. Note that this function has no effect.
        /// </summary>
        /// <param name="data"></param>
        public async Task SendDataPinConfigChannel(byte[] data)
        {
            
        }

        /// <summary>
        /// Read peripheral data from the DesignTime connection.  Note that this function has no effect.
        /// </summary>
        /// <param name="bytesToRead"></param>
        /// <returns></returns>
        public async Task<byte[]> ReadPeripheralResponsePacket(uint bytesToRead)
        {
            return new byte[0];
        }

        /// <summary>
        /// Dispose this DesignTime connection.
        /// </summary>
        public void Dispose()
        {
          
        }
    }
}
