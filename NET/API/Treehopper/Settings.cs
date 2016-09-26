using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper
{
    public class Settings
    {
        public uint Vid { get; set; }
        public uint Pid { get; set; }

        public uint BootloaderPid { get; set; }
        public uint BootloaderVid { get; set; }

        public Guid Guid { get; set; }


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
