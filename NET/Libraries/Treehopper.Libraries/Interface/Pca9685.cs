using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treehopper.Libraries.Displays;
using Treehopper.Utilities;

namespace Treehopper.Libraries.Interface
{
    /// <summary>
    /// NXP PCA9685 16-channel, 12-bit PWM interface
    /// </summary>
    /// <remarks>
    /// <para>You'll notice this driver lives in <see cref="Treehopper.Libraries.Interface"/>, and not <see cref="Treehopper.Libraries.Displays"/> (as other LED drivers do). While the PCA9685 is advertised as an LED controller, it has several unique features not found in most LED drivers, such as adjustable PWM frequency (via <see cref="Frequency"/>), configurable output drive modes (via <see cref="Pca9685.OutputDrive"/>) supporting push-pull and open-drain outputs, and invertible output logic allowing this driver to be used with both common-anode and common-cathode LED arrangements. The PCA9685 also lacks two important feature found in most LED drivers: programmable constant-current outputs (usually via an external resistor), and built-in high-current open-drain output drivers. As a result, this IC is better suited as a general-purpose PWM-capable GPIO port expander than as an LED driver, both in terms of electronic design and API interfacing design. To this end, this library provides a collection of <see cref="Pwm"/>-capable <see cref="Pins"/>), instead of a collection of <see cref="Treehopper.Libraries.Displays.Led"/>s, found in other LED drivers. You can obtain a collection of <see cref="Treehopper.Libraries.Displays.Led"/>s with the  </para>
    /// </remarks>
    public class Pca9685 : IFlushable
    {
        public Collection<PcaPin> Pins { get; private set; } = new Collection<PcaPin>();
        public Pca9685(I2c port, int speedKHz = 100, bool A0 = false, bool A1 = false, bool A2 = false, bool A3 = false, bool A4 = false, bool A5 = false)
            : this(port, (byte)(0x40 | 
                  (A0 ? 1 : 0) << 0 | 
                  (A1 ? 1 : 0) << 1 |
                  (A2 ? 1 : 0) << 2 |
                  (A3 ? 1 : 0) << 3 |
                  (A4 ? 1 : 0) << 4 |
                  (A5 ? 1 : 0) << 5), speedKHz)
        {
            
        }

        public Pca9685(I2c port, byte address, int speedKHz = 100)
        {
            this.dev = new SMBusDevice(address, port, speedKHz);
            for (int i = 0; i < 16; i++)
            {
                Pins.Add(new PcaPin(this, i));
            }
            updateConfig();
        }

        private OutputDriveMode outputDrive = OutputDriveMode.TotemPole;
        private SMBusDevice dev;
        private double frequency = 100;
        private bool useExternalClock = false;
        private bool invertOutput;

        public IFlushable Parent { get; private set; }

        public bool UseExternalClock
        {
            get { return useExternalClock; }
            set {
                if (useExternalClock == value) return;
                useExternalClock = value;
                updateConfig();
            }
        }



        public bool InvertOutput
        {
            get { return invertOutput; }
            set {
                if (invertOutput == value) return;
                invertOutput = value;
                updateConfig();
            }
        }


        public OutputDriveMode OutputDrive
        {
            get { return outputDrive; }
            set {
                if (outputDrive == value) return;
                outputDrive = value;
                updateConfig();
            }
        }

        public bool AutoFlush { get; set; } = true;

        public double Frequency
        {
            get { return frequency; }
            set {
                if (frequency.CloseTo(value)) return;
                frequency = value;
                updateConfig();
            }
        }

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

        byte[] pinRegisters = new byte[16 * 4];
        public async Task Flush(bool force = false)
        {
            foreach (var pin in Pins)
                setPinValue(pin);

            // send all the registers
            dev.WriteBufferData((byte)Registers.LedOnLowBase, pinRegisters).Wait();
        }

        private void setPinValue(PcaPin pin)
        {
            double dc = pin.DutyCycle;
            int onTicks = (int)Math.Round(dc * 4096);
            if (onTicks == 4096)
            {
                pinRegisters[4 * pin.PinNumber] = 0x00; // ON_L
                pinRegisters[4 * pin.PinNumber + 1] = 0x10; // ON_H
                pinRegisters[4 * pin.PinNumber + 2] = 0x00; // OFF_L
                pinRegisters[4 * pin.PinNumber + 3] = 0x00; // OFF_H
            }
            else if (onTicks == 0)
            {
                pinRegisters[4 * pin.PinNumber] = 0x00; // ON_L
                pinRegisters[4 * pin.PinNumber + 1] = 0x00; // ON_H
                pinRegisters[4 * pin.PinNumber + 2] = 0x00; // OFF_L
                pinRegisters[4 * pin.PinNumber + 3] = 0x10; // OFF_H
            }
            else
            {
                int delayTicks = pin.PinNumber * 4096 / 16; // stagger the outputs to reduce current inrush

                int offTicks;
                if (delayTicks + onTicks < 4096)
                {
                    offTicks = delayTicks + onTicks - 1;
                }
                else
                {
                    offTicks = delayTicks + onTicks - 4096;
                }

                pinRegisters[4*pin.PinNumber] = (byte)(delayTicks & 0xFF);
                pinRegisters[4*pin.PinNumber+1] = (byte)(delayTicks >> 8);
                pinRegisters[4*pin.PinNumber+2] = (byte)(offTicks & 0xFF);
                pinRegisters[4*pin.PinNumber+3] = (byte)(offTicks >> 8);
            }
        }

        internal void Update(PcaPin pin)
        {
            if (!AutoFlush) return;

            setPinValue(pin);
            dev.WriteBufferData((byte)((byte)Registers.LedOnLowBase + (4*pin.PinNumber)), pinRegisters.Skip(4*pin.PinNumber).Take(4).ToArray()).Wait();
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

    public class PcaPin : Pwm
    {
        private Pca9685 driver;
        public int PinNumber { get; private set; }

        public PcaPin(Pca9685 driver, int pinNumber)
        {
            this.driver = driver;
            this.PinNumber = pinNumber;
        }

        private double dutyCycle;

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
