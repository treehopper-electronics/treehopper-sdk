using System;

namespace Treehopper
{
    /// <summary>
    ///     An Exception representing I2c transfer errors that may occur
    /// </summary>
    public class I2CTransferException : Exception
    {
        /// <summary>
        ///     The I2c transfer error that happened
        /// </summary>
        public I2CTransferError Error { get; set; }
    }
}