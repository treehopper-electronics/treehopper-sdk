using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Treehopper.Libraries.Sensors.Inertial
{
    public class ComplementaryFilter : IDisposable, INotifyPropertyChanged
    {
        IAccelerometer accel;
        IGyroscope gyro;
        Poller<IAccelerometer> accelPoller;
        Poller<IAccelerometer> gyroPoller;

        public delegate void FilterUpdateEventHandler(object sender, EventArgs e);

        public ComplementaryFilter(IAccelerometer accelerometer, IGyroscope gyroscope, int samplePeriodMs = 10, bool usePerformanceTimer = false)
        {
            this.accel = accelerometer;
            this.gyro = gyroscope;

            accelPoller = new Poller<IAccelerometer>(accelerometer, samplePeriodMs, usePerformanceTimer);
            accelPoller.OnSensorValueChanged += PollerEvent;
            if (accelerometer != gyroscope) // if we have two different sensors
            {
                gyroPoller = new Poller<IAccelerometer>(accelerometer, samplePeriodMs, usePerformanceTimer);
                gyroPoller.OnSensorValueChanged += PollerEvent;
            }
                
        }

        private void PollerEvent(object sender, EventArgs e)
        {
            update();
        }

        Stopwatch sw = new Stopwatch();

        public event PropertyChangedEventHandler PropertyChanged;
        public event FilterUpdateEventHandler FilterUpdate;

        public bool AutoUpdateWhenPropertyRead { get; set; }
        public double Pitch { get; private set; } = 0;
        public double Roll { get; private set; } = 0;
        public double Yaw { get; private set; } = 0;

        public Quaternion RollQuaternion { get; private set; } = new Quaternion();
        public Quaternion PitchQuaternion { get; private set; } = new Quaternion();
        public Quaternion YawQuaternion { get; private set; } = new Quaternion();

        public Quaternion Transform { get; private set; } = new Quaternion();

        void update()
        {
            sw.Stop();
            double dt = sw.Elapsed.TotalSeconds;
            sw.Restart();

            double pitchAcc, rollAcc;

            double accelContrib = 0.15;
            double gyroContrib = 1 - accelContrib;


            // Integrate the gyroscope data -> int(angularSpeed) = angle

            //RollQuaternion = Quaternion.Multiply(RollQuaternion, new Quaternion(Xaxis, gyro.Gyroscope.X * dt));
            //PitchQuaternion = Quaternion.Multiply(PitchQuaternion, new Quaternion(Yaxis, gyro.Gyroscope.Y * dt));

            
            //Roll = RollQuaternion.Angle;
            //if (RollQuaternion.Axis.X == -1)
            //    Roll = -Roll;

            //Pitch = PitchQuaternion.Angle;
            //if (PitchQuaternion.Axis.Y == -1)
            //    Pitch = -Pitch;

            Roll += gyro.Gyroscope.X * dt;
            Pitch += gyro.Gyroscope.Y * dt;
            Yaw += gyro.Gyroscope.Z * dt;

            // Turning around the Y axis results in a vector on the X-axis
            rollAcc = Math.Atan2(-accel.Accelerometer.Y, -accel.Accelerometer.Z) * 180.0 / Math.PI;
            Roll = Roll * gyroContrib + rollAcc * accelContrib;

            // Turning around the X axis results in a vector on the Y-axis
            pitchAcc = Math.Atan2(accel.Accelerometer.X, Math.Sqrt(accel.Accelerometer.Y * accel.Accelerometer.Y + accel.Accelerometer.Z * accel.Accelerometer.Z)) * 180.0 / Math.PI;
            Pitch = Pitch * gyroContrib + pitchAcc * accelContrib;

            //Transform = Quaternion.Multiply(new Quaternion(axisX, Roll), new Quaternion(axisY, Pitch));
            //Transform = Quaternion.Multiply(new Quaternion(Xaxis, Roll), new Quaternion(Yaxis, Pitch));

            //       matrix.Rotate(new Quaternion(axisZ, Yaw));




            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Pitch"));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Roll"));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Yaw"));
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Matrix"));
            this.FilterUpdate?.Invoke(this, new EventArgs());
        }

        Vector3 Xaxis = new Vector3(1, 0, 0);
        Vector3 Yaxis = new Vector3(0, 1, 0);
        Vector3 Zaxis = new Vector3(0, 0, 1);

        public void Dispose()
        {
            accelPoller.Dispose();
            if (gyroPoller != null)
                gyroPoller.Dispose();
        }
    }
}
