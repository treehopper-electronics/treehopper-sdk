using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Media3D;
using Treehopper.Libraries.Sensors.Inertial;
using Treehopper.Mvvm.ViewModels;

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
        public double Yaw { get; set; }

        public bool EnableYaw { get; set; } = true;
        public MatrixTransform3D Transform { get; set; } = new MatrixTransform3D();

        ComplementaryFilter filter;

        public MainWindow()
        {
            InitializeComponent();
            Selector = new SelectorViewModel();
            DataContext = this;

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
            var imu = new Mpu9250(e.Board.I2c);
            imu.EnableMagnetometer = false;
            await imu.Calibrate();

            filter = new ComplementaryFilter(imu, imu, 5, true);
            filter.FilterUpdate += Filter_FilterUpdate;
        }

        private void Filter_FilterUpdate(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                Pitch = filter.Pitch;
                Roll = filter.Roll;
                if (EnableYaw)
                    Yaw = filter.Yaw;
                else
                    Yaw = 0;

                var pitchRotation = new Quaternion(new Vector3D(1, 0, 0), -Pitch);
                var rollRotation = new Quaternion(new Vector3D(0, 0, 1), Roll);
                var yawRotation = new Quaternion(new Vector3D(0, 1, 0), -Yaw);
                var quat = Quaternion.Multiply(Quaternion.Multiply(pitchRotation, rollRotation), yawRotation);
                var transform = new QuaternionRotation3D(quat);
                var rot = new RotateTransform3D(transform);
                Transform = new MatrixTransform3D(rot.Value);



                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Pitch"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Roll"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Yaw"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Transform"));
            }));
        }
    }
}
