using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Treehopper.Libraries.Utilities;

namespace Treehopper.Libraries.IO.PortExpander
{
    /// <summary>
    ///     NXP PCA9632 4 channel, 8-bit PWM driver
    /// </summary>
    [Supports("NXP", "PCA9632")]
    public class Pca9632
    {
        /// <summary>
        ///     Output drive modes
        /// </summary>
        public enum OutputDriveMode
        {
            /// <summary>
            ///     Open-drain output
            /// </summary>
            OpenDrain,

            /// <summary>
            ///     Totem-pole (push-pull) output
            /// </summary>
            TotemPole
        }

        private readonly SMBusDevice dev;
        private readonly LedOut ledOut;
        private readonly Mode1 mode1;
        private Mode2 mode2;

        public Pca9632(I2C i2c)
        {
            dev = new SMBusDevice(0x62, i2c);

            mode1.Ai = RegisterAutoIncrement.AutoIncrementAllRegisters;
            mode1.Sleep = false;
            dev.WriteByteData((byte) Registers.Mode1, mode1.ToByte()).Wait();

            // clear PWM values
            dev.WriteBufferData((byte) Registers.Pwm0 | 0x80, new byte[4] {0x00, 0x00, 0x00, 0x00}).Wait();

            ledOut.Ldr0 = LedOutputState.LedPwm;
            ledOut.Ldr1 = LedOutputState.LedPwm;
            ledOut.Ldr2 = LedOutputState.LedPwm;
            ledOut.Ldr3 = LedOutputState.LedPwm;
            dev.WriteByteData((byte) Registers.LedOut, ledOut.ToByte()).Wait();

            Pins.Add(new Pin(this, 0));
            Pins.Add(new Pin(this, 1));
            Pins.Add(new Pin(this, 2));
            Pins.Add(new Pin(this, 3));
        }

        public List<Pin> Pins { get; } = new List<Pin>();

        /// <summary>
        ///     Invert the output
        /// </summary>
        public bool InvertOutput
        {
            get { return mode2.InvertOutput; }
            set
            {
                if (mode2.InvertOutput == value) return;
                mode2.InvertOutput = value;
                updateMode2().Wait();
            }
        }

        /// <summary>
        ///     Gets or sets the output drive mode
        /// </summary>
        public OutputDriveMode OutputDrive
        {
            get { return mode2.OutputDrive; }
            set
            {
                if (mode2.OutputDrive == value) return;
                mode2.OutputDrive = value;
                updateMode2().Wait();
            }
        }

        private async Task updateMode2()
        {
            await dev.WriteByteData((byte) Registers.Mode2, mode2.ToByte());
        }

        public Task SetOutputs(byte[] data)
        {
            if (data.Length != 4)
                throw new Exception("Data must be 4 bytes");
            return dev.WriteBufferData((byte) Registers.Pwm0 | 0x80, data);
        }

        private void Update(Pin pin)
        {
            dev.WriteByteData((byte) ((byte) Registers.Pwm0 + pin.pinNumber), (byte) Math.Round(255.0 * pin.DutyCycle))
                .Wait();
        }

        private enum Registers
        {
            Mode1,
            Mode2,
            Pwm0,
            Pwm1,
            Pwm2,
            Pwm3,
            GrpPwm,
            GrpFreq,
            LedOut,
            SubAdr1,
            SubAdr2,
            SubAdr3,
            AllCallAdr
        }

        private enum RegisterAutoIncrement
        {
            NoAutoIncrement = 0x00,
            AutoIncrementAllRegisters = 0x04,
            AutoIncrementIndividualBrightness = 0x05,
            AutoIncrementGlobalControlRegisters = 0x06,
            AutoIncrementIndividualAndGlobal = 0x07
        }

        private enum LedOutputState
        {
            LedOff,
            LedOn,
            LedPwm,
            LedPwmPlusGrpPwm
        }

        private struct Mode1
        {
            [Bitfield(1)] public bool AllCall;
            [Bitfield(1)] public bool Sub3;
            [Bitfield(1)] public bool Sub2;
            [Bitfield(1)] public bool Sub1;
            [Bitfield(1)] public bool Sleep;
            [Bitfield(3)] public RegisterAutoIncrement Ai;
        }

        private struct Mode2
        {
            [Bitfield(2)] public int OutNe;
            [Bitfield(1)] public OutputDriveMode OutputDrive;
            [Bitfield(1)] public bool OutputChangeOnAck;
            [Bitfield(1)] public bool InvertOutput;
            [Bitfield(1)] public bool GroupControlBlinking;
        }

        private struct LedOut
        {
            [Bitfield(2)] public LedOutputState Ldr0;
            [Bitfield(2)] public LedOutputState Ldr1;
            [Bitfield(2)] public LedOutputState Ldr2;
            [Bitfield(2)] public LedOutputState Ldr3;
        }

        /// <summary>
        ///     PCA9685 pin
        /// </summary>
        public class Pin : Pwm
        {
            private readonly Pca9632 driver;

            private double dutyCycle;

            internal int pinNumber;

            internal Pin(Pca9632 driver, int pinNumber)
            {
                this.driver = driver;
                this.pinNumber = pinNumber;
            }

            /// <summary>
            ///     Gets or sets the duty cycle of this pin
            /// </summary>
            public double DutyCycle
            {
                get { return dutyCycle; }
                set
                {
                    dutyCycle = value;
                    driver.Update(this);
                }
            }

            /// <summary>
            ///     Whether the PWM pin is enabled. Writes to this value are ignored, and will always read as "true"
            /// </summary>
            public bool Enabled
            {
                get { return true; }
                set { }
            }

            /// <summary>
            ///     Gets or sets the pulse width, in microseconds.
            /// </summary>
            public double PulseWidth
            {
                get { return DutyCycle * 1000000 / 15625; }
                set { DutyCycle = value * 15625 / 1000000.0; }
            }

            public Task EnablePwm()
            {
                return Task.CompletedTask;
            }

            public Task DisablePwm()
            {
                return Task.CompletedTask;
            }
        }
    }
}