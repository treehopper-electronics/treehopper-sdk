using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Pressure
{
    public abstract class PressureSensor : IPressure
    {
        public bool AutoUpdateWhenPropertyRead { get; set; }

        public double Pascal { get; protected set; }

        public double Bar { get { return Pascal / 100000.0; } }

        public double Atm { get { return Pascal / (1.01325 * 100000.0); } }

        public double Psi { get { return Atm / 14.7; } }

        public abstract Task Update();
    }
}
