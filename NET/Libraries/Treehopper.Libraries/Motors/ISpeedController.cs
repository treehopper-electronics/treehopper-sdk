using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Motors
{
    public interface ISpeedController
    {
        bool Enabled { get; set; }
        /// <summary>
        /// Set or retrieve the speed -- from -1.0 to 1.0 -- of the motor
        /// </summary>
        double Speed { get; set; }
    }
}
