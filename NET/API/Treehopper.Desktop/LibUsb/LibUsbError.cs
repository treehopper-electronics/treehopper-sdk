namespace Treehopper.Desktop.LibUsb
{
    /// <summary>
    ///     Error codes.
    ///     Most libusb functions return 0 on success or one of these codes on failure.
    /// </summary>
    public enum LibUsbError
    {
        /// <summary>
        ///     Success (no error)
        /// </summary>
        Success = 0,

        /// <summary>
        ///     Input/output error
        /// </summary>
        ErrorIO = -1,

        /// <summary>
        ///     Invalid parameter
        /// </summary>
        ErrorInvalidParam = -2,

        /// <summary>
        ///     Access denied (insufficient permissions)
        /// </summary>
        ErrorAccess = -3,

        /// <summary>
        ///     No such device (it may have been disconnected)
        /// </summary>
        ErrorNoDevice = -4,

        /// <summary>
        ///     Entity not found
        /// </summary>
        ErrorNotFound = -5,

        /// <summary>
        ///     Resource busy
        /// </summary>
        ErrorBusy = -6,

        /// <summary>
        ///     Operation timed out
        /// </summary>
        ErrorTimeout = -7,

        /// <summary>
        ///     Overflow
        /// </summary>
        ErrorOverflow = -8,

        /// <summary>
        ///     Pipe error
        /// </summary>
        ErrorPipe = -9,

        /// <summary>
        ///     System call interrupted (perhaps due to signal)
        /// </summary>
        ErrorInterrupted = -10,

        /// <summary>
        ///     Insufficient memory
        /// </summary>
        ErrorNoMem = -11,

        /// <summary>
        ///     Operation not supported or unimplemented on this platform
        /// </summary>
        ErrorNotSupported = -12,

        /// <summary>
        ///     Cancel IO failed.
        /// </summary>
        ErrorIOCancelled = -13,

        /// <summary>
        ///     Other error
        /// </summary>
        ErrorOther = -99
    }
}