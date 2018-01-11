using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Optical
{
    /// <summary>
    /// Vishay VCNL4010  proximity and ambient light sensor
    /// </summary>
    public partial class Vcnl4010 : Proximity, IAmbientLight
    {
        Vcnl4010Registers registers;
        double lux;
        double meters;
        int rawProximity;

        /// <summary>
        /// Construct a VCNL4010
        /// </summary>
        /// <param name="i2c">The bus the device is attached to.</param>
        /// <remarks>
        /// The device is configured inside the constructor, so make sure the device is attached to the bus when you're initializing the object.
        /// </remarks>
        public Vcnl4010(I2C i2c)
        {
            registers = new Vcnl4010Registers(new SMBusDevice(0x13, i2c));
            Task.Run(() => registers.ReadRange(registers.Command, registers.AmbientLightParameters)).Wait();
            registers.ProximityRate.SetRate(Rates.Hz_7_8125);
            registers.LedCurrent.IrLedCurrentValue = 20;
            registers.AmbientLightParameters.SetAlsRate(AlsRates.Hz_10);
            registers.AmbientLightParameters.AutoOffsetCompensation = 1;
            registers.AmbientLightParameters.AveragingSamples = 5;
            Task.Run(() => registers.WriteRange(registers.Command, registers.AmbientLightParameters)).Wait();
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
                return registers.LedCurrent.IrLedCurrentValue * 10;
            }

            set
            {
                registers.LedCurrent.IrLedCurrentValue = (int)Math.Round(value / 10d);
                Task.Run(registers.LedCurrent.Write).Wait();
            }
        }

        public double Lux
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    Task.Run(Update).Wait();

                return lux;
            }
        }

        public override double Meters
        {
            get
            {
                if (AutoUpdateWhenPropertyRead)
                    Task.Run(Update).Wait();

                return meters;
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
                    Task.Run(Update).Wait();

                return rawProximity;
            }
        }

        /// <summary>
        /// Request an update of the lux and proximity data from the sensor.
        /// </summary>
        /// <returns>An awaitable Task that completes upon an update.</returns>
        public override async Task Update()
        {
            // start ambient and prox conversion
            registers.Command.AlsOnDemandStart = 1;
            registers.Command.ProxOnDemandStart = 1;
            await registers.Command.Write().ConfigureAwait(false);


            while(true)
            {
                await registers.Command.Read().ConfigureAwait(false);
                if (registers.Command.ProxDataReady == 1 && registers.Command.AlsDataReady == 1)
                    break;
            }

            await registers.AmbientLightResult.Read().ConfigureAwait(false);
            await registers.ProximityResult.Read().ConfigureAwait(false);
            
            // from datasheet
            lux = registers.AmbientLightResult.Value * 0.25;

            rawProximity = registers.ProximityResult.Value;

            // derived empirically
            if (registers.ProximityResult.Value < 2298)
                meters = double.PositiveInfinity;
            else
                meters = 81.0 * Math.Pow(registers.ProximityResult.Value - 2298, -0.475) / 100;
        }
    }
}
