using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Optical
{
    /// <summary>
    /// Rohm Semiconductor BH1750FVI ambient light sensor
    /// </summary>
    [Supports("Rohm Semiconductor", "BH1750FVI")]
    public class Bh1750 : AmbientLightSensor
    {
        /// <summary>
        /// Probes the specified I2C bus to discover any BH1750s attacked.
        /// </summary>
        /// <param name="i2c">The bus to probe</param>
        /// <param name="rate">The rate, in kHz</param>
        /// <returns></returns>
        public static async Task<IList<Bh1750>> ProbeAsync(I2C i2c, int rate=100)
        {
            List<Bh1750> devs = new List<Bh1750>();

            i2c.Enabled = true;

            bool oldExceptionsValue = TreehopperUsb.Settings.ThrowExceptions;
            TreehopperUsb.Settings.ThrowExceptions = true;

            try
            {
                var response = await i2c.SendReceiveAsync(0x23, null, 1).ConfigureAwait(false);
                devs.Add(new Bh1750(i2c, false, rate));
            }
            catch (Exception) { }

            try
            {
                var response = await i2c.SendReceiveAsync(0x5C, null, 1).ConfigureAwait(false);
                devs.Add(new Bh1750(i2c, true, rate));
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
                Task.Run(() => dev.WriteByteAsync((byte)(0x10 | (byte)resolution))).Wait();
            }
        }


        public Bh1750(I2C i2c, bool addressPin = false, int rate=100)
        {
            this.dev = new SMBusDevice((byte)(addressPin ? 0x5C : 0x23), i2c, rate);
            Task.Run(() => dev.WriteByteAsync(0x07)).Wait(); // reset
            Resolution = LuxResolution.High;
        }

        /// <summary>
        /// Requests a reading from the sensor and updates its data properties with the gathered values.
        /// </summary>
        /// <returns>An awaitable Task</returns>
        /// <remarks>
        /// Note that when #AutoUpdateWhenPropertyRead is `true` (which it is, by default), this method is implicitly 
        /// called when any sensor data property is read from --- there's no need to call this method unless you set
        /// AutoUpdateWhenPropertyRead to `false`.
        /// 
        /// Unless otherwise noted, this method updates all sensor data simultaneously, which can often lead to more efficient
        /// bus usage (as well as reducing USB chattiness).
        /// </remarks>
        public override async Task UpdateAsync()
        {
            _lux = await dev.ReadWordBEAsync().ConfigureAwait(false) / 1.2;
            RaisePropertyChanged(this);
        }
    }
}
