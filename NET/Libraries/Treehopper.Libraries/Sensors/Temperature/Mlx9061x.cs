using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Temperature
{
    public class Mlx90614
    {
        protected SMBusDevice dev;

        public Mlx90614(I2c module)
        {
            this.dev = new SMBusDevice(0x5A, module);
            Object =  new TempRegister(dev, 0x07);
            Ambient = new TempRegister(dev, 0x06);
        }
        public ITemperatureSensor Ambient { get; protected set; }
        public ITemperatureSensor Object { get; protected set; }

        public int RawIrData { get { return dev.ReadWordData(0x25).Result;  } }

        protected class TempRegister : TemperatureSensor
        {
            public override string ToString()
            {
                return TemperatureCelsius.ToString();
            }
            private SMBusDevice dev;
            private byte register;

            public TempRegister(SMBusDevice dev, byte register)
            {
                this.register = register;
                this.dev = dev;
            }

            public override async Task Update()
            {
                var data = await dev.ReadWordData(register).ConfigureAwait(false);

                data &= 0x7FFF; // chop off the error bit of the high byte
                TemperatureCelsius = data * 0.02 - 273.15;
            }
        }
    }

    public class Mlx90615 : Mlx90614
    {
        public Mlx90615(I2c module) : base(module)
        {
            this.dev = new SMBusDevice(0x5B, module);
            Object = new TempRegister(dev, 0x27);
            Ambient = new TempRegister(dev, 0x26);
        }
    }
}
