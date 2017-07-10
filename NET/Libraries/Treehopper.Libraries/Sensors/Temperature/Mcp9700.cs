using System.ComponentModel;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Temperature
{
    /// <summary>
    ///     Microchip MCP9700 and MCP9701 analog temperature sensor
    /// </summary>
    public class Mcp9700 : TemperatureSensor
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

        public override event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Construct a new Microchip MCP9700 or MCP9701
        /// </summary>
        public Mcp9700(AdcPin pin, Type type = Type.Mcp9700)
        {
            this.type = type;
            this.pin = pin;
            pin.MakeAnalogIn();
            pin.AnalogVoltageChangedThreshold = 0.01;
            pin.AnalogVoltageChanged += Pin_AnalogVoltageChanged;
            ;
        }

        /// <summary>
        ///     Read the current AnalogVoltage and update the temperature data
        /// </summary>
        /// <returns>An awaitable task</returns>
        public override async Task Update()
        {
            var voltage = pin.AnalogVoltage;
            var v0 = type == Type.Mcp9700 ? 0.5 : 0.4;
            var tc = type == Type.Mcp9700 ? 0.01 : 0.0195;
            Celsius = (voltage - v0) / tc;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Celsius)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Fahrenheit)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Kelvin)));
        }

        private async void Pin_AnalogVoltageChanged(object sender, AnalogVoltageChangedEventArgs e)
        {
            await Update();
        }
    }
}