using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Desktop;
using Treehopper.Libraries.Input;

namespace RotaryEncoderDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            App();
        }

        static async Task App()
        {
            var board = await ConnectionService.Instance.GetFirstDeviceAsync();
            await board.ConnectAsync();

            var encoder = new RotaryEncoder(board.Pins[0], board.Pins[1], 4);
            encoder.MinValue = 0;
            encoder.MaxValue = 19;
            encoder.PositionChanged += Encoder_PositionChanged;
        }

        private static void Encoder_PositionChanged(object sender, RotaryEncoder.PositionChangedEventArgs e)
        {
            Console.WriteLine($"New position: {e.NewPosition}");
        }
    }
}
