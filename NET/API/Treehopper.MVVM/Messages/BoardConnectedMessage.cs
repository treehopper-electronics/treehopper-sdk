using Treehopper;

namespace Treehopper.Mvvm.Messages
{
    /// <summary>
    /// This message is used to communicate board connection/disconnection.
    /// </summary>
    public class BoardConnectedMessage
    {
        /// <summary>
        /// The connected board
        /// </summary>
        public TreehopperUsb Board { get; set; }
    }
}
