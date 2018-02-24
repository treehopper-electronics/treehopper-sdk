using System;
using System.Collections.Generic;
using System.Text;

namespace Treehopper.Libraries.Sensors.Inertial
{
    public struct EularAngles
    {
        public float Yaw;
        public float Roll;
        public float Pitch;

        public override string ToString()
        {
            return $"Yaw: {Yaw}, Pitch: {Pitch}, Roll: {Roll}";
        }
    }
}
