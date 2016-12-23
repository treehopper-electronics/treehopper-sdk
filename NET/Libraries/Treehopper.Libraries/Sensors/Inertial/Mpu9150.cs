using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
namespace Treehopper.Libraries.Sensors.Inertial
{
    public class Mpu9150 : Mpu6050
    {
        public Mpu9150(I2c i2c, bool addressPin = false, int ratekHz = 400) : base(i2c, addressPin, ratekHz)
        {
            dev.WriteByteData((byte)Registers.INT_PIN_CFG, 0x22).Wait();
            mag = new SMBusDevice(0x0C, i2c, ratekHz);
        }

        private SMBusDevice mag;

        protected Vector3 magnetometer = new Vector3();
        public Vector3 Magnetometer
        {
            get
            {
                if (AutoUpdateWhenPropertyRead) Update().Wait();
                return magnetometer;
            }
        }

        public bool EnableMagnetometer { get; set; }

        public override async Task Update()
        {
            await base.Update();

            if (!EnableMagnetometer) return;

            var magData = await mag.ReadBufferData((byte)AK8975CRegisters.HXL, 6);

            magnetometer.X = magData[1] << 8 | magData[0];
            magnetometer.Y = magData[3] << 8 | magData[2];
            magnetometer.Z = magData[5] << 8 | magData[4];

        }

        protected enum AK8975CRegisters
        {
            WIA = 0x00,
            INFO = 0x01,
            ST1 = 0x02,
            HXL = 0x03,
            HXH = 0x04,
            HYL = 0x05,
            HYH = 0x06,
            HZL = 0x07,
            HZH = 0x08,

            ST2 = 0x09,
            CNTL = 0x0A,
            RSV = 0x0B,
            ASTC = 0x0C,
            TS1 = 0x0D,
            TS2 = 0x0E,
            I2CDIS = 0x0F,
            ASAX = 0x10,
            ASAY = 0x11,
            ASAZ = 0x12
        }
    }
}
