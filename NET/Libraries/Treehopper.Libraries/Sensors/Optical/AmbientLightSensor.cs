using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Optical
{
    public abstract class AmbientLightSensor : IAmbientLightSensor
    {
        protected double lux;
        public double Lux
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    Task.Run(UpdateAsync).Wait();

                return lux;
            }
        }

        public bool AutoUpdateWhenPropertyRead { get; set; } = true;
        public int AwaitPollingInterval { get; set; }

        public abstract Task UpdateAsync();
    }
}
