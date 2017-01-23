namespace Treehopper.Libraries.Sensors.Temperature
{
    using System.Threading.Tasks;

    /// <summary>
    /// Microchip MCP9700 and MCP9701 analog temperature sensor library
    /// </summary>
    public class Mcp9700 : TemperatureSensor
    {
        private Type type;
        private AdcPin pin;

        /// <summary>
        /// Construct a new Microchip MCP9700 or MCP9701
        /// </summary>
        public Mcp9700(AdcPin pin, Type type = Type.Mcp9700)
        {
            this.type = type;
            this.pin = pin;
            pin.MakeAnalogIn();
            pin.AnalogVoltageChangedThreshold = 0.01;
            pin.AnalogVoltageChanged += Pin_AnalogVoltageChanged; ;
        }

        /// <summary>
        /// An enumeration representing which sensor to use
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// MCP9700, MCP9700A
            /// </summary>
            Mcp9700,

            /// <summary>
            /// MCP9701, MCP9701A
            /// </summary>
            Mcp9701
        }

        /// <summary>
        /// Read the current AnalogVoltage and update the temperature data
        /// </summary>
        /// <returns>An awaitable task</returns>
        public override async Task Update()
        {
            double voltage = pin.AnalogVoltage;
            double v0 = (type == Type.Mcp9700) ? 0.5 : 0.4;
            double tc = (type == Type.Mcp9700) ? 0.01 : 0.0195;
            Celsius = (voltage - v0) / tc;
        }

        private void Pin_AnalogVoltageChanged(object sender, AnalogVoltageChangedEventArgs e)
        {
            Update();
        }
    }
}
