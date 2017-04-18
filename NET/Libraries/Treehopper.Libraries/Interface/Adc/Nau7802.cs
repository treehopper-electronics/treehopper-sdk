namespace Treehopper.Libraries.Interface.Adc
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Utilities;

    /// <summary>
    /// Nuvoton NAU7802 24-bit dual-channel ADC for bridge sensors
    /// </summary>
    public class Nau7802 : AdcPeripheralPin, IAdcPeripheral
    {
        SMBusDevice dev;
        Ctrl1 ctrl1;
        Ctrl2 ctrl2;
        I2cCtrl i2cCtrl = new I2cCtrl();
        PowerUpControl puCtrl;
        Pga pga = new Pga();
        Adc adc = new Adc();
        private DigitalIn drdy;

        public enum Channel
        {
            Ch0,
            Ch1
        }

        enum Registers
        {
            PuCtrl = 0x00,
            Ctrl1 = 0x01,
            Ctrl2 = 0x02,
            Ocal1_B2 = 0x03,
            Ocal1_B1 = 0x04,
            Ocal1_B0 = 0x05,
            GCal1_B3 = 0x06,
            GCal1_B2 = 0x07,
            GCal1_B1 = 0x08,
            GCal1_B0 = 0x09,
            Ocal2_B2 = 0x0A,
            Ocal2_B1 = 0x0B,
            Ocal2_B0 = 0x0C,
            GCal2_B3 = 0x0D,
            GCal2_B2 = 0x0E,
            GCal2_B1 = 0x0F,
            GCal2_B0 = 0x10,
            I2cCtrl = 0x11,
            Adco_B2 = 0x12,
            Adco_B1 = 0x13,
            Adco_B0 = 0x14,
            Adc = 0x15,
            Otp_B0 = 0x16,
            Pga = 0x1B,
            PwerCtrl = 0x1C,
            DevRevision = 0x1F
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CalData
        {
            UInt24 Ocal1;
            UInt32 GCal1;
            UInt24 Ocal2;
            UInt32 GCal2;
        }

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
            x128,
        };

        public enum ConversionRate
        {
            SPS_10 = 0x00,
            SPS_20 = 0x01,
            SPS_40 = 0x02,
            SPS_80 = 0x03,
            SPS_320 = 0x07
        }

        public enum CalibrationMode
        {
            OffsetCalibrationInternal,
            Reserved,
            OffsetCalibrationSystem,
            GainCalibrationSystem
        }

        public enum AdcCommonModeConfiguration
        {
            DisableExtendedCommonMode = 0x00,
            EnableExtendedCommonModeWhenCloseToRefN = 0x02,
            EnableExtendedCommonModeWhenCloseToRefP = 0x03
        }

        public enum AdcClockChopFrequency
        {
            Reserved = 0x0,
            TurnedOff = 0x03
        }


#pragma warning disable 649
        struct Pga
        {
            [Bitfield(1)]
            public bool DisableChopper;
            [Bitfield(2)]
            int Reserved;
            [Bitfield(1)]
            public bool InvertPgaInputPhase;
            [Bitfield(1)]
            public bool BypassPga;
            [Bitfield(1)]
            public bool OutputBufferEnable;
            [Bitfield(1)]
            public bool LdoImproveStability;
            [Bitfield(1)]
            public bool AdcReadWillReadOtp;
        }

        struct Adc
        {
            [Bitfield(2)]
            public int EnableAdcClockDelay;
            [Bitfield(2)]
            public AdcCommonModeConfiguration commonMode;
            [Bitfield(2)]
            public AdcClockChopFrequency chopFrequency;
        }

        struct PowerUpControl
        {
            [Bitfield(1)]
            public bool RegisterReset;

            [Bitfield(1)]
            public bool PowerUpDigitalCircuit;

            [Bitfield(1)]
            public bool PowerUpAnalogCircuit;

            [Bitfield(1)]
            public bool PowerUpReady;

            [Bitfield(1)]
            public bool CycleStart;

            [Bitfield(1)]
            public bool CycleReady;

            [Bitfield(1)]
            public bool UseExternalCrystal;

            [Bitfield(1)]
            public bool UseInternalLdo;
        }

        struct Ctrl1
        {
            [Bitfield(3)]
            public GainSelect Gains;

            [Bitfield(3)]
            public LdoVoltage LdoVoltage;

            [Bitfield(1)]
            public bool DrdyOutputClockOrConversionReady;

            [Bitfield(1)]
            public bool ConversionReadyWhenCrdyLow;
        }

        struct Ctrl2
        {
            [Bitfield(2)]
            public CalibrationMode CalMod;

            [Bitfield(1)]
            public bool CalStart;

            [Bitfield(1)]
            public bool CalError;

            [Bitfield(3)]
            public ConversionRate SampleRate;

            [Bitfield(1)]
            public Channel Channel;

        }

        struct I2cCtrl
        {
            [Bitfield(1)]
            public bool DisableBandgapChopper;

            [Bitfield(1)]
            public bool ReadTemperatureSensor;

            [Bitfield(1)]
            public bool EnableBurnoutCurrentSource;

            [Bitfield(1)]
            public bool ShortInputTogether;

            [Bitfield(1)]
            public bool DisableWeakPullUp;

            [Bitfield(1)]
            public bool EnableStrongPullUp;

            [Bitfield(1)]
            public bool EnableFastReadAdcData;

            [Bitfield(1)]
            public bool EnablePullSdaLowOnConversionComplete;
        }
#pragma warning restore 649
        public GainSelect Gain
        {
            get { return ctrl1.Gains; }
            set {
                ctrl1.Gains = value; UpdateCtrl1().Wait(); UpdateReferenceVoltage(); }
        }

        public bool BypassPga
        {
            get { return pga.BypassPga; }
            set { pga.BypassPga = value; UpdatePga().Wait(); }
        }

        public LdoVoltage Ldo
        {
            get { return ctrl1.LdoVoltage; }
            set { ctrl1.LdoVoltage = value; UpdateCtrl1().Wait(); UpdateReferenceVoltage(); }
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

        private Task UpdatePga()
        {
            return dev.WriteByteData((byte)Registers.Pga, pga.ToByte());
        }

        private Task UpdateAdc()
        {
            return dev.WriteByteData((byte)Registers.Adc, adc.ToByte());
        }

        private async Task<int> PerformConversion()
        {
            if(drdy == null)
            {
                while (!await ConversionDone()) { }
            }
                
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

        private void UpdateReferenceVoltage()
        {
            double ldoVoltage = 4.5 - (int)ctrl1.LdoVoltage * 0.3;
            double gain = 1 << ((int)ctrl1.Gains);

            ReferenceVoltage = ldoVoltage / gain;
        }

        public async Task<bool> Calibrate()
        {
            ctrl2.CalStart = true;
            await UpdateCtrl2();
            ctrl2.CalStart = false;
            while ((await dev.ReadByteData((byte)Registers.Ctrl2) & 0x04) > 0)
            {
            }

            return ((await dev.ReadByteData((byte)Registers.Ctrl2) & 0x08) == 0);

        }

        public CalData CalibrationData
        {
            get
            {
                var calData = dev.ReadBufferData((byte)Registers.Ocal1_B2, 14).Result;
                var result = calData.BytesToStruct<CalData>(Endianness.BigEndian);
                return result;
            }
        }

        bool autoUpdate = true;
        public bool AutoUpdateWhenPropertyRead {
            get { return autoUpdate; }
            set { if (drdy != null) return;  autoUpdate = value; } // if the user constructed us with a DRDY pin, we should *never* update when property read.
        }

        public int AwaitPollingInterval { get; set; }

        public Nau7802(I2c i2c, DigitalIn drdy = null) : base(null, 24, 0)
        {
            if(drdy != null)
            {
                this.drdy = drdy;
                drdy.MakeDigitalIn();
                drdy.DigitalValueChanged += Drdy_DigitalValueChanged;
                autoUpdate = false;
            }

            dev = new SMBusDevice(0x2A, i2c);
            puCtrl = new PowerUpControl() { RegisterReset = true }; // reset all registers
            UpdatePuCtrl().Wait();
            puCtrl = new PowerUpControl() { PowerUpDigitalCircuit = true }; // power up digital
            UpdatePuCtrl().Wait();
            Task.Delay(10).Wait();

            // useful defaults
            puCtrl = new PowerUpControl()
            {
                UseExternalCrystal = false,
                UseInternalLdo = true,
                PowerUpDigitalCircuit = true,
                PowerUpAnalogCircuit = true
            };
            UpdatePuCtrl().Wait();

            ctrl1.LdoVoltage = LdoVoltage.mV_3000;
            UpdateCtrl1().Wait();

            UpdateReferenceVoltage(); // set the pins up with the default gains

            pga.BypassPga = false;
            pga.DisableChopper = true;
            UpdatePga().Wait();

            adc.chopFrequency = AdcClockChopFrequency.TurnedOff;
            adc.EnableAdcClockDelay = 0;
            UpdateAdc().Wait();

            ctrl2.SampleRate = ConversionRate.SPS_10;
            UpdateCtrl2();

            puCtrl.CycleStart = true;
            UpdatePuCtrl().Wait();

            parent = this;
        }

        private async void Drdy_DigitalValueChanged(object sender, DigitalInValueChangedEventArgs e)
        {
            if(e.NewValue == true) // only convert on rising edge, plz
                AdcValue = await PerformConversion();
        }

        public async Task SetChannel(Channel channel)
        {
            ctrl2.Channel = channel;
            await UpdateCtrl2();
            await Calibrate();
        }
        public async Task Update()
        {
            AdcValue = await PerformConversion();
        }
    }
}
