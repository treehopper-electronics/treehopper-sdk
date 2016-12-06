using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Motors.Amis30624
{
    /// <summary>
    /// Step mode
    /// </summary>
    public enum StepMode
    {
        /// <summary>
        /// Half-stepping
        /// </summary>
        HalfStepping,

        /// <summary>
        /// Quarter-stepping
        /// </summary>
        QuarterStepping,

        /// <summary>
        /// Eighth-stepping
        /// </summary>
        EighthStepping,

        /// <summary>
        /// Sixteenth-stepping
        /// </summary>
        SixteenthStepping
    }
}
