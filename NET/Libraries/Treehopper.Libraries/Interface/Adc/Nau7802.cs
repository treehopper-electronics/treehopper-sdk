using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Libraries.Sensors.Temperature;
using Treehopper.Utilities;

namespace Treehopper.Libraries.Interface.Adc
{
    public class Nau7802 : TemperatureSensor, IAdcPeripheral
    {
        SMBusDevice dev;
        Ctrl1 ctrl1;
        Ctrl2 ctrl2;
        I2cCtrl i2cCtrl = new I2cCtrl();
        PowerUpControl puCtrl;

        enum Registers
        {
            PuCtrl,
            Ctrl1,
            Ctrl2,
            Ocal1_B2,
            Ocal1_B1,
            Ocal1_B0,
            GCal1_B3,
            GCal1_B2,
            GCal1_B1,
            GCal1_B0,
            Ocal2_B2,
            Ocal2_B1,
            Ocal2_B0,
            GCal2_B3,
            GCal2_B2,
            GCal2_B1,
            GCal2_B0,
            I2cCtrl,
            Adco_B2,
            Adco_B1,
            Adco_B0,
            Otp_B1,
            Otp_B0,
            DevRevision
        };

        public enum LdoVoltage
        {
            mV_4500,
            mV_4200,
            mV_3900,
            mV_3600,
            mV_3300,
            mV_3000,
            mV_2700,
            mV_2400
        };

        public enum GainSelect
        {
            x1,
            x2,
            x4,
            x8,
            x16,
            x32,
            x64,
            x128
        };

        public enum ConversionRate
        {
            SPS_10,
            SPS_20,
            SPS_40,
            SPS_80,
            SPS_320
        }

        public enum CalibrationMode
        {
            OffsetCalibrationInternal,
            Reserved,
            OffsetCalibrationSystem,
            GainCalibrationSystem
        }

        struct PowerUpControl
        {
            [BitField(1)]
            public bool RegisterReset;

            [BitField(1)]
            public bool PowerUpDigitalCircuit;

            [BitField(1)]
            public bool PowerUpAnalogCircuit;

            [BitField(1)]
            public bool PowerUpReady;

            [BitField(1)]
            public bool CycleStart;

            [BitField(1)]
            public bool CycleReady;

            [BitField(1)]
            public bool UseExternalCrystal;

            [BitField(1)]
            public bool UseInternalLdo;
        }

        struct Ctrl1
        {
            [BitField(3)]
            public GainSelect Gains;

            [BitField(3)]
            public LdoVoltage LdoVoltage;

            [BitField(1)]
            public bool DrdyOutputClockOrConversionReady;

            [BitField(1)]
            public bool ConversionReadyWhenCrdyLow;
        }

        struct Ctrl2
        {
            [BitField(2)]
            public CalibrationMode CalMod;

            [BitField(1)]
            public bool CalStart;

            [BitField(1)]
            public bool CalError;

            [BitField(3)]
            public ConversionRate SampleRate;

            [BitField(1)]
            public int Channel;

        }

        struct I2cCtrl
        {
            [BitField(1)]
            public bool DisableBandgapChopper;

            [BitField(1)]
            public bool ReadTemperatureSensor;

            [BitField(1)]
            public bool EnableBurnoutCurrentSource;

            [BitField(1)]
            public bool ShortInputTogether;

            [BitField(1)]
            public bool DisableWeakPullUp;

            [BitField(1)]
            public bool EnableStrongPullUp;

            [BitField(1)]
            public bool EnableFastReadAdcData;

            [BitField(1)]
            public bool EnablePullSdaLowOnConversionComplete;
        }

        public GainSelect Gain
        {
            get { return ctrl1.Gains; }
            set { ctrl1.Gains = value; UpdateCtrl1().Wait(); UpdatePinGains(); }
        }

        public LdoVoltage Ldo
        {
            get { return ctrl1.LdoVoltage; }
            set { ctrl1.LdoVoltage = value; UpdateCtrl1().Wait(); UpdatePinGains(); }
        }

        public bool UseInternalLdo
        {
            get { return puCtrl.UseInternalLdo; }
            set { puCtrl.UseInternalLdo = true; UpdatePuCtrl().Wait(); }
        }

        public ConversionRate SampleRate
        {
            get { return ctrl2.SampleRate; }
            set { ctrl2.SampleRate = value; UpdateCtrl2().Wait(); }
        }

        private Task UpdatePuCtrl()
        {
            return dev.WriteByteData((byte)Registers.PuCtrl, puCtrl.ToByte());
        }

        private Task UpdateCtrl1()
        {
            return dev.WriteByteData((byte)Registers.Ctrl1, ctrl1.ToByte());
        }

        private Task UpdateI2cCtrl()
        {
            return dev.WriteByteData((byte)Registers.I2cCtrl, i2cCtrl.ToByte());
        }

        public async override Task Update()
        {
            // update temperature sensor
            i2cCtrl.ReadTemperatureSensor = true;
            await UpdateI2cCtrl();
            var result = await PerformConversion();
            i2cCtrl.ReadTemperatureSensor = false;
            await UpdateI2cCtrl();

            for (int i=0;i<2;i++)
            {
                ctrl2.Channel = i;
                await UpdateCtrl2();
                result = await PerformConversion();
                Channels[i].AdcValue = result;
            }
            
        }

        private async Task<int> PerformConversion()
        {
            puCtrl.CycleStart = true;
            await UpdatePuCtrl();
            while (!await ConversionDone()) { }
            puCtrl.CycleStart = false;
            await UpdatePuCtrl();
            var adcResult = await dev.ReadBufferData((byte)Registers.Adco_B2, 3);
            int result = adcResult[0] << 24 | adcResult[1] << 16 | adcResult[2] << 8;
            result /= 256;
            return result;
        }
        private Task UpdateCtrl2()
        {
            return dev.WriteByteData((byte)Registers.Ctrl2, ctrl2.ToByte());
        }

        private async Task<bool> ConversionDone()
        {
            var result = await dev.ReadByteData((byte)Registers.PuCtrl);
            return (result & 0x20) > 0 ? true : false;
        }

        private void UpdatePinGains()
        {
            double ldoVoltage = 4.5 - (int)ctrl1.LdoVoltage * 0.3;
            double gain = 1 << ((int)ctrl1.Gains);

            foreach(var channel in Channels)
            {
                channel.ReferenceVoltage = ldoVoltage / gain;
            }
        }

        public IList<NauPin> Channels { get; private set; } = new List<NauPin>();

        public Nau7802(I2c i2c)
        {
            Channels.Add(new NauPin(this));
            Channels.Add(new NauPin(this));

            dev = new SMBusDevice(0x2A, i2c);
            puCtrl = new PowerUpControl() { RegisterReset = true }; // reset all registers
            UpdatePuCtrl().Wait();
            puCtrl = new PowerUpControl() { PowerUpDigitalCircuit = true }; // power up digital
            UpdatePuCtrl().Wait();

            // useful defaults
            puCtrl = new PowerUpControl()
            {
                UseExternalCrystal = false,
                UseInternalLdo = true,
                PowerUpDigitalCircuit = true,
                PowerUpAnalogCircuit = true
            };
            UpdatePuCtrl().Wait();
            UpdatePinGains(); // set the pins up with the default gains
        }

        public class NauPin : AdcPeripheralPin
        {
            public NauPin(IAdcPeripheral parent) : base(parent, 24, 0)
            {

            }
        }
    }
}
