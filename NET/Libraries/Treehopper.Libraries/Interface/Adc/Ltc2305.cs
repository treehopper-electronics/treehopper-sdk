using System;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface.Adc
{
    /// <summary>
    ///     Linear LTC2305 12-bit two-channel ADC
    /// </summary>
    public class Ltc2305
    {
        /// <summary>
        ///     The ADC channels
        /// </summary>
        public enum Channel
        {
            /// <summary>
            ///     Channel 0
            /// </summary>
            Channel0 = 0xC0,

            /// <summary>
            ///     Channel 1
            /// </summary>
            Channel1 = 0x80
        }

        private readonly SMBusDevice dev;

        /// <summary>
        ///     Construct a new LTC2305
        /// </summary>
        /// <param name="address">The address to use</param>
        /// <param name="I2cModule">The I2c module this ADC is attached to</param>
        public Ltc2305(byte address, I2C I2cModule)
        {
            dev = new SMBusDevice(address, I2cModule, 100);
        }

        /// <summary>
        ///     Read the specified channel from this ADC
        /// </summary>
        /// <param name="channel">The channel to read</param>
        /// <returns>An awaitable double value representing the voltage of the ADC</returns>
        public async Task<double> Read(Channel channel = Channel.Channel0)
        {
            //  SendReceive(byte address, byte[] dataToWrite, byte numBytesToRead)
            int data = await dev.ReadWordData((byte) ((byte) channel | (byte) Ltc2305ConfigBits.UnipolarMode));
            var retVal = CodeToVoltage(data, 5.0, Ltc2305ConfigBits.UnipolarMode);
            return retVal;
        }

        private double CodeToVoltage(int adc_code, double vref, Ltc2305ConfigBits uni_bipolar)
        {
            double voltage;
            double sign = 1;

            if (uni_bipolar == Ltc2305ConfigBits.UnipolarMode)
            {
                voltage =
                    (adc_code >> 4) /
                    (Math.Pow(2, 12) -
                     1); //! 2) This calculates the input as a fraction of the reference voltage (dimensionless)
            }
            else
            {
                vref = vref / 2;
                if ((adc_code & 0x8000) == 0x8000) //adc code is < 0
                {
                    adc_code = (adc_code ^ 0xFFFF) + 1; //! Convert ADC code from two's complement to binary
                    sign = -1;
                }
                voltage = sign * adc_code;
                voltage =
                    voltage /
                    (Math.Pow(2, 15) -
                     1); //! 2) This calculates the input as a fraction of the reference voltage (dimensionless)
            }
            voltage = voltage * vref; //! 3) Multiply fraction by Vref to get the actual voltage at the input (in volts)


            return voltage;
        }

        private enum Ltc2305ConfigBits
        {
            SleepMode = 0x04,
            ExitSleepMode = 0x00,
            UnipolarMode = 0x08,
            BipolarMode = 0x00,
            SingleEndedMode = 0x80,
            DifferentialMode = 0x00,
            P0_N1 = 0x00,
            P1_N0 = 0x40
        }
    }
}