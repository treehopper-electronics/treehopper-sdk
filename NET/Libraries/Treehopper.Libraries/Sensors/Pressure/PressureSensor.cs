using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Pressure
{
    /// <summary>
    ///     Pressure sensor base
    /// </summary>
    public abstract class PressureSensor : IPressure
    {
        /// <summary>
        ///     The pressure, in bars
        /// </summary>
        public double Bar => Pascal / 100000.0;

        /// <summary>
        ///     The pressure, in Atmospheres.
        /// </summary>
        public double Atm => Pascal / (1.01325 * 100000.0);

        /// <summary>
        ///     The pressure, in PSI
        /// </summary>
        public double Psi => Atm / 14.7;

        /// <summary>
        ///     Whether to update this pressure sensor when a property is read from
        /// </summary>
        public bool AutoUpdateWhenPropertyRead { get; set; }

        /// <summary>
        ///     The pressure, in Pascal
        /// </summary>
        public double Pascal { get; protected set; }

        /// <summary>
        ///     Update the sensor
        /// </summary>
        /// <returns>An awaitable task</returns>
        public abstract Task Update();

        public int AwaitPollingInterval { get; set; }
    }
}