using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Treehopper.Libraries.Sensors.Inertial.MPU9150
{
    public class MPU9150
    {
        public MPU9150(TreehopperUsb board, Pin interruptPin, MPU9160Address address)
        {

        }
    }

    public enum MPU9160Address
    {
        AD0_LOW = 0x68,
        AD0_HIGH = 0x69
    }

}
