using Treehopper;
using Treehopper.Libraries;
using Treehopper.Libraries.Sensors.Temperature;
/// <summary>
///     MLX90615 non-contact IR thermopile temperature sensor
/// </summary>
public class Mlx90615 : Mlx90614
{
    /// <summary>
    ///     Construct a new MLX90615 attached to the given i2c port
    /// </summary>
    /// <param name="module"></param>
    public Mlx90615(I2C module) : base(module)
    {
        dev = new SMBusDevice(0x5B, module);
        Object = new TempRegister(dev, 0x27);
        Ambient = new TempRegister(dev, 0x26);
    }
}