using System;
using System.Threading;
using System.Threading.Tasks;
using Treehopper.Utilities;

namespace Treehopper.Libraries.IO.Adc
{
    /// <summary>
    ///     Nuvoton NAU7802 24-bit dual-channel ADC for bridge sensors
    /// </summary>
    public partial class Nau7802 : AdcPeripheralPin, IAdcPeripheral
    {
        private DigitalIn drdy;
        private bool autoUpdate = false;
        private SMBusDevice dev;
        private Nau7802Registers registers;

        public Nau7802(I2C i2c, DigitalIn drdy = null) : base(null, 24, 0)
        {
            parent = this;

            if (drdy != null)
            {
                this.drdy = drdy;
                drdy.MakeDigitalIn();
                drdy.DigitalValueChanged += Drdy_DigitalValueChanged;
            }

            AutoUpdateWhenPropertyRead = true;

            dev = new SMBusDevice(0x2A, i2c);
            registers = new Nau7802Registers(dev);

            registers.PuCtrl.RegisterReset = 1;  // reset all registers
            registers.PuCtrl.Write().Wait();
            registers.PuCtrl.RegisterReset = 0;  // clear reset
            registers.PuCtrl.PowerUpDigital = 1; // power up digital
            registers.PuCtrl.Write().Wait();

            Task.Delay(10).Wait();
            if (registers.PuCtrl.Read().Result.PowerUpReady == 0)
            {
                Utility.Error("Could not power up NAU7802");
                return;
            }


            // useful defaults
            registers.PuCtrl.UseExternalCrystal = 0;
            registers.PuCtrl.UseInternalLdo = 1;
            registers.PuCtrl.PowerUpDigital = 1;
            registers.PuCtrl.PowerUpAnalog = 1;
            registers.PuCtrl.Write().Wait();

            registers.Ctrl1.SetVldo(LdoVoltage.mV_3000);
            registers.Ctrl1.Write().Wait();

            UpdateReferenceVoltage(); // set the pins up with the default gains

            registers.Pga.PgaBypass = 0;
            registers.Pga.DisableChopper = 1;
            registers.Pga.Write().Wait();

            registers.Adc.SetRegChpFreq(RegChpFreqs.off);
            registers.Adc.RegChp = 0;
            registers.Adc.Write().Wait();

            registers.I2cCtrl.BgpCp = 0;
            registers.I2cCtrl.Write().Wait();

            registers.Ctrl2.SetConversionRate(ConversionRates.Sps_10);
            registers.Ctrl2.Write().Wait();

            registers.PuCtrl.CycleStart = 1;
            registers.PuCtrl.Write().Wait();


        }

        public bool AutoUpdateWhenPropertyRead
        {
            get { return autoUpdate; }
            set
            {
                if (drdy != null) return;
                autoUpdate = value;
            } // if the user constructed us with a DRDY pin, we should *never* update when property read.
        }

        public int AwaitPollingInterval { get; set; }

        public async Task Update()
        {
            AdcValue = await PerformConversion();
        }

        private async Task<int> PerformConversion()
        {
            if (drdy == null)
                while (!await ConversionDone()) ;

            await registers.AdcResult.Read();
            return registers.AdcResult.Value;
        }

        private async Task<bool> ConversionDone()
        {
            await registers.Read(registers.PuCtrl);
            return registers.PuCtrl.CycleReady == 1;
        }

        private void UpdateReferenceVoltage()
        {
            var ldoVoltage = 4.5 - registers.Ctrl1.Vldo * 0.3;
            double gain = 1 << registers.Ctrl1.Gain;

            ReferenceVoltage = ldoVoltage / gain;
        }

        public async Task<bool> Calibrate()
        {
            registers.Ctrl2.CalStart = 1;
            await registers.Ctrl2.Write().ConfigureAwait(false);
            registers.Ctrl2.CalStart = 1;

            while ((await registers.Ctrl2.Read()).CalStart == 1) ;

            return (registers.Ctrl2.CalError == 0);
        }

        public Gains Gain
        {
            get { return registers.Ctrl1.GetGain(); }
            set {
                registers.Ctrl1.SetGain(value);
                registers.Ctrl1.Write().Wait();
            }
        }

        public bool Cfilter
        {
            get { return registers.PowerCtrl.PgaCapEn > 0; }
            set {
                registers.PowerCtrl.PgaCapEn = value ? 1 : 0;
                registers.PowerCtrl.Write().Wait();
            }
        }

        public ConversionRates ConversionRate
        {
            get { return registers.Ctrl2.GetConversionRate(); }
            set
            {
                registers.Ctrl2.SetConversionRate(value);
                registers.Ctrl2.Write().Wait();
            }
        }

        private async void Drdy_DigitalValueChanged(object sender, DigitalInValueChangedEventArgs e)
        {
            if (e.NewValue) // only convert on rising edge, plz
                AdcValue = await PerformConversion();
        }

        public async Task SetChannel(int channel)
        {
            registers.Ctrl2.ChannelSelect = channel;
            await registers.Ctrl2.Write();
            await Calibrate();
        }
    }
}