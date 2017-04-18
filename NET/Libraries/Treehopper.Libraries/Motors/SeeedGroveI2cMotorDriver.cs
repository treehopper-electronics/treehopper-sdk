namespace Treehopper.Libraries.Motors
{
    using System;
    using System.Threading.Tasks;
    using Treehopper.Utilities;

    /// <summary>
    /// Seeed Grove I2c Motor Driver
    /// </summary>
    public class SeeedGroveI2cMotorDriver
    {
        enum Registers
        {
            MotorSpeedSet = 0x82,
            PwmFrequencySet = 0x84,
            DirectionSet = 0xaa,
            MotorSetA = 0xa1,
            MotorSetB = 0xa5,
            Nothing = 0x01
        }

        enum MotorSetDirection
        {
            BothClockwise = 0x0a,
            BothCounterClockwise = 0x05,
            Motor1ClockwiseMotor2CounterClockwise = 0x06,
            Motor1CounterClockwiseMotor2Clockwise = 0x09
        }

        /// <summary>
        /// The prescaler frequencies
        /// </summary>
        public enum PrescalerFrequency
        {
            Freq_31372Hz = 0x01,
            Freq_3921Hz = 0x02,
            Freq_490Hz = 0x03,
            Freq_122Hz = 0x04,
            Freq_30Hz = 0x05
        }

        /// <summary>
        /// Construct a new Seeed Grove I2c Motor Driver with the given address DIP switch pins
        /// </summary>
        /// <param name="i2c">I2c module to use</param>
        /// <param name="a1">A1</param>
        /// <param name="a2">A2</param>
        /// <param name="a3">A3</param>
        /// <param name="a4">A4</param>
        public SeeedGroveI2cMotorDriver(I2c i2c, bool a1, bool a2, bool a3, bool a4) : this(i2c, (byte)((a1 ? 1 : 0 << 3) | (a2 ? 1 : 0 << 2) | (a3 ? 1 : 0 << 1) | (a4 ? 1 : 0 << 0)))
        {
            
        }

        /// <summary>
        /// Construct a new Seeed Grove I2c Motor Driver with a given address
        /// </summary>
        /// <param name="i2c">I2c module to use</param>
        /// <param name="address">The address to use</param>
        public SeeedGroveI2cMotorDriver(I2c i2c, byte address = 0x0f)
        {
            dev = new SMBusDevice((byte)(address), i2c);
            Frequency = PrescalerFrequency.Freq_3921Hz;
        }

        private double m1_speed = 0;
        private double m2_speed = 0;
        private PrescalerFrequency frequency;
        private SMBusDevice dev;

        /// <summary>
        /// Gets or sets the driver frequency
        /// </summary>
        public PrescalerFrequency Frequency
        {
            get
            {
                return frequency;
            }
            set
            {
                frequency = value;
                dev.WriteBufferData((byte)Registers.PwmFrequencySet, new byte[] { (byte)frequency, 0x01 }).Wait();
            }
        }

        /// <summary>
        /// Gets or sets the speed for Motor1
        /// </summary>
        public double Motor1Speed
        {
            get
            {
                return m1_speed;
            }

            set
            {
                if (value > 1.0 || value < -1.0)
                    Utility.Error("Speed must be between -1.0 and 1.0");
                if (m1_speed.CloseTo(value)) return;
                m1_speed = value;

                Update().Wait();
            }
        }

        /// <summary>
        /// Gets or sets the speed for Motor2
        /// </summary>
        public double Motor2Speed
        {
            get
            {
                return m2_speed;
            }

            set
            {
                if (value > 1.0 || value < -1.0)
                    Utility.Error("Speed must be between -1.0 and 1.0");
                if (m2_speed.CloseTo(value)) return;
                m2_speed = value;

                Update().Wait();
            }
        }

        private async Task Update()
        {
            byte m1speed = (byte)Math.Round(Math.Abs(m1_speed) * 255);
            byte m2speed = (byte)Math.Round(Math.Abs(m2_speed) * 255);
            await dev.WriteBufferData((byte)Registers.MotorSpeedSet, new byte[] { m1speed, m2speed }).ConfigureAwait(false);

            await Task.Delay(10);

            if (m1_speed >= 0 && m2_speed >= 0)
                await dev.WriteBufferData((byte)Registers.DirectionSet, new byte[] { (byte)MotorSetDirection.BothClockwise, 0x01 }).ConfigureAwait(false);
            else if (m1_speed >= 0 && m2_speed < 0)
                await dev.WriteBufferData((byte)Registers.DirectionSet, new byte[] { (byte)MotorSetDirection.Motor1ClockwiseMotor2CounterClockwise, 0x01 }).ConfigureAwait(false);
            else if (m1_speed < 0 && m2_speed < 0)
                await dev.WriteBufferData((byte)Registers.DirectionSet, new byte[] { (byte)MotorSetDirection.BothCounterClockwise, 0x01 }).ConfigureAwait(false);
            else if (m1_speed < 0 && m2_speed >= 0)
                await dev.WriteBufferData((byte)Registers.DirectionSet, new byte[] { (byte)MotorSetDirection.Motor1CounterClockwiseMotor2Clockwise, 0x01 }).ConfigureAwait(false);
        }
    }
}
