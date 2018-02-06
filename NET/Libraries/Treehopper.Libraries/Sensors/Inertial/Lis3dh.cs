using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Utilities;

namespace Treehopper.Libraries.Sensors.Inertial
{
    [Supports("ST", "LIS3DH")]
    public partial class Lis3dh : IAccelerometer
    {
        private SMBusDevice dev;
        private Vector3 accel;
        private Lis3dhRegisters reg;

        public Lis3dh(I2C i2c, bool sdo = true)
        {
            dev = new SMBusDevice((byte)(sdo ? 0x19 : 0x18), i2c);
            reg = new Lis3dhRegisters(dev);

            if (Task.Run(reg.whoAmI.read).Result.value != 0x33)
            {
                Utility.Error("Incorrect chip ID found when addressing the LIS3DH");
            }

            reg.ctrl1.xAxisEnable = 1;
            reg.ctrl1.yAxisEnable = 1;
            reg.ctrl1.zAxisEnable = 1;
            reg.ctrl1.setOutputDataRate(OutputDataRates.Hz_1);
            reg.ctrl1.lowPowerEnable = 0;
            Task.Run(reg.ctrl1.write).Wait();

            //reg.TempCfgReg.AdcEn = 1;
            //reg.TempCfgReg.TempEn = 1;
            //reg.TempCfgReg.Write().Wait();



            //reg.WriteRange(reg.Ctrl0, reg.ActivationDuration).Wait();

        }

        public bool AutoUpdateWhenPropertyRead { get; set; } = true;
        public int AwaitPollingInterval { get; set; } = 10;
        public async Task Update()
        {
            await reg.readRange(reg.outX, reg.outZ).ConfigureAwait(false);
            accel.X = (float) reg.outX.value;
            accel.Y = (float)reg.outY.value;
            accel.Z = (float)reg.outZ.value;
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
