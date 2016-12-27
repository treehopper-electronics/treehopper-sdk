using System;
using System.Collections.Generic;
using System.Linq;
using Treehopper.ThirdParty;

namespace Treehopper
{
    /// <summary>
    /// The SoftPwmManager is used to configure soft-PWM pins on the board
    /// </summary>
    internal class SoftPwmManager : IDisposable
    {
        AsyncLock mutex = new AsyncLock();

        private Dictionary<int, SoftPwmPinConfig> pins;

        private double resolution = 0.25; // 0.25 microseconds / tick
        TreehopperUsb board;

        internal SoftPwmManager(TreehopperUsb board)
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
                entry.Value.Pin.Mode = PinMode.DigitalInput;
            }
            pins.Clear();
            UpdateConfig();
        }

        internal void StartPin(Pin pin)
        {
            if (pins.ContainsKey(pin.PinNumber))
                return;
            pins.Add(pin.PinNumber, new SoftPwmPinConfig() { Pin = pin, PulseWidthUs = 0, UsePulseWidth = true });
            UpdateConfig();
        }

        internal void StopPin(Pin pin)
        {
            pins.Remove(pin.PinNumber);
            UpdateConfig();
        }

        private void UpdateConfig()
        {
                if (pins.Count > 0)
                {
                    foreach (KeyValuePair<int, SoftPwmPinConfig> entry in pins)
                    {
                        // for pins that use pulse width, calculate value based on resolution
                        if (entry.Value.UsePulseWidth)
                        {
                            entry.Value.Ticks = (UInt16)(entry.Value.PulseWidthUs / resolution);
                            // just in case the user wants to retrieve duty cycle, update its value, too
                            entry.Value.DutyCycle = entry.Value.Ticks / 65535;
                        }
                        else // for pins that use duty-cycle, calculate based on period count
                        {
                            entry.Value.Ticks = (UInt16)Math.Round(entry.Value.DutyCycle * 65535);
                            // just in case the user wants to retrieve pulse width, update its value too
                            entry.Value.PulseWidthUs = (int)(entry.Value.Ticks * resolution);
                        }
                    }

                    // now the fun part; let's figure out the delta delays between each pin
                    var orderedValues = pins.Values
                        //.Where((pin) => pin.Ticks != 0 && pin.Ticks != ushort.MaxValue)
                        .OrderBy((pin) => pin.Ticks);

                    var list = orderedValues.ToList();

                    int count = list.Count() + 1;
                    byte[] config = new byte[2 + 3 * count]; // { , (byte)pins.Count, timerVal };
                    config[0] = (byte)DeviceCommands.SoftPwmConfig;
                    config[1] = (byte)count;
                    if (count > 1)
                    {
                        int i = 2;

                        int time = 0;

                        for (int j = 0; j < count; j++)
                        {
                            int ticks = 0;

                            if (j < list.Count())
                                ticks = list[j].Ticks - time;
                            else
                                ticks = UInt16.MaxValue - time;

                            int TmrVal = UInt16.MaxValue - ticks;
                            if (j == 0)
                                config[i++] = (byte)0;
                            else
                                config[i++] = (byte)list[j - 1].Pin.PinNumber;

                            config[i++] = (byte)(TmrVal >> 8);
                            config[i++] = (byte)(TmrVal & 0xff);
                            time += ticks;
                        }
                    } else
                    {
                        config[1] = 0;
                    }

                    board.sendPeripheralConfigPacket(config);
                }
                else
                {
                    // disable SoftPWM
                    board.sendPeripheralConfigPacket(new byte[] { (byte)DeviceCommands.SoftPwmConfig, 0 });
                }
            

        }

        internal async void SetDutyCycle(Pin pin, double dutyCycle)
        {
            using (await mutex.LockAsync().ConfigureAwait(false))
            {
                pins[pin.PinNumber].DutyCycle = dutyCycle;
                pins[pin.PinNumber].UsePulseWidth = false;
                UpdateConfig();
            }
                
        }

        internal async void SetPulseWidth(Pin pin, double pulseWidth)
        {
            using (await mutex.LockAsync().ConfigureAwait(false))
            {
                pins[pin.PinNumber].PulseWidthUs = pulseWidth;
                pins[pin.PinNumber].UsePulseWidth = true;
                UpdateConfig();
            }
        }

        internal double GetDutyCycle(Pin pin)
        {
            if (!pins.ContainsKey(pin.PinNumber)) return 0;
            return pins[pin.PinNumber].DutyCycle;
        }

        internal double GetPulseWidth(Pin pin)
        {
            if (!pins.ContainsKey(pin.PinNumber)) return 0;
            return pins[pin.PinNumber].PulseWidthUs;

        }

        public void Dispose()
        {
            Stop();
        }
    }


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
        public double PulseWidthUs { get; set; }
        public bool UsePulseWidth { get; set; }
        public UInt16 Ticks { get; set; }
    }
}
