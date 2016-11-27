using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Interface
{


    public class Ltc2305 : SMBusDevice
    {
        public Ltc2305(byte address, I2c I2cModule) : base(address, I2cModule, 100)
        {

        }

        public async Task<double> Read(Ltc2305Channels channelNumber = Ltc2305Channels.Channel0)
        {
           //  SendReceive(byte address, byte[] dataToWrite, byte numBytesToRead)
            byte[] data = await I2c.SendReceive(this.address, new byte[] { (byte)((byte)channelNumber | (byte)Ltc2305ConfigBits.UnipolarMode) }, 2);
            int code = data[1] | (data[0]<<8);
            double retVal = CodeToVoltage(code, 5.0, Ltc2305ConfigBits.UnipolarMode);
            return retVal;
        }

        private double CodeToVoltage(int adc_code, double vref, Ltc2305ConfigBits uni_bipolar)
        {
            double voltage;
            double sign = 1;

            if (uni_bipolar == Ltc2305ConfigBits.UnipolarMode)
            {
                voltage = (adc_code >> 4) / (Math.Pow(2, 12) - 1);    //! 2) This calculates the input as a fraction of the reference voltage (dimensionless)
            }
            else
            {
                vref = vref / 2;
                if ((adc_code & 0x8000) == 0x8000)	//adc code is < 0
                {
                    adc_code = (adc_code ^ 0xFFFF) + 1;                                    //! Convert ADC code from two's complement to binary
                    sign = -1;
                }
                voltage = sign * (float)adc_code;
                voltage = voltage / (Math.Pow(2, 15) - 1);    //! 2) This calculates the input as a fraction of the reference voltage (dimensionless)
            }
            voltage = voltage * vref;           //! 3) Multiply fraction by Vref to get the actual voltage at the input (in volts)


            return (voltage);
        }

        public enum Ltc2305ConfigBits
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

        public enum Ltc2305Channels
        {
            Channel0 = 0xC0,
            Channel1 = 0x80
        }
    }
}
