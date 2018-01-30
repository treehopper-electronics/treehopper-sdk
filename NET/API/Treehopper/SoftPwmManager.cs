using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Treehopper.ThirdParty;
using Treehopper.Utilities;

namespace Treehopper
{
    /// <summary>
    ///     The SoftPwmManager is used to configure soft-PWM pins on the board
    /// </summary>
    internal class SoftPwmManager : IDisposable
    {
        private readonly TreehopperUsb _board;
        private readonly AsyncLock _mutex = new AsyncLock();
        private readonly Dictionary<int, SoftPwmPinConfig> _pins;
        private readonly double _resolution = 0.25; // 0.25 microseconds / tick

        internal SoftPwmManager(TreehopperUsb board)
        {
            _board = board;
            _pins = new Dictionary<int, SoftPwmPinConfig>();
        }

        public void Dispose()
        {
            Stop();
        }

        ~SoftPwmManager()
        {
            Stop();
        }

        public override string ToString()
        {
            if (_pins.Count > 1)
                return $"{_pins.Count} SoftPwm pins running";
            if (_pins.Count == 1)
                return "1 SoftPwm pin running";
            return "No SoftPwm pins running";
        }

        internal Task Stop()
        {
            foreach (var entry in _pins)
                entry.Value.Pin.Mode = PinMode.DigitalInput;

            _pins.Clear();
            return UpdateConfig();
        }

        internal Task StartPin(Pin pin)
        {
            if (_pins.ContainsKey(pin.PinNumber))
                return Task.CompletedTask;
            _pins.Add(pin.PinNumber, new SoftPwmPinConfig {Pin = pin, PulseWidthUs = 0, UsePulseWidth = true});
            return UpdateConfig();
        }

        internal Task StopPin(Pin pin)
        {
            _pins.Remove(pin.PinNumber);
            return UpdateConfig();
        }

        internal async void SetDutyCycle(Pin pin, double dutyCycle)
        {
            if (dutyCycle > 1.0 || dutyCycle < 0.0)
                Utility.Error("DutyCycle must be between 0.0 and 1.0");

            using (await _mutex.LockAsync().ConfigureAwait(false))
            {
                _pins[pin.PinNumber].DutyCycle = dutyCycle;
                _pins[pin.PinNumber].UsePulseWidth = false;
                await UpdateConfig().ConfigureAwait(false);
            }
        }

        internal async void SetPulseWidth(Pin pin, double pulseWidth)
        {
            if (pulseWidth > 16409 || pulseWidth < 0.0)
                Utility.Error("PulseWidth must be between 0 and 16409");

            using (await _mutex.LockAsync().ConfigureAwait(false))
            {
                _pins[pin.PinNumber].PulseWidthUs = pulseWidth;
                _pins[pin.PinNumber].UsePulseWidth = true;
                await UpdateConfig().ConfigureAwait(false);
            }
        }

        internal double GetDutyCycle(Pin pin)
        {
            if (!_pins.ContainsKey(pin.PinNumber)) return 0;
            return _pins[pin.PinNumber].DutyCycle;
        }

        internal double GetPulseWidth(Pin pin)
        {
            if (!_pins.ContainsKey(pin.PinNumber)) return 0;
            return _pins[pin.PinNumber].PulseWidthUs;
        }

        private Task UpdateConfig()
        {
            if (_pins.Count > 0)
            {
                foreach (var entry in _pins)
                {
                    // for pins that use pulse width, calculate value based on resolution
                    if (entry.Value.UsePulseWidth)
                    {
                        entry.Value.Ticks = (ushort)(entry.Value.PulseWidthUs / _resolution);

                        // just in case the user wants to retrieve duty cycle, update its value, too
                        entry.Value.DutyCycle = entry.Value.Ticks / 65535d;
                    }
                    else
                    {
                        // for pins that use duty-cycle, calculate based on period count
                        entry.Value.Ticks = (ushort)Math.Round(entry.Value.DutyCycle * 65535);

                        // just in case the user wants to retrieve pulse width, update its value too
                        entry.Value.PulseWidthUs = (int)(entry.Value.Ticks * _resolution);
                    }
                }
                    

                // now the fun part; let's figure out the delta delays between each pin
                var orderedValues = _pins.Values.OrderBy(pin => pin.Ticks);

                var list = orderedValues.ToList();

                var count = list.Count + 1;
                var config = new byte[2 + 3 * count]; // { , (byte)pins.Count, timerVal };
                config[0] = (byte) DeviceCommands.SoftPwmConfig;
                config[1] = (byte) count;
                if (count > 1)
                {
                    var i = 2;
                    var time = 0;

                    for (var j = 0; j < count; j++)
                    {
                        int ticks;

                        if (j < list.Count)
                            ticks = list[j].Ticks - time;
                        else
                            ticks = ushort.MaxValue - time;

                        var tmrVal = ushort.MaxValue - ticks;
                        if (j == 0)
                            config[i++] = 0;
                        else
                            config[i++] = (byte) list[j - 1].Pin.PinNumber;

                        config[i++] = (byte) (tmrVal >> 8);
                        config[i++] = (byte) (tmrVal & 0xff);
                        time += ticks;
                    }
                }
                else
                {
                    config[1] = 0;
                }

                return _board.SendPeripheralConfigPacket(config);
            }
            else
            {
                // disable SoftPWM
                return _board.SendPeripheralConfigPacket(new byte[] {(byte) DeviceCommands.SoftPwmConfig, 0});
            }
        }
    }
}