namespace Treehopper.Libraries.Interface.PortExpander
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Treehopper.Libraries.Displays;
    using Treehopper.Utilities;

    /// <summary>
    /// NXP PCA9685 16-channel, 12-bit PWM driver
    /// </summary>
    /// <remarks>
    /// <para>You'll notice this driver lives in <see cref="Treehopper.Libraries.Interface"/>, and not <see cref="Treehopper.Libraries.Displays"/> (as other LED drivers do). While the PCA9685 is advertised as an LED controller, it has several unique features not found in most LED drivers, such as adjustable PWM frequency (via <see cref="Frequency"/>), configurable output drive modes (via <see cref="Pca9685.OutputDrive"/>) supporting push-pull and open-drain outputs, and invertible output logic allowing this driver to be used with both common-anode and common-cathode LED arrangements. The PCA9685 also lacks two important feature found in most LED drivers: programmable constant-current outputs (usually via an external resistor), and built-in high-current open-drain output drivers. As a result, this IC is better suited as a general-purpose PWM-capable GPIO port expander than as an LED driver, both in terms of electronic design and API interfacing design. To this end, this library provides a collection of <see cref="Pwm"/>-capable <see cref="Pins"/>), instead of a collection of <see cref="Treehopper.Libraries.Displays.Led"/>s, found in other LED drivers. You can obtain a collection of <see cref="Treehopper.Libraries.Displays.Led"/>s with the  </para>
    /// </remarks>
    public class Pca9685 : IFlushable
    {
        /// <summary>
        /// Construct a new PCA9685
        /// </summary>
        /// <param name="i2c">The i2c port this chip is attached to</param>
        /// <param name="speed">The speed, in kHz, to use with this chip</param>
        /// <param name="a0">The state of the A0 pin</param>
        /// <param name="a1">The state of the A1 pin</param>
        /// <param name="a2">The state of the A2 pin</param>
        /// <param name="a3">The state of the A3 pin</param>
        /// <param name="a4">The state of the A4 pin</param>
        /// <param name="a5">The state of the A5 pin</param>
        public Pca9685(I2c i2c, int speed = 100, bool a0 = false, bool a1 = false, bool a2 = false, bool a3 = false, bool a4 = false, bool a5 = false)
            : this(i2c, (byte)(0x40 | 
                  (a0 ? 1 : 0) << 0 | 
                  (a1 ? 1 : 0) << 1 |
                  (a2 ? 1 : 0) << 2 |
                  (a3 ? 1 : 0) << 3 |
                  (a4 ? 1 : 0) << 4 |
                  (a5 ? 1 : 0) << 5), speed)
        {
            
        }

        /// <summary>
        /// Construct a new PCA9685
        /// </summary>
        /// <param name="i2c">The i2c port this chip is attached to</param>
        /// <param name="address">The 7-bit address to use</param>
        /// <param name="speed">The speed, in kHz, to use with this chip</param>
        public Pca9685(I2c i2c, byte address, int speed = 100)
        {
            this.dev = new SMBusDevice(address, i2c, speed);
            for (int i = 0; i < 16; i++)
            {
                Pins.Add(new Pin(this, i));
            }
            updateConfig();
        }

        byte[] pinRegisters = new byte[16 * 4];
        private OutputDriveMode outputDrive = OutputDriveMode.TotemPole;
        private SMBusDevice dev;
        private double frequency = 100;
        private bool useExternalClock = false;
        private bool invertOutput;

        /// <summary>
        /// The collection of PWM-capable pins that belong to this instance
        /// </summary>
        public Collection<Pin> Pins { get; private set; } = new Collection<Pin>();

        /// <summary>
        /// The parent object. Always returns null.
        /// </summary>
        public IFlushable Parent { get { return null; } }

        /// <summary>
        /// Whether to use the external clock
        /// </summary>
        public bool UseExternalClock
        {
            get { return useExternalClock; }
            set {
                if (useExternalClock == value) return;
                useExternalClock = value;
                updateConfig();
            }
        }

        /// <summary>
        /// Invert the output
        /// </summary>
        public bool InvertOutput
        {
            get { return invertOutput; }
            set {
                if (invertOutput == value) return;
                invertOutput = value;
                updateConfig();
            }
        }

        /// <summary>
        /// Gets or sets the output drive mode
        /// </summary>
        public OutputDriveMode OutputDrive
        {
            get { return outputDrive; }
            set {
                if (outputDrive == value) return;
                outputDrive = value;
                updateConfig();
            }
        }

        /// <summary>
        /// Whether to automatically write updates to the pins immediately
        /// </summary>
        public bool AutoFlush { get; set; } = true;

        /// <summary>
        /// Gets or sets the PWM frequency to use
        /// </summary>
        public double Frequency
        {
            get { return frequency; }
            set {
                if (frequency.CloseTo(value)) return;
                frequency = value;
                updateConfig();
            }
        }

        /// <summary>
        /// Output drive modes
        /// </summary>
        public enum OutputDriveMode
        {
            /// <summary>
            /// Open-drain output
            /// </summary>
            OpenDrain,

            /// <summary>
            /// Totem-pole (push-pull) output
            /// </summary>
            TotemPole
        }

        private void updateConfig()
        {
            // sleep the chip
            dev.WriteByteData((byte)Registers.Mode1, 0x10).Wait();
            // do the updates
            byte mode1 = (byte)(0x30 | ((UseExternalClock ? 1 : 0) << 6));
            dev.WriteByteData((byte)Registers.Mode1, mode1).Wait();

            byte mode2 = (byte)(((int)OutputDrive << 2) | ((InvertOutput ? 1 : 0) << 4));
            dev.WriteByteData((byte)Registers.Mode2, mode2).Wait();

            //int prescaler = (int)Math.Round((25000000 / (4096 * frequency * 0.8963)) - 1);
            int prescaler = (int)Math.Round((6809.68 / frequency) - 1.1157); // weird overshoot issue
            if (prescaler > 255)
            {
                Debug.WriteLine("NOTICE: PCA9685 frequency out of range. Clipping to 24 Hz");
                prescaler = 255;
            }

            dev.WriteByteData((byte)Registers.Prescale, (byte)prescaler).Wait();

            // wake up
            mode1 &= 0xEF; // clear the sleep flag
            dev.WriteByteData((byte)Registers.Mode1, mode1).Wait();
            Flush(true).Wait(); // we have to rewrite the PWM values (why?)
        }

        /// <summary>
        /// Flush the data to the PCA9685
        /// </summary>
        /// <param name="force">Whether to force the update</param>
        /// <returns>An awaitable task</returns>
        public async Task Flush(bool force = false)
        {
            foreach (var pin in Pins)
                setPinValue(pin);

            // send all the registers
            dev.WriteBufferData((byte)Registers.LedOnLowBase, pinRegisters).Wait();
        }

        private void setPinValue(Pin pin)
        {
            double dc = pin.DutyCycle;
            int onTicks = (int)Math.Round(dc * 4096);
            if (onTicks == 4096)
            {
                pinRegisters[4 * pin.pinNumber] = 0x00; // ON_L
                pinRegisters[4 * pin.pinNumber + 1] = 0x10; // ON_H
                pinRegisters[4 * pin.pinNumber + 2] = 0x00; // OFF_L
                pinRegisters[4 * pin.pinNumber + 3] = 0x00; // OFF_H
            }
            else if (onTicks == 0)
            {
                pinRegisters[4 * pin.pinNumber] = 0x00; // ON_L
                pinRegisters[4 * pin.pinNumber + 1] = 0x00; // ON_H
                pinRegisters[4 * pin.pinNumber + 2] = 0x00; // OFF_L
                pinRegisters[4 * pin.pinNumber + 3] = 0x10; // OFF_H
            }
            else
            {
                int delayTicks = pin.pinNumber * 4096 / 16; // stagger the outputs to reduce current inrush

                int offTicks;
                if (delayTicks + onTicks < 4096)
                {
                    offTicks = delayTicks + onTicks - 1;
                }
                else
                {
                    offTicks = delayTicks + onTicks - 4096;
                }

                pinRegisters[4*pin.pinNumber] = (byte)(delayTicks & 0xFF);
                pinRegisters[4*pin.pinNumber+1] = (byte)(delayTicks >> 8);
                pinRegisters[4*pin.pinNumber+2] = (byte)(offTicks & 0xFF);
                pinRegisters[4*pin.pinNumber+3] = (byte)(offTicks >> 8);
            }
        }

        internal void Update(Pin pin)
        {
            if (!AutoFlush) return;

            setPinValue(pin);
            dev.WriteBufferData((byte)((byte)Registers.LedOnLowBase + (4*pin.pinNumber)), pinRegisters.Skip(4*pin.pinNumber).Take(4).ToArray()).Wait();
        }

        private enum Registers
        {
            Mode1 = 0x00,
            Mode2 = 0x01,
            SubAdr1 = 0x02,
            SubAdr2 = 0x03,
            SubAdr3 = 0x04,
            AllCallAdr = 0x05,
            LedOnLowBase = 0x06,
            LedOnHighBase = 0x07,
            LedOffLowBase = 0x08,
            LedOffHighBase = 0x09,
            AllLedOnLow = 0xFA,
            AllLedOnHigh = 0xFB,
            AllLedOffLow = 0xFC,
            AllLedOffHigh = 0xFD,
            Prescale = 0xFE,
            TestMode = 0xFF
        }
    }

    /// <summary>
    /// PCA9685 pin
    /// </summary>
    public class Pin : Pwm
    {
        private Pca9685 driver;

        internal int pinNumber;

        internal Pin(Pca9685 driver, int pinNumber)
        {
            this.driver = driver;
            this.pinNumber = pinNumber;
        }

        private double dutyCycle;

        /// <summary>
        /// Gets or sets the duty cycle of this pin
        /// </summary>
        public double DutyCycle
        {
            get { return dutyCycle; }
            set {
                dutyCycle = value;
                driver.Update(this);
            }
        }

        /// <summary>
        /// Whether the PWM pin is enabled. Writes to this value are ignored, and will always read as "true"
        /// </summary>
        public bool Enabled { get { return true; } set { } }

        /// <summary>
        /// Gets or sets the pulse width, in microseconds.
        /// </summary>
        /// <remarks>
        /// This property will read the <see cref="Pca9685.Frequency"/> of the driver and change the <see cref="DutyCycle"/> property of this pin to set the desired pulse width. The accuracy of this property is dependent on the oscillator source; the PCA9685 internal oscillator is not nearly as accurate as an external crystal oscillator, so if you need accurate pulse width control, consider using an external oscillator.
        /// </remarks>
        public double PulseWidth
        {
            get {
                return DutyCycle * 1000000/(driver.Frequency);
            }
            set {
                DutyCycle = value * driver.Frequency / 1000000.0;
            }
        }
    }
}
