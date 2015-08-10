using Treehopper;

namespace Treehopper.WPF.Message
{
    /// <summary>
    /// This message is used to communicate board connection/disconnection.
    /// </summary>
    public class BoardConnectedMessage
    {
        public TreehopperUsb Board { get; set; }
    }
}
