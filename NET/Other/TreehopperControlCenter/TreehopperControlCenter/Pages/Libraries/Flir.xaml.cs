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

namespace TreehopperControlCenter.Pages.Libraries
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

        protected override async Task Start()
        {
            Sensor = new FlirLepton(Board.Spi, CsPin);
            run();
        }

        async Task run()
        {
            while(true)
            {
                var data = await Sensor.GetRawFrameAsync();
                for (int i = 0; i < 80; i++)
                {
                    for (int j = 0; j < 40; j++)
                    {
                        bitmap.SetPixel(i, j, valueToHeat(data[j, i]));
                    }
                }

                canvasView.InvalidateSurface();
            }
        }

        protected override async Task Stop()
        {
            
        }

        public override void Dispose()
        {
            
        }

        private SKColor valueToHeat(ushort val)
        {
            var color = Treehopper.Libraries.Utilities.ColorConverter.FromHsl(val / (float)UInt16.MaxValue * 360f, 100, 50);
            return new SKColor(color.R, color.G, color.B);
        }

        public override async Task Update()
        {

        }

        private void SKCanvasView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.Scale(5, 5);
            e.Surface.Canvas.DrawBitmap(bitmap, new SKPoint(0, 0));
        }
    }
}