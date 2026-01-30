using Treehopper;
using Treehopper.Libraries.Displays;

var board = await ConnectionService.Instance.GetFirstDeviceAsync();

await board.ConnectAsync();

var controller = new Tm1650(board.I2c);
var display = new SevenSegmentDisplay(controller.Leds);
display.Text = 2345;