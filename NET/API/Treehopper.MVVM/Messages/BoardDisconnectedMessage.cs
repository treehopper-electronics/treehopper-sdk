namespace Treehopper.Mvvm.Messages
{
    /// <summary>
    /// A Message representing when the board has disconnected
    /// </summary>
    public class BoardDisconnectedMessage
    {
        /// <summary>
        /// The disconnected board
        /// </summary>
        public TreehopperUsb Board { get; set; }
    }
}
