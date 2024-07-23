using System;

namespace Treehopper
{
    /// <summary>
    ///     This class represents global settings used by the Treehopper API
    /// </summary>
    public class Settings
    {
        /// <summary>
        ///     Get or sets whether exceptions should be thrown.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         If true, any exceptions received will be thrown to user code, as well as printed. If false, exceptions will
        ///         only be printed.
        ///     </para>
        /// </remarks>
        public bool ThrowExceptions { get; set; } = false;

        /// <summary>
        ///     The VID to search for
        /// </summary>
        public uint Vid { get; set; } = 0x10c4;

        /// <summary>
        ///     The PID to search for
        /// </summary>
        public uint Pid { get; set; } = 0x8a7e;

        /// <summary>
        ///     The WinUSB GUID used by the library
        /// </summary>
        public Guid Guid { get; set; } = new Guid("{5B34B38B-F4CD-49C3-B2BB-60E47A43E12D}");

        /// <summary>
        ///     Whether or not writing to properties should return immediately, or only upon successfully sending to the board.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Properties provide a useful abstraction for the state of peripherals (i.e., <see cref="Pin.DigitalValue" />),
        ///         but they are inherently synchronous. This setting allows you to control how writes to properties behave; when
        ///         true, a write to one of these properties will queue up a request to the board but will return immediately ---
        ///         setting this property to false will synchronously wait for the message to be sent to the board.
        ///     </para>
        /// </remarks>
        public bool PropertyWritesReturnImmediately { get; set; } = false;
    }
}