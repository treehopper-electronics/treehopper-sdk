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
    /// <summary>
    /// Polls an IPollable device at a regular rate
    /// </summary>
    /// <typeparam name="TPollable">The IPollable device to poll</typeparam>
    /// <remarks>
    /// <para>
    /// While simple devices (such as temperature sensors) can simply auto-update whenever the relevant properties are read from, it is often useful to continuously update these values on a background thread so that the <see cref="IPollable"/> device can fire events. This is especially true for GPIO port expanders used as inputs in event-driven programming.
    /// </para>
    /// <para>
    /// Since this class implements INotifyPropertyChanged, WPF and XAML applications can bind directly to properties of the underlying sensor in this class, and GUI controls will be automatically updated
    /// </para>
    /// </remarks>
    public class Poller<TPollable> : INotifyPropertyChanged, IDisposable where TPollable : IPollable
    {
        /// <summary>
        /// The signature of the callback to use with the <see cref="OnNewSensorValue"/> event
        /// </summary>
        /// <param name="sender">The Poller where this call originated from</param>
        /// <param name="e">An empty EventArgs</param>
        public delegate void NewSensorValueEventHandler(object sender, EventArgs e);

        private int sampleDelay;
        private TPollable sensor;
        Task sampleTask;
        bool isRunning = true;
        Stopwatch sw = new Stopwatch();

        /// <summary>
        /// Fires whenever the sensor value is updated
        /// </summary>
        public event NewSensorValueEventHandler OnNewSensorValue;

        /// <summary>
        /// Fires whenever the sensor value is updated
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Construct a new Poller for the given sensor
        /// </summary>
        /// <param name="sensor">The sensor to construct a Poller for</param>
        /// <param name="samplePeriod">The sample period, in milliseconds, to poll this sensor at</param>
        /// <param name="usePrecisionTimer">Whether to use a (CPU-heavy) precision timer</param>
        /// <remarks>
        /// <para>
        /// When <paramref name="usePrecisionTimer"/> is set to true, a busy-wait timer will be used. All things considered, this provides for relatively precise timing (even given USB latencies, there is often less than 100 microseconds of jitter), but this will cause dramatic increases in CPU usage. We recommend enabling this only in scenarios where you absolutely need low-jitter sampling.
        /// </para>
        /// </remarks>
        public Poller(TPollable sensor, int samplePeriod = 10, bool usePrecisionTimer = false)
        {
            this.sensor = sensor;
            sampleDelay = samplePeriod;

            sensor.AutoUpdateWhenPropertyRead = false;

            sampleTask = new Task(async() =>
            {
                while(isRunning)
                {
                    await sensor.Update().ConfigureAwait(false);
                    OnNewSensorValue?.Invoke(this, new EventArgs());
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Sensor"));
                    if(usePrecisionTimer)
                    {
                        sw.Restart();
                        while (sw.ElapsedMilliseconds < samplePeriod);
                        sw.Stop();
                    } else
                    {
                        await Task.Delay(samplePeriod).ConfigureAwait(false);
                    }
                    
                }
                
            });

            sampleTask.Start();
        }

        /// <summary>
        /// The underlying sensor that is polled by this instance
        /// </summary>
        public TPollable Sensor => sensor;

        /// <summary>
        /// Destroy the poller object
        /// </summary>
        ~Poller()
        {
            Dispose();
        }

        /// <summary>
        /// Stops the sampling thread safely and closes resources
        /// </summary>
        public void Dispose()
        {
            isRunning = false;
            sampleTask.Wait();
            // Hack alert: maybe Wait() doesn't guarantee that all awaits in sampleTask are done? 
            // Regardless, if we don't wait "a little longer" then we may exit out of Dispose() before the task is done running. This is problematic if the next line of code in the caller is something like board.Disconnect();
            Task.Delay(sampleDelay * 2).Wait();
        }
    }
}
