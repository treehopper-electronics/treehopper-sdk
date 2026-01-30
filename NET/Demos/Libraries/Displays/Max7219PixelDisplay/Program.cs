using Treehopper;
using Treehopper.Libraries.Displays;

var board = await ConnectionService.Instance.GetFirstDeviceAsync();
await board.ConnectAsync();
var display = HobbyDisplayFactories.GetMax7219GraphicLedDisplay(board.Spi, board.Pins[9], 16);

display.Brightness = 0.2;


Console.WriteLine("Press any key to clear display and exit...");

while (!Console.KeyAvailable)
{
    await display.Write(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString());
    await Task.Delay(250);
}

await display.Clear();

board.Disconnect();