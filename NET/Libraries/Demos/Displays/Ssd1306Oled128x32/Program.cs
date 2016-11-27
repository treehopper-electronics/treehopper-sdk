using Displays;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Windows.UI.Xaml.Media.Imaging;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace Ssd1306Oled128x32
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
            var display = new Ssd1306(board.I2c);
            for (int i = 0; i < display.Height * display.Width / 8; i++)
            {
                display.RawBuffer[i++] = 0x55;
                display.RawBuffer[i] = 0xAA;
            }
            display.Flush();

            var bitmap = new Bitmap(display.Width, display.Height, PixelFormat.Format32bppArgb);

            RectangleF rectf = new RectangleF(0, 0, display.Width, display.Height);

            Graphics g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.None;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            int count = 0;
            while(!Console.KeyAvailable)
            {
                g.FillRectangle(Brushes.Black, 0, 0, display.Width, display.Height);
                g.DrawString("COUNTING UP: " + count++, new Font("Segoe UI Light", 10), Brushes.White, rectf);

                g.Flush();

                for (int i = 0; i < display.Width; i++)
                {
                    for (int j = 0; j < display.Height; j++)
                    {
                        var color = bitmap.GetPixel(i, j);
                        if (color.G > 0)
                            display.BoolBuffer[i, j] = true;
                        else
                            display.BoolBuffer[i, j] = false;
                    }
                }

                display.Flush();
                //await Task.Delay(500);
            }


            //bitmap.PixelBuffer
        }
    }
}
