using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Motors
{
    /// <summary>
    /// 1A, 36-V dual H-bridge motor controller
    /// </summary>
    [Supports("Texas Instruments", "L293D")]
    public class L293d
    {
        /// <summary>
        /// Channel A H-bridge
        /// </summary>
        public DualHalfBridge ChannelA { get; set; }

        /// <summary>
        /// Channel B H-Bridge
        /// </summary>
        public DualHalfBridge ChannelB { get; set; }
    }
}
