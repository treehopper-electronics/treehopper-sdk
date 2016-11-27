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
        /// The VID to search for
        /// </summary>
        public uint Vid { get; set; }

        /// <summary>
        /// The PID to search for
        /// </summary>
        public uint Pid { get; set; }

        /// <summary>
        /// The PID used by the bootloader
        /// </summary>
        public uint BootloaderPid { get; set; }

        /// <summary>
        /// The VID used by the bootloader
        /// </summary>
        public uint BootloaderVid { get; set; }

        /// <summary>
        /// The WinUSB GUID used by the librarh
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Construct a new settings object with Treehopper defaults.
        /// </summary>
        public Settings()
        {
            Guid = new Guid("{5B34B38B-F4CD-49C3-B2BB-60E47A43E12D}");
            Vid = 0x10c4;
            Pid = 0x8a7e;

            BootloaderVid = 0x10c4;
            BootloaderPid = 0xeac9;
        }
    }
}
