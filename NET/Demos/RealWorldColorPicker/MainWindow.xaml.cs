using System;
using System.Windows;
using System.Windows.Media;
using Treehopper;
using Treehopper.Libraries;
namespace Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TreehopperUSB myBoard;
        Pin1 led;
        ColorSensor_ADJDS311 colorSensor;
        Brush brush;

        public MainWindow()
        {
            InitializeComponent();
            TreehopperManager manager = new TreehopperManager();
            manager.BoardAdded += manager_BoardAdded;
        }

        void manager_BoardAdded(object sender, TreehopperUSB board)
        {
            myBoard = board;
            myBoard.Open();
            colorSensor = new ColorSensor_ADJDS311(board.I2C, board.Pin1);
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {

            colorSensor.updateColor();
            // brush = new SolidColorBrush(colorSensor.Color);

            canvas.Background = new SolidColorBrush(Color.FromRgb((byte)(colorSensor.Red/4), (byte)(colorSensor.Green/4), (byte)(colorSensor.Blue/4)));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            myBoard.Close();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Topmost = true;
        }
    }
}
