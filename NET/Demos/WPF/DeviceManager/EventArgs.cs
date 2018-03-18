using System;

namespace Treehopper.Mvvm
{
    /// <summary>
    ///     EventArgs representing when the selected board has changed
    /// </summary>
    public class SelectedBoardChangedEventArgs : EventArgs
    {
        /// <summary>
        ///     The new selected board
        /// </summary>
        public TreehopperUsb Board { get; set; }
    }

    /// <summary>
    ///     EventArgs representing when the selected board has connected
    /// </summary>
    public class BoardConnectedEventArgs : EventArgs
    {
        /// <summary>
        ///     The connected board.
        /// </summary>
        public TreehopperUsb Board { get; set; }
    }

    /// <summary>
    ///     EventArgs representing when the selected board has disconnected
    /// </summary>
    public class BoardDisconnectedEventArgs : EventArgs
    {
        /// <summary>
        ///     The disconnected board.
        /// </summary>
        public TreehopperUsb Board { get; set; }
    }
}