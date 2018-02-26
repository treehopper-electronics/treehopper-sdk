using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Utilities;

namespace Treehopper.Libraries.Sensors.Inertial
{
    [Supports("STMicroelectronics", "LIS3DH")]
    public partial class Lis3dh : IAccelerometer
    {
        private SMBusDevice dev;
        private Vector3 accel;
        private Lis3dhRegisters registers;

        public Lis3dh(I2C i2c, bool sdo = true)
        {
            dev = new SMBusDevice((byte)(sdo ? 0x19 : 0x18), i2c);
            registers = new Lis3dhRegisters(dev);

            if (Task.Run(registers.whoAmI.read).Result.value != 0x33)
            {
                Utility.Error("Incorrect chip ID found when addressing the LIS3DH");
            }

            registers.ctrl1.xAxisEnable = 1;
            registers.ctrl1.yAxisEnable = 1;
            registers.ctrl1.zAxisEnable = 1;
            registers.ctrl1.setOutputDataRate(OutputDataRates.Hz_1);
            registers.ctrl1.lowPowerEnable = 0;
            Task.Run(registers.ctrl1.write).Wait();

            //reg.TempCfgReg.AdcEn = 1;
            //reg.TempCfgReg.TempEn = 1;
            //reg.TempCfgReg.Write().Wait();



            //reg.WriteRange(reg.Ctrl0, reg.ActivationDuration).Wait();

        }

        public bool AutoUpdateWhenPropertyRead { get; set; } = true;
        public int AwaitPollingInterval { get; set; } = 10;
        public async Task Update()
        {
            await registers.readRange(registers.outX, registers.outZ).ConfigureAwait(false);
            accel.X = (float) registers.outX.value;
            accel.Y = (float)registers.outY.value;
            accel.Z = (float)registers.outZ.value;
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
