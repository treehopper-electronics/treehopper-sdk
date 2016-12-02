﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries.Displays;
using Treehopper.Libraries.Interface;
using Treehopper.Libraries.Interface.PcfSeries;

namespace CharacterLcd16x2Pcf8574
{
    class Program
    {
        static void Main(string[] args)
        {
            App().Wait();
        }

        static async Task App()
        {
            var board = await ConnectionService.Instance.GetFirstDeviceAsync();

            await board.ConnectAsync();
            var ioExpander = new Pcf8574(board.I2c, false, false, false);
            var display = HobbyDisplayFactories.GetCharacterDisplayFromPcf8574(ioExpander, 20, 4);
            display.WriteLine("The current date is:");
            while (true)
            {
                display.WriteLine(DateTime.Now.ToLongTimeString());
                display.WriteLine(DateTime.Now.ToShortDateString());
                display.SetCursorPosition(0, 1);
                await Task.Delay(100);
            }


        }
    }
}