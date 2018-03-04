using System.ComponentModel;
using System.Threading.Tasks;

namespace Treehopper.Libraries.Sensors.Temperature
{
    /// <summary>
    ///     Microchip MCP9808 I2c temperature sensor
    /// </summary>
    public class Mcp9808 : TemperatureSensorBase
    {
        private readonly SMBusDevice dev;

        /// <summary>
        ///     Construct a new MCP9808 temperature sensor
        /// </summary>
        /// <param name="i2c">The i2C port to use</param>
        /// <param name="a0">the state of the A0 pin</param>
        /// <param name="a1">the state of the A1 pin</param>
        /// <param name="a2">the state of the A2 pin</param>
        public Mcp9808(I2C i2c, bool a0 = false, bool a1 = false, bool a2 = false)
        {
            dev = new SMBusDevice((byte) (0x18 | (a0 ? 1 : 0) | ((a1 ? 1 : 0) << 1) | ((a2 ? 1 : 0) << 2)), i2c);
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
            var data = await dev.ReadWordDataBEAsync(0x05);
            double temp = data & 0x0FFF;
            temp /= 16.0;
            if ((data & 0x1000) > 0)
                temp -= 256;
            celsius = temp;

            RaisePropertyChanged(this);
        }
    }
}