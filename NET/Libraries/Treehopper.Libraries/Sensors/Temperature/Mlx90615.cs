using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Temperature
{
    public class Mlx90615
    {
        private SMBusDevice dev;

        public Mlx90615(I2c module)
        {
            this.dev = new SMBusDevice(0x5B, module, 30);
            Object =  new TempRegister(dev, 0x27);
            Ambient = new TempRegister(dev, 0x26);
        }
        public Temperature Ambient { get; private set; }
        public Temperature Object { get; private set; }

        public int RawIrData { get { return dev.ReadWordData(0x25).Result;  } }

        internal class TempRegister : TemperatureSensor
        {
            private SMBusDevice dev;
            private byte register;

            public TempRegister(SMBusDevice dev, byte register)
            {
                this.register = register;
                this.dev = dev;
            }

            private double temp = 0;

            public override double TemperatureCelsius
            {
                get
                {
                    if (AutoUpdateWhenPropertyRead) Update().Wait();

                    return temp;
                }
            }

            public override async Task Update()
            {
                var data = await dev.ReadWordData(register);

                data &= 0x7FFF; // chop off the error bit of the high byte
                temp = data * 0.02 - 273.15;
            }
        }
    }
}
