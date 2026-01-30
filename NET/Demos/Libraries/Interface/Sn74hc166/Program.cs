using System.Diagnostics;
using Treehopper;
using Treehopper.Libraries.IO.PortExpander;

var board = await ConnectionService.Instance.GetFirstDeviceAsync();
await board.ConnectAsync();
var shiftRegister = new Hc166(board.Spi, board.Pins[10]);
while (true)
{
    var value = shiftRegister.Pins[0].AwaitDigitalValueChangeAsync();
    Debug.WriteLine(value);
}