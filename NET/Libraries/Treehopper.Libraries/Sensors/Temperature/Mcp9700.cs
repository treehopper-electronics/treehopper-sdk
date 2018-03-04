using System.ComponentModel;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Temperature
{
    /// <summary>
    ///     Microchip MCP9700 and MCP9701 analog temperature sensor
    /// </summary>
    public class Mcp9700 : TemperatureSensorBase
    {
        /// <summary>
        ///     An enumeration representing which sensor to use
        /// </summary>
        public enum Type
        {
            /// <summary>
            ///     MCP9700, MCP9700A
            /// </summary>
            Mcp9700,

            /// <summary>
            ///     MCP9701, MCP9701A
            /// </summary>
            Mcp9701
        }

        private readonly AdcPin pin;
        private readonly Type type;

        /// <summary>
        ///     Construct a new Microchip MCP9700 or MCP9701
        /// </summary>
        public Mcp9700(AdcPin pin, Type type = Type.Mcp9700)
        {
            this.type = type;
            this.pin = pin;
            pin.MakeAnalogInAsync();
            pin.AnalogVoltageChangedThreshold = 0.01;
            pin.AnalogVoltageChanged += Pin_AnalogVoltageChanged;
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
            var voltage = pin.AnalogVoltage;
            var v0 = type == Type.Mcp9700 ? 0.5 : 0.4;
            var tc = type == Type.Mcp9700 ? 0.01 : 0.0195;
            celsius = (voltage - v0) / tc;

            RaisePropertyChanged(this);
        }

        private async void Pin_AnalogVoltageChanged(object sender, AnalogVoltageChangedEventArgs e)
        {
            await UpdateAsync();
        }
    }
}