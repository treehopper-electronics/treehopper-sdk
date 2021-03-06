﻿using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;
using Treehopper.Libraries.Utilities;

namespace Treehopper.Libraries.Sensors.Magnetic
{
    /// <summary>
    ///     Honeywell HMC5883L 3-axis digital compass
    /// </summary>
    [Supports("Honewell", "HMC5883L")]
    public class Hmc5883l : MagnetometerBase
    {
        /// <summary>
        ///     Range (gain) settings
        /// </summary>
        public enum RangeSetting
        {
            /// <summary>
            ///     +/- 0.88 Ga
            /// </summary>
            GAIN_0_88 = 0x10,

            /// <summary>
            ///     +/- 1.3 Ga
            /// </summary>
            GAIN_1_3 = 0x20,

            /// <summary>
            ///     +/- 1.9 Ga
            /// </summary>
            GAIN_1_9 = 0x40,

            /// <summary>
            ///     +/- 2.5 Ga
            /// </summary>
            GAIN_2_5 = 0x60,

            /// <summary>
            ///     +/- 4.0 Ga
            /// </summary>
            GAIN_4_0 = 0x80,

            /// <summary>
            ///     +/- 4.7 Ga
            /// </summary>
            GAIN_4_7 = 0xA0,

            /// <summary>
            ///     +/- 5.6 Ga
            /// </summary>
            GAIN_5_6 = 0xC0,

            /// <summary>
            ///     +/- 8.1 Ga
            /// </summary>
            GAIN_8_1 = 0xE0,

            /// <summary>
            ///     Reserved (uninitialized) gain setting
            /// </summary>
            Reserved
        }

        private readonly SMBusDevice dev;
        private float Gauss_XY;
        private float Gauss_Z;

        private Vector3 mag;

        private RangeSetting range = RangeSetting.Reserved;

        /// <summary>
        ///     Construct a new HMC5883L connected to the specified <see cref="I2C" /> port.
        /// </summary>
        /// <param name="i2c">The I2C port to use</param>
        /// <param name="rateKHz">The frequency, in kHz, to use.</param>
        public Hmc5883l(I2C i2c, int rateKHz = 100)
        {
            dev = new SMBusDevice(0x1E, i2c, rateKHz);

            var idA = dev.ReadByteDataAsync((byte) Registers.IdA).Result;
            var idB = dev.ReadByteDataAsync((byte) Registers.IdB).Result;
            var idC = dev.ReadByteDataAsync((byte) Registers.IdC).Result;

            if (idA != 0x48 || idB != 0x34 || idC != 0x33)
                Debug.WriteLine("WARNING: this library may not be compatible with the attached chip");

            dev.WriteByteDataAsync((byte) Registers.ConfigA, 0x1C).Wait();
            dev.WriteByteDataAsync((byte) Registers.Mode, 0x00).Wait(); // continuous conversion
            Range = RangeSetting.GAIN_1_3;
        }

        /// <summary>
        ///     The range, in Gauss, to use.
        /// </summary>
        /// <remarks>
        ///     <para>For maximum resolution, select the minimum range value that constrains your desired signal.</para>
        /// </remarks>
        public RangeSetting Range
        {
            get { return range; }
            set
            {
                if (range == value) return;
                range = value;

                dev.WriteByteDataAsync((byte) Registers.ConfigB, (byte) range);

                switch (range)
                {
                    case RangeSetting.GAIN_0_88:
                        Gauss_XY = 1380;
                        Gauss_Z = 1380;
                        break;
                    case RangeSetting.GAIN_1_3:
                        Gauss_XY = 1100;
                        Gauss_Z = 980;
                        break;
                    case RangeSetting.GAIN_1_9:
                        Gauss_XY = 855;
                        Gauss_Z = 760;
                        break;
                    case RangeSetting.GAIN_2_5:
                        Gauss_XY = 670;
                        Gauss_Z = 600;
                        break;
                    case RangeSetting.GAIN_4_0:
                        Gauss_XY = 450;
                        Gauss_Z = 400;
                        break;
                    case RangeSetting.GAIN_4_7:
                        Gauss_XY = 400;
                        Gauss_Z = 255;
                        break;
                    case RangeSetting.GAIN_5_6:
                        Gauss_XY = 330;
                        Gauss_Z = 295;
                        break;
                    case RangeSetting.GAIN_8_1:
                        Gauss_XY = 230;
                        Gauss_Z = 205;
                        break;
                }
            }
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
            var bytes = await dev.ReadBufferDataAsync((byte) Registers.XOutHi, 6).ConfigureAwait(false);
            var report = bytes.BytesToStruct<DataReport>(Endianness.BigEndian);
            mag.X = report.x / Gauss_XY * 100;
            mag.Y = report.y / Gauss_XY * 100;
            mag.Z = report.z / Gauss_Z * 100;

            RaisePropertyChanged(this);
        }

        private enum Registers
        {
            ConfigA = 0x00,
            ConfigB = 0x01,
            Mode = 0x02,
            XOutHi = 0x03,
            XOutLo = 0x04,
            ZOutHi = 0x05,
            ZOutLo = 0x06,
            YOutHi = 0x07,
            YOutLo = 0x08,
            Status = 0x09,
            IdA = 0x0A,
            IdB = 0x0B,
            IdC = 0x0C,
            TEMP_OUT_H_M = 0x31,
            TEMP_OUT_L_M = 0x32
        }

        private struct DataReport
        {
            public short x;
            public short z;
            public short y;
        }
    }
}