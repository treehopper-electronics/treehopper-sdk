using Treehopper;
using Treehopper.Libraries.Input;
using Treehopper.Libraries.Sensors;

var board = await ConnectionService.Instance.GetFirstDeviceAsync().ConfigureAwait(false);
await board.ConnectAsync().ConfigureAwait(false);

Console.WriteLine("Starting demo. Press any key to stop...");

using (var poller = new Poller<SuperNesController>(new SuperNesController(board.Spi, board.Pins[5])))
{
    // NES and SNES controllers
    poller.Sensor.A.OnPressed += A_OnPressed;
    poller.Sensor.B.OnPressed += B_OnPressed;
    poller.Sensor.Start.OnPressed += Start_OnPressed;
    poller.Sensor.Select.OnPressed += Select_OnPressed;
    poller.Sensor.DPadStateChanged += Sensor_DPadChanged;

    // SNES only
    poller.Sensor.L.OnPressed += L_OnPressed;
    poller.Sensor.R.OnPressed += R_OnPressed;
    poller.Sensor.X.OnPressed += X_OnPressed;
    poller.Sensor.Y.OnPressed += Y_OnPressed;

    // wait for a key to be pressed (events will continue to fire)
    while (!Console.KeyAvailable)
    {
        await Task.Delay(100);
    }
    Console.Write("Stopping demo...");
    board.Disconnect();
    Console.WriteLine("Board disconnected");
}
static void Y_OnPressed(object sender, Button.ButtonPressedEventArgs e)
{
    Console.WriteLine("Y pressed");
}

static void X_OnPressed(object sender, Button.ButtonPressedEventArgs e)
{
    Console.WriteLine("X pressed");
}

static void R_OnPressed(object sender, Button.ButtonPressedEventArgs e)
{
    Console.WriteLine("Right trigger pressed");
}

static void L_OnPressed(object sender, Button.ButtonPressedEventArgs e)
{
    Console.WriteLine("Left trigger pressed");
}

static void Sensor_DPadChanged(object sender, DPadStateEventArgs e)
{
    Console.WriteLine("DPad: " + e.NewValue);
}

static void A_OnPressed(object sender, Button.ButtonPressedEventArgs e)
{
    Console.WriteLine("A pressed");
}

static void B_OnPressed(object sender, Button.ButtonPressedEventArgs e)
{
    Console.WriteLine("B pressed");
}

static void Start_OnPressed(object sender, Button.ButtonPressedEventArgs e)
{
    Console.WriteLine("Start pressed");
}

static void Select_OnPressed(object sender, Button.ButtonPressedEventArgs e)
{
    Console.WriteLine("Select pressed");
}
