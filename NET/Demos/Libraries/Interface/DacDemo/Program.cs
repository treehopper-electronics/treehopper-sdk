using System.Diagnostics;
using Treehopper;
using Treehopper.Libraries.IO.Dac;

var board = await ConnectionService.Instance.GetFirstDeviceAsync();
await board.ConnectAsync();

var dac = new Mcp4725(board.I2c);
Stopwatch sw = new Stopwatch();
Console.WriteLine("Outputing sine wave. Press any key to stop");
sw.Start();
double frequency = 10;
while (!Console.KeyAvailable)
{
    dac.Value = Math.Sin(2.0 * Math.PI * (frequency * sw.Elapsed.TotalMilliseconds / 1000.0)) / 2.0 + 0.5;
}
Console.WriteLine("Disconnected");