using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Treehopper;
using System.Diagnostics;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Blink
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            TreehopperUsb.BoardAdded += TreehopperUsb_BoardAdded;

        }

       async private void TreehopperUsb_BoardAdded(TreehopperUsb BoardAdded)
        {
            Debug.WriteLine("Board added ");

            BoardAdded.Open();

            BoardAdded.Pin2.MakeDigitalInput();
            BoardAdded.Pin2.DigitalValueChanged += Pin2_DigitalValueChanged;

           BoardAdded.Pin3.ReferenceLevel = AdcReferenceLevel.VREF_1V65;
            BoardAdded.Pin3.MakeAnalogInput();
            //   BoardAdded.Pin3.AnalogValueChanged += Pin3_AnalogValueChanged;
            BoardAdded.Pin3.AdcVoltageChangedThreshold = 0.005;
            BoardAdded.Pin3.AnalogVoltageChanged += Pin3_AnalogVoltageChanged;

            while (true)
            {
                BoardAdded.Pin1.ToggleOutput();
                await Task.Delay( TimeSpan.FromSeconds(1) );
            }

        }

        private void Pin3_AnalogVoltageChanged(Pin sender, double voltage)
        {
            Debug.WriteLine("Anal - og value changed to : " + voltage + " on pin " + sender.PinNumber);
        }

        private void Pin3_AnalogValueChanged(Pin sender, int value)
        {
         //   Debug.WriteLine("Anal - og value changed to : " + value + " on pin " + sender.PinNumber);
        }

        private void Pin2_DigitalValueChanged(Pin sender, bool value)
        {
            Debug.WriteLine("We have a new value here bruh: " + value);
        }
    }
}
