using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors
{
    public abstract class ProximitySensor : IProximity
    {
        public double Millimeters => Meters * 1000;
        public double Centimeters => Meters * 100;
        public double Inches => Meters * 39.3701;
        public double Feet => Meters * 3.28085;
        public abstract double Meters { get; }
        public bool AutoUpdateWhenPropertyRead { get; set; }
        public int AwaitPollingInterval { get; set; }

        public abstract Task Update();
    }
}
