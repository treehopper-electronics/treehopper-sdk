using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Treehopper.Libraries;
using Treehopper.Libraries.Sensors.Inertial;
using Treehopper.Libraries.Sensors.Optical;
using System.IO;
using SkiaSharp;

namespace TreehopperApp.Pages.Libraries
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Flir : LibraryComponent
    {
        public Pin CsPin { get; set; }

        public TreehopperUsb Board { get; }

        public FlirLepton Sensor { get; set; }

        public ImageSource ImageStream { get; set; }

        SKBitmap bitmap = new SKBitmap(80, 40);

        public Flir(LibrariesPage page, TreehopperUsb Board = null) : base("FLIR Lepton", page)
        {
            this.Board = Board;
            InitializeComponent();
            BindingContext = this;
        }

        public Flir() : base("FLIR Lepton", new LibrariesPage())
        {
            InitializeComponent();
            BindingContext = this;
        }

        public override async Task Start()
        {
            Sensor = new FlirLepton(Board.Spi, CsPin);
            Task.Run(run);
        }

        async Task run()
        {
            while(IsRunning)
            {
                var data = await Sensor.GetRawFrameAsync();
                for (int i = 0; i < bitmap.Width; i++)
                {
                    for (int j = 0; j < bitmap.Height; j++)
                    {
                        bitmap.SetPixel(i, j, valueToHeat(data[j, i]));
                    }
                }

                canvasView.InvalidateSurface();
            }
        }

        public override async Task Stop()
        {
            IsRunning = false;
        }

        public override void Dispose()
        {
            IsRunning = false;
        }

        private SKColor valueToHeat(ushort val)
        {
            var color = Treehopper.Libraries.Utilities.Color.FromHsl((1-(val / (float)(UInt16.MaxValue))) * 360f, 100, 50);
            return new SKColor(color.R, color.G, color.B);
        }

        public override async Task Update()
        {

        }

        private void SKCanvasView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            canvasView.HeightRequest = canvasView.Width * 50 / 80;
            var scale = e.Info.Width / 80;
            e.Surface.Canvas.Scale(scale);
            e.Surface.Canvas.DrawBitmap(bitmap, new SKPoint(0, 0));
        }
    }
}