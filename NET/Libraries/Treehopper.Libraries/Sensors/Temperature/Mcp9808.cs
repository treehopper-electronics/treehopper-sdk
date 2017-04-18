using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Temperature
{
    /// <summary>
    /// Microchip MCP9808 I2c temperature sensor
    /// </summary>
    public class Mcp9808 : TemperatureSensor
    {
        private readonly SMBusDevice dev;

        /// <summary>
        /// Construct a new MCP9808 temperature sensor
        /// </summary>
        /// <param name="i2c">The i2C port to use</param>
        /// <param name="a0">the state of the A0 pin</param>
        /// <param name="a1">the state of the A1 pin</param>
        /// <param name="a2">the state of the A2 pin</param>
        public Mcp9808(I2c i2c, bool a0 = false, bool a1 = false, bool a2 = false)
        {
            dev = new SMBusDevice((byte)(0x18 | (a0 ? 1 : 0) | ((a1 ? 1 : 0) << 1) | ((a2 ? 1 : 0) << 2)), i2c);
        }

        /// <summary>
        /// Force an update of the MCP9808 temperature sensor
        /// </summary>
        /// <returns>An awaitable task</returns>
        public override async Task Update()
        {
            var data = await dev.ReadWordDataBE(0x05);
            double temp = data & 0x0FFF;
            temp /= 16.0;
            if ((data & 0x1000) > 0)
                temp -= 256;
            Celsius = temp;
        }
    }
}
