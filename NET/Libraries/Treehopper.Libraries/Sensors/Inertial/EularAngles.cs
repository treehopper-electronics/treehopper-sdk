using System;
using System.Collections.Generic;
using System.Text;

namespace Treehopper.Libraries.Sensors.Inertial
{
    /// <summary>
    /// Eular angles representing yaw, roll, and pitch
    /// </summary>
    public struct EularAngles
    {
        /// <summary>
        /// Yaw, measured in degrees
        /// </summary>
        public float Yaw;

        /// <summary>
        /// Roll, measured in degrees
        /// </summary>
        public float Roll;

        /// <summary>
        /// Pitch, measured in degrees
        /// </summary>
        public float Pitch;

        /// <summary>
        /// Retrieve a string representation of the Eular angles
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Yaw: {Yaw}, Pitch: {Pitch}, Roll: {Roll}";
        }
    }
}
