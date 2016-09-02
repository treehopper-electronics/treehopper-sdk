using System;
using System.Collections.Generic;

namespace Treehopper
{
    internal class SoftPwmPinConfig
    {
        public Pin Pin { get; set; }
        /// <summary>
        /// Duty Cycle, from 0 to 1.
        /// </summary>
        public double DutyCycle { get; set; }
        /// <summary>
        /// Pulse Width, in Milliseconds
        /// </summary>
        public double PulseWidth { get; set; }
        public bool UsePulseWidth { get; set; }
        public ushort ActualCount { get; set; }
    }

    internal class SoftPwmManager : IDisposable
    {
        private Dictionary<int, SoftPwmPinConfig> pins;

        private byte timerVal = 0;
        /// <summary>
        /// Period, in ticks (determined by resolution)
        /// </summary>
        private ushort period = 233; // 100 Hz rate
        /// <summary>
        /// Resolution, in microseconds
        /// </summary>
        private int resolution = 43;
        TreehopperBoard board;

        internal SoftPwmManager(TreehopperBoard board)
        {
            this.board = board;
            pins = new Dictionary<int, SoftPwmPinConfig>();
        }
        
        ~SoftPwmManager()
        {
            Stop();
        }

        internal void Stop()
        {
            foreach (KeyValuePair<int, SoftPwmPinConfig> entry in pins)
            {
                entry.Value.Pin.MakeDigitalInput();
            }
            pins.Clear();
            UpdateConfig();
        }

        internal void StartPin(Pin pin)
        {
            if (pins.ContainsKey(pin.PinNumber))
                return;
            pin.MakeDigitalOutput();
            pins.Add(pin.PinNumber, new SoftPwmPinConfig() { Pin = pin, PulseWidth = 0, UsePulseWidth = true });
            UpdateConfig();
        }

        internal void StopPin(Pin pin)
        {
            pins.Remove(pin.PinNumber);
            UpdateConfig();
            pin.MakeDigitalInput();
        }

        private void UpdateConfig()
        {
            // timerVal = 0 --> 54.25 us
            // timerVal = 50 --> 45.8 us
            // timerVal = 100 --> 37.5 us
            // timerVal = 150 --> 29.17 us
            // timerVal = 200 --> 20.83 us
            if(pins.Count > 0)
            { 
                double timer = Math.Round(268.0934 - 6.22568 * this.resolution);
            
                if (timerVal < 0)
                    throw new Exception("Bad timer config");
            
                timerVal = (byte)timer;
            
                byte[] periodBytes = BitConverter.GetBytes(period);
            
                byte[] config = new byte[5+pins.Count]; // { , (byte)pins.Count, timerVal };
                config[0] = (byte)DeviceCommands.SoftPwmConfig;
                config[1] = (byte)pins.Count;
                config[2] = (byte)timerVal;
                periodBytes.CopyTo(config, 3);
                int i = 5;
                foreach (KeyValuePair<int, SoftPwmPinConfig> entry in pins)
                {
                    config[i++] = (byte)entry.Key;
                }
                board.sendCommsConfigPacket(config);
            }
            else
            {
                // turn off the SoftPWM interrupt
                board.sendCommsConfigPacket(new byte[] { (byte)DeviceCommands.SoftPwmConfig, 0 });
            }


        }

        internal int Period
        {
            get {
                return period * resolution * 1000;
            }

            set {
                period = (ushort)(value * 1000 / resolution);
                UpdateConfig();
                UpdateDutyCycles();
            }
            
        }

        internal void SetFrequency(double hz)
        {
            int milliseconds = (int)Math.Round(1.0 / hz * 1000.0);
            Period = milliseconds;
        }

        internal void SetResolution(int microseconds)
        {
            if(microseconds < 10)
            {
                throw new Exception("The minimum resolution supported is 10 microseconds");
            } else if(microseconds > 43.0)
            {
                throw new Exception("The maximum resolution supported is 27 microseconds");
            }
            this.resolution = microseconds;
            //timerVal = 0;
            UpdateConfig();
            UpdateDutyCycles();
        }

        private void UpdateDutyCycles()
        {
            byte[] config = new byte[1 + pins.Count*2]; // { , (byte)pins.Count, timerVal };
            config[0] = (byte)DeviceCommands.SoftPwmUpdateDc;
            int i = 1;
            foreach (KeyValuePair<int, SoftPwmPinConfig> entry in pins)
            {
                // for pins that use pulse width, calculate value based on resolution
                if (entry.Value.UsePulseWidth)
                {
                    entry.Value.ActualCount = (ushort)(entry.Value.PulseWidth*1000.0 / resolution);
                    // just in case the user wants to retrieve duty cycle, update its value, too
                    entry.Value.DutyCycle = entry.Value.ActualCount / period;
                }
                else // for pins that use duty-cycle, calculate based on period count
                {
                    entry.Value.ActualCount = (ushort)Math.Round(entry.Value.DutyCycle * period);
                    // just in case the user wants to retrieve pulse width, update its value, too
                    entry.Value.PulseWidth = (double)(entry.Value.ActualCount * resolution) / 1000.0;

                }
                byte[] count = BitConverter.GetBytes(entry.Value.ActualCount);
                count.CopyTo(config, i);
                i = i + 2;
            }

            board.sendCommsConfigPacket(config);
        }

        internal void SetDutyCycle(Pin pin, double dutyCycle)
        {
            pins[pin.PinNumber].DutyCycle = dutyCycle;
            pins[pin.PinNumber].UsePulseWidth = false;
            UpdateDutyCycles();
        }

        internal void SetPulseWidth(Pin pin, double pulseWidth)
        {
            pins[pin.PinNumber].PulseWidth = pulseWidth;
            pins[pin.PinNumber].UsePulseWidth = true;
            UpdateDutyCycles();
        }

        internal double GetDutyCycle(Pin pin)
        {
            return pins[pin.PinNumber].DutyCycle;
        }

        internal double GetPulseWidth(Pin pin)
        {
            return pins[pin.PinNumber].PulseWidth;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
