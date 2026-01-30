using Treehopper;
using Treehopper.Libraries.Displays;

var board = await ConnectionService.Instance.GetFirstDeviceAsync();
await board.ConnectAsync();
var controller = new Max7219(board.Spi, board.Pins[7]);
var display = new SevenSegmentDisplay(controller.Leds, true);

int i = 0;
while (!Console.KeyAvailable)
{
    display.Text = i++;
    await Task.Delay(10);
}

board.Disconnect();