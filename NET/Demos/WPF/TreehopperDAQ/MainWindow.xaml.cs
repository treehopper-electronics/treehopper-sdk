using FirstFloor.ModernUI.Windows.Controls;
using Syncfusion.UI.Xaml.Charts;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

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

                BindingOperations.SetBinding(series, FastScatterBitmapSeries.ItemsSourceProperty, binding);

                
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
                SfChart.SetRow(axis, numPins - 1 - i); // rows start from the bottom
            }

        }
    }
}
