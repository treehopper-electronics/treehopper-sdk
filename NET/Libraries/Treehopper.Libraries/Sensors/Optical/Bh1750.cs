using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Optical
{
    [Supports("Rohm Semiconductor", "BH1750FVI")]
    public class Bh1750 : AmbientLightSensor
    {
        public static async Task<IList<Bh1750>> Probe(I2C i2c)
        {
            List<Bh1750> devs = new List<Bh1750>();

            i2c.Enabled = true;

            bool oldExceptionsValue = TreehopperUsb.Settings.ThrowExceptions;
            TreehopperUsb.Settings.ThrowExceptions = true;

            try
            {
                var response = await i2c.SendReceive(0x23, null, 1);
                devs.Add(new Bh1750(i2c, false));
            }
            catch (Exception) { }

            try
            {
                var response = await i2c.SendReceive(0x5C, null, 1);
                devs.Add(new Bh1750(i2c, true));
            }
            catch (Exception) { }

            TreehopperUsb.Settings.ThrowExceptions = false;
            TreehopperUsb.Settings.ThrowExceptions = oldExceptionsValue;

            return devs;
        }

        public enum LuxResolution
        {
            Medium,
            High,
            Low
        }

        private LuxResolution resolution;
        private SMBusDevice dev;

        public LuxResolution Resolution
        {
            get { return resolution; }
            set {
                if (resolution == value) return;
                resolution = value;
                Task.Run(() => dev.WriteByte((byte)(0x10 | (byte)resolution))).Wait();
            }
        }


        public Bh1750(I2C i2c, bool addressPin = false)
        {
            this.dev = new SMBusDevice((byte)(addressPin ? 0x5C : 0x23), i2c, 100);
            Task.Run(() => dev.WriteByte(0x07)).Wait(); // reset
            Resolution = LuxResolution.High;
        }


        public override async Task Update()
        {
            lux = await dev.ReadWordBE() / 1.2;
        }
    }
}
