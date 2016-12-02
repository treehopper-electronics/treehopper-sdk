using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper
{
    /// <summary>
    /// This class represents global settings used by the Treehopper API
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Get or sets whether exceptions should be thrown.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If true, any exceptions received will be thrown to user code, as well as printed. If false, exceptions will only be printed.
        /// </para>
        /// </remarks>
        public bool ThrowExceptions { get; set; }
        /// <summary>
        /// The VID to search for
        /// </summary>
        public uint Vid { get; set; } = 0x10c4;

        /// <summary>
        /// The PID to search for
        /// </summary>
        public uint Pid { get; set; } = 0x8a7e;

        /// <summary>
        /// The PID used by the bootloader
        /// </summary>
        public uint BootloaderPid { get; set; } = 0xeac9;

        /// <summary>
        /// The VID used by the bootloader
        /// </summary>
        public uint BootloaderVid { get; set; } = 0x10c4;

        /// <summary>
        /// The WinUSB GUID used by the librarh
        /// </summary>
        public Guid Guid { get; set; } = new Guid("{5B34B38B-F4CD-49C3-B2BB-60E47A43E12D}");

        /// <summary>
        /// Construct a new settings object with Treehopper defaults.
        /// </summary>
        public Settings()
        {

        }
    }
}
