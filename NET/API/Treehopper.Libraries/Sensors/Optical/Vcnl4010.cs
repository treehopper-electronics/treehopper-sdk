using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Optical
{
    /// <summary>
    /// Vishay VCNL4010  proximity and ambient light sensor
    /// </summary>
    [Supports("Vishay", "VCNL4010")]
    public partial class Vcnl4010 : ProximitySensor, IAmbientLightSensor
    {
        Vcnl4010Registers registers;
        double lux;
        int rawProximity;

        /// <summary>
        /// Construct a VCNL4010
        /// </summary>
        /// <param name="i2c">The bus the device is attached to.</param>
        public Vcnl4010(I2C i2c)
        {
            registers = new Vcnl4010Registers(new SMBusRegisterManagerAdapter(new SMBusDevice(0x13, i2c)));
            Task.Run(() => registers.readRange(registers.command, registers.ambientLightParameters)).Wait();
            registers.proximityRate.setRate(Rates.Hz_7_8125);
            registers.ledCurrent.irLedCurrentValue = 20;
            registers.ambientLightParameters.setAlsRate(AlsRates.Hz_10);
            registers.ambientLightParameters.autoOffsetCompensation = 1;
            registers.ambientLightParameters.averagingSamples = 5;
            Task.Run(() => registers.writeRange(registers.command, registers.ambientLightParameters)).Wait();
        }

        /// <summary>
        /// Gets or sets the LED current --- in mA --- used by the VCNL4010
        /// </summary>
        /// <remarks>
        /// Adjusting this value from its default (200 mA) will affect the accuracy of the proximity distances reported.
        /// </remarks>
        public int LedCurrent
        {
            get
            {
                return registers.ledCurrent.irLedCurrentValue * 10;
            }

            set
            {
                registers.ledCurrent.irLedCurrentValue = (int)Math.Round(value / 10d);
                Task.Run(registers.ledCurrent.writeAsync).Wait();
            }
        }

        public double Lux
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    Task.Run(UpdateAsync).Wait();

                return lux;
            }
        }

        /// <summary>
        /// Gets the raw proximity data reported by the sensor
        /// </summary>
        public int RawProximitiy
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    Task.Run(UpdateAsync).Wait();

                return rawProximity;
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
            // start ambient and prox conversion
            registers.command.alsOnDemandStart = 1;
            registers.command.proxOnDemandStart = 1;
            await registers.command.writeAsync().ConfigureAwait(false);


            while(true)
            {
                await registers.command.readAsync().ConfigureAwait(false);
                if (registers.command.proxDataReady == 1 && registers.command.alsDataReady == 1)
                    break;
            }

            await registers.ambientLightResult.readAsync().ConfigureAwait(false);
            await registers.proximityResult.readAsync().ConfigureAwait(false);
            
            // from datasheet
            lux = registers.ambientLightResult.value * 0.25;

            rawProximity = registers.proximityResult.value;

            // derived empirically
            if (registers.proximityResult.value < 2298)
                meters = double.PositiveInfinity;
            else
                meters = 81.0 * Math.Pow(registers.proximityResult.value - 2298, -0.475) / 100;

            RaisePropertyChanged(this);
            RaisePropertyChanged(this, "Lux");
            RaisePropertyChanged(this, "RawProximitiy");
        }
    }
}
