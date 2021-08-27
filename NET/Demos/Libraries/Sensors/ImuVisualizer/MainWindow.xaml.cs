using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using Treehopper;
using Treehopper.Libraries.Sensors.Inertial;

namespace ImuVisualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow, INotifyPropertyChanged
    {

        public double Pitch { get; set; }
        public double Roll { get; set; }
        public double Yaw { get; set; }

        public bool EnableYaw { get; set; } = true;
        public MatrixTransform3D Transform { get; set; } = new MatrixTransform3D();

        ComplementaryFilter filter;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Task.Run(async () =>
            {
                var board = await ConnectionService.Instance.GetFirstDeviceAsync();
                await board.ConnectAsync();
                var imu = (await Mpu9250.ProbeAsync(board.I2c))[0];
                imu.EnableMagnetometer = false;
                await imu.Calibrate();

                filter = new ComplementaryFilter(imu, imu, 5, true);
                filter.FilterUpdate += Filter_FilterUpdate;
            });
        }


        public event PropertyChangedEventHandler PropertyChanged;

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
