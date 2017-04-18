using FirstFloor.ModernUI.Windows.Controls;
using Syncfusion.UI.Xaml.Charts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using ToggleSwitch;
namespace TreehopperDAQ
{
    public partial class MainWindow : ModernWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            int numPins = 20;
            // programmatically create the chart series so we don't copy-and-paste
            for (int i = 0; i < numPins; i++)
            {
                channelGrid.RowDefinitions.Add(new RowDefinition() { Height=new GridLength(1, GridUnitType.Star)});


                var tb = new TextBlock() { Text = i.ToString(), HorizontalAlignment=HorizontalAlignment.Left};
                TextOptions.SetTextFormattingMode(tb, TextFormattingMode.Ideal);
                var vb = new Viewbox() { Width = 70, StretchDirection = StretchDirection.Both, Stretch = Stretch.Uniform };
                vb.Child = tb;

                var toggle = new HorizontalToggleSwitch() { };

                var toggleBinding = new Binding();
                toggleBinding.Source = DataContext;
                toggleBinding.Path = new PropertyPath("ChannelEnabled["+i+"]");
                toggleBinding.Mode = BindingMode.TwoWay;

                BindingOperations.SetBinding(toggle, ToggleSwitchBase.IsCheckedProperty, toggleBinding);

                var sp = new StackPanel() { Orientation = Orientation.Horizontal };
                sp.Children.Add(vb);
                sp.Children.Add(toggle);

                channelGrid.Children.Add(sp);

                Grid.SetRow(sp, i);

                var row = new ChartRowDefinition();
                row.BorderThickness = 5;
                row.BorderStroke = new SolidColorBrush(Color.FromArgb(0xff, 0x6C, 0x6C, 0x6C));
                chart.RowDefinitions.Add(row);

                var series = new FastLineSeries();

                var binding = new Binding();
                binding.Source = DataContext;
                binding.Path = new PropertyPath("Data");
                binding.Mode = BindingMode.OneWay;
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                BindingOperations.SetBinding(series, ChartSeriesBase.ItemsSourceProperty, binding);

                
                series.XBindingPath = "TimestampOffset";
                series.YBindingPath = "Values[" + i + "]";
                var axis = new NumericalAxis();
                axis.TickLineSize = 0;
                axis.LabelFormat = " ";
                axis.ShowGridLines = false;
                axis.Minimum = 0;
                axis.Maximum = 3.3;
                axis.PlotOffset = 10;
                series.YAxis = axis;
                chart.Series.Add(series);
                ChartBase.SetRow(axis, numPins - 1 - i); // rows start from the bottom
            }

            channelGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(35, GridUnitType.Pixel) });

        }
    }
}
