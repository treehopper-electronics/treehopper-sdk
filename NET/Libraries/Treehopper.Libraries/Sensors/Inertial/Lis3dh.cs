using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Utilities;

namespace Treehopper.Libraries.Sensors.Inertial
{
    public partial class Lis3dh : IAccelerometer
    {
        private SMBusDevice dev;
        private Vector3 accel;
        private Lis3dhRegisters reg;

        public Lis3dh(I2C i2c, bool sdo = true)
        {
            dev = new SMBusDevice((byte)(sdo ? 0x19 : 0x18), i2c);
            reg = new Lis3dhRegisters(dev);

            if (reg.WhoAmI.Read().Result.Value != 0x33)
            {
                Utility.Error("Incorrect chip ID found when addressing the LIS3DH");
            }

            reg.Ctrl1.XAxisEnable = 1;
            reg.Ctrl1.YAxisEnable = 1;
            reg.Ctrl1.ZAxisEnable = 1;
            reg.Ctrl1.SetOutputDataRate(OutputDataRates.Hz_1);
            reg.Ctrl1.LowPowerEnable = 0;
            reg.Ctrl1.Write().Wait();

            //reg.TempCfgReg.AdcEn = 1;
            //reg.TempCfgReg.TempEn = 1;
            //reg.TempCfgReg.Write().Wait();



            //reg.WriteRange(reg.Ctrl0, reg.ActivationDuration).Wait();

        }

        public bool AutoUpdateWhenPropertyRead { get; set; } = true;
        public int AwaitPollingInterval { get; set; } = 10;
        public async Task Update()
        {
            await reg.ReadRange(reg.OutX, reg.OutZ).ConfigureAwait(false);
            accel.X = (float) reg.OutX.Value;
            accel.Y = (float)reg.OutY.Value;
            accel.Z = (float)reg.OutZ.Value;
        }

        public Vector3 Accelerometer
        {
            get
            {
                if (AutoUpdateWhenPropertyRead) Update().Wait();
                return accel;
            }
        }
    }
}
