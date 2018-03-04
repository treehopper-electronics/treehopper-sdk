using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper;
using Treehopper.Libraries;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Treehopper.Libraries.Displays;
using Treehopper.Libraries.IO.PortExpander;

namespace TreehopperControlCenter.Pages.Libraries
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Display : LibraryComponent
    {
        public Pwm SelectedPin { get; set; }
        public TreehopperUsb Board { get; }

        public string[] DisplayType { get; set; } = new string[] { "16x2 with PCA8574 I2C Expander", "SSD1309 128x64 OLED" };
        public string SelectedDisplay { get; set; }

        ICharacterDisplay display;

        private string text = "";
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                if (display == null)
                {
                    text = value;
                    return;
                }

                // capture old text value, since we run updates in a separate Task
                string oldText = text;

                if (value.Length-1 == oldText.Length && value.Substring(0, oldText.Length) == text)
                {
                    // we have an appended character
                    Task.Run(() => display.Write(value[oldText.Length])); // write the new char
                } else
                {
                    Task.Run(async () =>
                    {
                        await display.Clear().ConfigureAwait(false);
                        await display.Write(value).ConfigureAwait(false);
                    }).Wait();
                }

                text = value;
            }
        }

        public Display(LibrariesPage page, TreehopperUsb Board = null) : base("Display", page)
        {
            this.Board = Board;
            InitializeComponent();
            BindingContext = this;
        }

        public Display() : base("Display", new LibrariesPage())
        {
            InitializeComponent();
        }

        protected override async Task Start()
        {
            switch (SelectedDisplay)
            {
                case "SSD1309 128x64 OLED":
                    display = new Ssd1306(Board.I2c, 128, 32, 60, 800);
                    break;

                case "16x2 with PCA8574 I2C Expander":
                    display = HobbyDisplayFactories.GetCharacterDisplayFromPcf8574(new Pcf8574(Board.I2c));
                    break;
            }
            Text = "Hello, world!";
            OnPropertyChanged("Text");
        }

        protected override async Task Stop()
        {
            Task.Run(display.Clear);
        }

        public override void Dispose()
        {
            Task.Run(display.Clear);
        }

        public override async Task Update()
        {

        }
    }
}