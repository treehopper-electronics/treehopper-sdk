using Treehopper;
using Treehopper.Libraries.Input;

var board = await ConnectionService.Instance.GetFirstDeviceAsync();
await board.ConnectAsync();

var encoder = new RotaryEncoder(board.Pins[0], board.Pins[1], 4);
encoder.MinValue = 0;
encoder.MaxValue = 19;
encoder.PositionChanged += Encoder_PositionChanged;

static void Encoder_PositionChanged(object sender, RotaryEncoder.PositionChangedEventArgs e)
{
    Console.WriteLine($"New position: {e.NewPosition}");
}
