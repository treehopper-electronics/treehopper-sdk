namespace Treehopper
{
    using System;

    /// <summary>
    /// An Exception representing I2c transfer errors that may occur
    /// </summary>
    public class I2cTransferException : Exception
    {
        /// <summary>
        /// The I2c transfer error that happened
        /// </summary>
        public I2cTransferError Error { get; set; }
    }
}
