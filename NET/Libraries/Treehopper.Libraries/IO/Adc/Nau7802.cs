using System;
using System.Threading;
using System.Threading.Tasks;
using Treehopper.Utilities;

namespace Treehopper.Libraries.IO.Adc
{
    /// <summary>
    ///     Nuvoton NAU7802 24-bit dual-channel ADC for bridge sensors
    /// </summary>
    [Supports("Nuvoton", "NAU7802")]
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
                drdy.MakeDigitalInAsync();
                drdy.DigitalValueChanged += Drdy_DigitalValueChanged;
            }

            AutoUpdateWhenPropertyRead = true;

            dev = new SMBusDevice(0x2A, i2c);
            registers = new Nau7802Registers(dev);

            registers.puCtrl.registerReset = 1;  // reset all registers
            registers.puCtrl.write().Wait();
            registers.puCtrl.registerReset = 0;  // clear reset
            registers.puCtrl.powerUpDigital = 1; // power up digital
            registers.puCtrl.write().Wait();

            Task.Delay(10).Wait();
            if (registers.puCtrl.read().Result.powerUpReady == 0)
            {
                Utility.Error("Could not power up NAU7802");
                return;
            }


            // useful defaults
            registers.puCtrl.useExternalCrystal = 0;
            registers.puCtrl.useInternalLdo = 1;
            registers.puCtrl.powerUpDigital = 1;
            registers.puCtrl.powerUpAnalog = 1;
            registers.puCtrl.write().Wait();

            registers.ctrl1.setVldo(Vldoes.mV_3000);
            registers.ctrl1.write().Wait();

            UpdateReferenceVoltage(); // set the pins up with the default gains

            registers.pga.pgaBypass = 0;
            registers.pga.disableChopper = 1;
            registers.pga.write().Wait();

            registers.adc.setRegChpFreq(RegChpFreqs.off);
            registers.adc.regChp = 0;
            registers.adc.write().Wait();

            registers.i2cCtrl.bgpCp = 0;
            registers.i2cCtrl.write().Wait();

            registers.ctrl2.setConversionRate(ConversionRates.Sps_10);
            registers.ctrl2.write().Wait();

            registers.puCtrl.cycleStart = 1;
            registers.puCtrl.write().Wait();
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

        public async Task UpdateAsync()
        {
            AdcValue = await PerformConversionAsync();
        }

        private async Task<int> PerformConversionAsync()
        {
            if (drdy == null)
                while (!await ConversionDoneAsync()) ;

            await registers.adcResult.read();
            return registers.adcResult.value;
        }

        private async Task<bool> ConversionDoneAsync()
        {
            await registers.read(registers.puCtrl);
            return registers.puCtrl.cycleReady == 1;
        }

        private void UpdateReferenceVoltage()
        {
            var ldoVoltage = 4.5 - registers.ctrl1.vldo * 0.3;
            double gain = 1 << registers.ctrl1.gain;

            ReferenceVoltage = ldoVoltage / gain;
        }

        public async Task<bool> CalibrateAsync()
        {
            registers.ctrl2.calStart = 1;
            await registers.ctrl2.write().ConfigureAwait(false);
            registers.ctrl2.calStart = 1;

            while ((await registers.ctrl2.read()).calStart == 1) ;

            return (registers.ctrl2.calError == 0);
        }

        public Gains Gain
        {
            get { return registers.ctrl1.getGain(); }
            set {
                registers.ctrl1.setGain(value);
                registers.ctrl1.write().Wait();
            }
        }

        public bool Cfilter
        {
            get { return registers.powerCtrl.pgaCapEn > 0; }
            set {
                registers.powerCtrl.pgaCapEn = value ? 1 : 0;
                registers.powerCtrl.write().Wait();
            }
        }

        public ConversionRates ConversionRate
        {
            get { return registers.ctrl2.getConversionRate(); }
            set
            {
                registers.ctrl2.setConversionRate(value);
                registers.ctrl2.write().Wait();
            }
        }

        private async void Drdy_DigitalValueChanged(object sender, DigitalInValueChangedEventArgs e)
        {
            if (e.NewValue) // only convert on rising edge, plz
                AdcValue = await PerformConversionAsync();
        }

        public async Task SetChannelAsync(int channel)
        {
            registers.ctrl2.channelSelect = channel;
            await registers.ctrl2.write();
            await CalibrateAsync();
        }
    }
}