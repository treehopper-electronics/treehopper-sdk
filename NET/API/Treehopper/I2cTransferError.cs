namespace Treehopper
{
    /// <summary>
    /// Describes the transfer error, if not Success, that occured
    /// </summary>
    public enum I2cTransferError
    {
        /// <summary>
        /// Bus arbitration was lost
        /// </summary>
        ArbitrationLostError,

        /// <summary>
        /// The slave device failed to Nack back.
        /// </summary>
        NackError,

        /// <summary>
        /// Unknown error
        /// </summary>
        UnknownError,

        /// <summary>
        /// Tx buffer underrun error
        /// </summary>
        TxunderError,

        /// <summary>
        /// Successful transaction
        /// </summary>
        Success = 255
    }
}
