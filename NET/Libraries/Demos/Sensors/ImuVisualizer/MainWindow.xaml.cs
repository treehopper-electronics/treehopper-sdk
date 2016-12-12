using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Treehopper.Libraries.Sensors;
using Treehopper.Libraries.Sensors.Inertial;
using Treehopper.Mvvm.ViewModel;

namespace ImuVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow, INotifyPropertyChanged
    {
        public SelectorViewModel Selector { get; set; }

        public double Pitch { get; set; }
        public double Roll { get; set; }

        public MatrixTransform3D Transform { get; set; } = new MatrixTransform3D();

        ComplementaryFilter filter;

        public MainWindow()
        {
            InitializeComponent();
            Selector = new SelectorViewModel();
            this.DataContext = this;

            Selector.OnBoardConnected += Selector_OnBoardConnected;
            Selector.OnBoardDisconnected += Selector_OnBoardDisconnected;

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Selector_OnBoardDisconnected(object sender, Treehopper.Mvvm.BoardDisconnectedEventArgs e)
        {
            filter.Dispose();
        }

        private async void Selector_OnBoardConnected(object sender, Treehopper.Mvvm.BoardConnectedEventArgs e)
        {
            var imu = new Mpu9150(e.Board.I2c);
            imu.EnableMagnetometer = false;
            await imu.Calibrate();

            filter = new ComplementaryFilter(imu, imu, 5, true);
            filter.FilterUpdate += Filter_FilterUpdate;
        }

        private void Filter_FilterUpdate(object sender, EventArgs e)
        {
            Quaternion corrected = new Quaternion();
            corrected.X = filter.Transform.Y;
            corrected.Y = filter.Transform.Z;
            corrected.Z = -filter.Transform.X;
            corrected.W = filter.Transform.W;

            var transform = new QuaternionRotation3D(corrected);
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                var rot = new RotateTransform3D();
                rot.Rotation = transform;
                Transform = new MatrixTransform3D(rot.Value);

                

                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Transform"));
            }));
        }

        //private void ImuPoller_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
            
        //    //ComplementaryFilter(ImuPoller.Sensor.Accelerometer, ImuPoller.Sensor.Gyroscope);

        //    //var axisX = new Vector3D(1, 0, 0);
        //    //var axisY = new Vector3D(0, 1, 0);
        //    //var axisZ = new Vector3D(0, 0, 1);
        //    //var matrix = Matrix3D.Identity;
        //    //matrix.Rotate(new Quaternion(axisX, Pitch));
        //    //matrix.Rotate(new Quaternion(axisZ, Roll));

        //    //Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
        //    //{
        //    //    Transform = new MatrixTransform3D(matrix);
        //    //    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImuPoller"));
        //    //    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Roll"));
        //    //    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Pitch"));
        //    //    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Transform"));
        //    //}));

        //}

    }
}
