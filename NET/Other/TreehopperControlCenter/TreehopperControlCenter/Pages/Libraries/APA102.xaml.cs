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

namespace TreehopperControlCenter.Pages.Libraries
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Apa102 : LibraryComponent
    {
        public TreehopperUsb Board { get; }

        public float Speed { get; set; } = 1;

        public int NumLeds { get; set; } = 32;

        Treehopper.Libraries.Displays.Apa102 driver;

        float hueOffset = 0;

        public Apa102(LibrariesPage page, TreehopperUsb Board = null) : base("APA102 RGB LEDs", page)
        {
            this.Board = Board;
            InitializeComponent();
            BindingContext = this;
        }

        public Apa102() : base("APA102 RGB LEDs", new LibrariesPage())
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override async Task Start()
        {
            driver = new Treehopper.Libraries.Displays.Apa102(Board.Spi, NumLeds);
            driver.Brightness = 1;
            driver.AutoFlush = false;
            Run();
        }

        async Task Run()
        {
            while(IsRunning)
            {
                for (int i = 0; i < driver.Leds.Count; i++)
                {
                    driver.Leds[i].SetHsl((360f / driver.Leds.Count) * i + hueOffset, 100, 50);
                }

                await driver.FlushAsync().ConfigureAwait(false);
                await Task.Delay(10).ConfigureAwait(false);
                hueOffset += Speed;
            }
        }

        protected override async Task Stop()
        {

        }

        public override void Dispose()
        {

        }

        public override async Task Update()
        {

        }
    }
}