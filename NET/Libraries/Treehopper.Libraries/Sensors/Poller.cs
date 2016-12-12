using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



namespace Treehopper.Libraries.Sensors
{
    public delegate void SensorValueChanged(object sender, EventArgs e);
    public class Poller<TPollable> : INotifyPropertyChanged, IDisposable where TPollable : IPollable
    {
        private int sampleDelay;
        private TPollable sensor;
        Task sampleTask;
        bool isRunning = true;
        public event SensorValueChanged OnSensorValueChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        Stopwatch sw = new Stopwatch();

        public Poller(TPollable sensor, int sampleDelayMs = 10, bool usePerformanceTimer = false)
        {
            this.sensor = sensor;
            this.sampleDelay = sampleDelayMs;

            sensor.AutoUpdateWhenPropertyRead = false;

            sampleTask = new Task(async() =>
            {
                while(isRunning)
                {
                    await sensor.Update();
                    OnSensorValueChanged?.Invoke(this, new EventArgs());
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Sensor"));
                    if(usePerformanceTimer)
                    {
                        sw.Restart();
                        while (sw.ElapsedMilliseconds < sampleDelayMs);
                        sw.Stop();
                    } else
                    {
                        await Task.Delay(sampleDelayMs);
                    }
                    
                }
                
            });

            sampleTask.Start();
        }

        public TPollable Sensor { get { return sensor; } }

        ~Poller()
        {
            Dispose();
        }

        public void Dispose()
        {
            isRunning = false;
            sampleTask.Wait();
        }
    }
}
