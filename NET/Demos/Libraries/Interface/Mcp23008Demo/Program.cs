using Treehopper;
using Treehopper.Libraries.IO.PortExpander;

var board = await ConnectionService.Instance.GetFirstDeviceAsync();
await board.ConnectAsync();

var gpio = new Mcp23008(board.I2c);

gpio.Pins[0].DigitalValue = true;
gpio.Pins[7].PullUpEnabled = true;
while (!Console.KeyAvailable)
{
    Console.WriteLine(await gpio.Pins[7].AwaitDigitalValueChangeAsync());
}