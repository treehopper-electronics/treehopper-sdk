namespace Treehopper.Libraries.Motors
{
    using System;
    using System.Threading.Tasks;
    using Treehopper;

    /// <summary>
    /// PositionChanged event argument
    /// </summary>
    public class PositionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The new position of the shaft
        /// </summary>
        public short Position;
    }

    /// <summary>
    /// Event handler delegate for PositionChanged events
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void PositionChangedHandler(object sender, PositionChangedEventArgs e);

    /// <summary>
    /// Allegro AMIS30624 I2c stepper motor driver
    /// </summary>
    public class Amis30624
    {
        private readonly SMBusDevice dev;

        /// <summary>
        /// Construct a new AMIS30624 stepper motor controller
        /// </summary>
        /// <param name="module">The I2c module this stepper motor is attached to</param>
        /// <param name="addressPin">The hardwired address pin state</param>
        /// <param name="speed">The speed to operate this peripheral at</param>
        public Amis30624(I2c module, bool addressPin = false, int speed = 400) : this(module, (byte)(addressPin ? 0x61 : 0x60), speed)
        {

        }


        /// <summary>
        /// Construct a new AMIS30624 stepper motor controller
        /// </summary>
        /// <param name="module">The I2c module this stepper motor is attached to</param>
        /// <param name="address">The address of the module</param>
        /// <param name="speed">The speed to operate this peripheral at</param>
        public Amis30624(I2c module, byte address, int speed)
        {
            dev = new SMBusDevice(address, module, speed);
            ResetToDefault().ConfigureAwait(false);
            GetFullStatus().ConfigureAwait(false);
        }

        /// <summary>
        /// An event that fires whenever the stepper motor's position has changed
        /// </summary>
        public event PositionChangedHandler PositionChanged;



        /// <summary>
        /// Reset to default
        /// </summary>
        /// <returns>An awaitable task that completes when finished</returns>
        public Task ResetToDefault()
        {
            return dev.WriteByte((byte)Command.ResetToDefault);
        }

        /// <summary>
        /// Soft stop the motor
        /// </summary>
        /// <returns>An awaitable task that completes when finished</returns>
        public Task SoftStop()
        {
            return dev.WriteByte((byte)Command.SoftStop);
        }

        /// <summary>
        /// Hard (emergency) stop the motor
        /// </summary>
        /// <returns>An awaitable task that completes when finished</returns>
        public Task HardStop()
        {
            return dev.WriteByte((byte)Command.HardStop);
        }

        /// <summary>
        /// Run the motor continuously with the given velocity settings
        /// </summary>
        /// <returns>An awaitable task that completes when finished</returns>
        public Task RunVelocity()
        {
            return dev.WriteByte((byte)Command.RunVelocity);
        }

        private async Task GetFullStatus()
        {
            var data = await dev.ReadBufferData((byte)Command.GetFullStatus1, 9);
            int irun = (data[1] >> 4) & 0x0f;
            runningCurrent = (RunningCurrent)irun;

            int ihold = data[1] & 0x0f;
            holdingCurrent = (HoldingCurrent)irun;

            int vmax = (data[2] >> 4) & 0x0f;
            maxVelocity = (MaxVelocityType)vmax;

            int vmin = data[2] & 0x0f;
            minVelocityFactorThirtySeconds = (vmin == 0) ? 32 : vmin;

            int acc = data[3] & 0x0f;
            acceleration = (Accel)acc;

            accelShape = (data[3] & 0x80) > 0;

            int _stepMode = (data[3] >> 5) & 0x03;
            stepMode = (StepModeType)_stepMode;

            shaftDirection = (data[3] & 0x10) > 0;

            await GetPositionStatus();
        }

        private async Task GetPositionStatus()
        {
            var data = await dev.ReadBufferData((byte)Command.GetFullStatus2, 8);
            ActualPosition = (short)(data[1] << 8 | data[2]);
            targetPosition = (short)(data[3] << 8 | data[4]);
            securePosition = (short)(data[5]);

        }

        private short actualPosition;

        /// <summary>
        /// Get the actual position of the shaft
        /// </summary>
        public short ActualPosition
        {
            get
            {
                return actualPosition;
            }
            private set
            {
                if (actualPosition == value) return;
                actualPosition = value;
                PositionChanged?.Invoke(this, new PositionChangedEventArgs() { Position = actualPosition });
            }
        }

        private short targetPosition;

        /// <summary>
        /// Get or set the target position
        /// </summary>
        public short TargetPosition
        {
            get { return targetPosition; }
            set
            {
                if (targetPosition == value) return;
                targetPosition = value;
                SetPosition(targetPosition).ConfigureAwait(false);
            }
        }

        private short securePosition;

        /// <summary>
        /// Get or set the secure position
        /// </summary>
        public short SecurePosition
        {
            get { return targetPosition; }
            set
            {
                if (securePosition == value) return;
                securePosition = value;
                SetMotorParams();
            }
        }

        /// <summary>
        /// Move the motor to a target position
        /// </summary>
        /// <param name="position">The new position</param>
        /// <returns>An awaitable task that completes when the motor reaches the target position</returns>
        public async Task MoveAsync(short position)
        {
            await SetPosition(position);
            while(true)
            {
                await GetPositionStatus();
                if (ActualPosition == TargetPosition)
                    return;
            }

        }

        private Task SetPosition(short position)
        {
            var data = new byte[4];
            data[0] = 0xff;
            data[1] = 0xff;
            data[2] = (byte)(position >> 8);
            data[3] = (byte)(position & 0xff);
            return dev.WriteBufferData((byte)Command.SetPosition, data);
        }

        private bool shaftDirection;

        /// <summary>
        /// Determines whether a positive motion is clockwise or counter-clockwise. Note that this value is ignored for RunVelocity mode!
        /// </summary>
        public bool ShaftDirection
        {
            get { return shaftDirection;  }
            set
            {
                if (shaftDirection == value) return;
                shaftDirection = value;
                SetMotorParams();
            }
        }

        private bool accelShape;

        /// <summary>
        /// get or set the acceleration shape
        /// </summary>
        public bool AccelShape
        {
            get { return accelShape; }
            set
            {
                if (accelShape == value) return;
                accelShape = value;
                SetMotorParams();
            }
        }

        private int minVelocityFactorThirtySeconds;

        /// <summary>
        /// Get or set the minimum velocity factor, in 1/32nds. 
        /// </summary>
        public int MinVelocityFactorThirtySeconds {
            get { return minVelocityFactorThirtySeconds; }
            set
            {
                if (value == minVelocityFactorThirtySeconds) return;
                minVelocityFactorThirtySeconds = value;
                SetMotorParams();
            }
        }

        private Accel acceleration;

        /// <summary>
        /// Get or set the acceleration curve to use
        /// </summary>
        public Accel Acceleration
        {
            get { return acceleration;  }
            set
            {
                if (acceleration == value) return;
                acceleration = value;
                SetMotorParams();
            }
        }

        private RunningCurrent runningCurrent;

        /// <summary>
        /// Get or set the running current
        /// </summary>
        public RunningCurrent RunCurrent
        {
            get { return runningCurrent; }
            set
            {
                if (runningCurrent == value) return;
                runningCurrent = value;
                SetMotorParams();
            }
        }
        private HoldingCurrent holdingCurrent;

        /// <summary>
        /// Get or set the holding current
        /// </summary>
        public HoldingCurrent HoldCurrent
        {
            get { return holdingCurrent; }
            set
            {
                if (holdingCurrent == value) return;
                holdingCurrent = value;
                SetMotorParams();
            }
        }

        private MaxVelocityType maxVelocity;

        /// <summary>
        /// Get or set the maximum velocity
        /// </summary>
        public MaxVelocityType MaxVelocity
        {
            get { return maxVelocity;  }
            set
            {
                if (maxVelocity == value) return;
                maxVelocity = value;
                SetMotorParams();
            }
        }



        private StepModeType stepMode;

        /// <summary>
        /// Get or set the step mode
        /// </summary>
        public StepModeType StepMode
        {
            get { return stepMode; }
            set
            {
                if (stepMode == value) return;
                stepMode = value;
                SetMotorParams();
            }
        }

        bool pwmFreq;
        bool pwmjen;
        private void SetMotorParams()
        {
            byte[] data = new byte[7];
            data[0] = 0xff;
            data[1] = 0xff;
            data[2] = (byte)(((byte)runningCurrent << 4) | ((byte)holdingCurrent & 0x0f));
            data[3] = (byte)(((byte)maxVelocity << 4) | ((byte)minVelocityFactorThirtySeconds & 0x0f));
            data[4] = (byte)(((byte)(securePosition >> 8) << 5) | ((shaftDirection ? 1 : 0) << 4) | ((byte)acceleration & 0x0f));
            data[5] = (byte)(securePosition & 0xff);
            data[6] = (byte)(
                    (1 << 7) | 
                    ((pwmFreq ? 1 : 0) << 6) | 
                    (1 << 5) | 
                    ((accelShape ? 1 : 0) << 4) |
                    ((byte)stepMode << 2) | 
                    (1 << 1) | 
                    (pwmjen ? 1 : 0)
                );
            dev.WriteBufferData((byte)Command.SetMotorParam, data).Wait();
        }
        /// <summary>
        /// Acceleration of the motor
        /// </summary>
        public enum Accel
        {
            /// <summary>
            /// 49 steps/s^2
            /// </summary>
            StepsPerSec2_49,

            /// <summary>
            /// 218 steps/s^2
            /// </summary>
            StepsPerSec2_218,

            /// <summary>
            /// 1004 steps/s^2
            /// </summary>
            StepsPerSec2_1004,

            /// <summary>
            /// 3609 steps/s^2
            /// </summary>
            StepsPerSec2_3609,

            /// <summary>
            /// 6228 steps/s^2
            /// </summary>
            StepsPerSec2_6228,

            /// <summary>
            /// 8848 steps/s^2
            /// </summary>
            StepsPerSec2_8848,

            /// <summary>
            /// 11409 steps/s^2
            /// </summary>
            StepsPerSec2_11409,

            /// <summary>
            /// 13970 steps/s^2
            /// </summary>
            StepsPerSec2_13970,

            /// <summary>
            /// 16531 steps/s^2
            /// </summary>
            StepsPerSec2_16531,

            /// <summary>
            /// 19092 steps/s^2
            /// </summary>
            StepsPerSec2_19092,

            /// <summary>
            /// 21886 steps/s^2
            /// </summary>
            StepsPerSec2_21886,

            /// <summary>
            /// 24447 steps/s^2
            /// </summary>
            StepsPerSec2_24447,

            /// <summary>
            /// 27008 steps/s^2
            /// </summary>
            StepsPerSec2_27008,

            /// <summary>
            /// 29570 steps/s^2
            /// </summary>
            StepsPerSec2_29570,

            /// <summary>
            /// 34925 steps/s^2
            /// </summary>
            StepsPerSec2_34925,

            /// <summary>
            /// 40047 steps/s^2
            /// </summary>
            StepsPerSec2_40047
        }

        internal enum Command
        {
            GetFullStatus1 = 0x81,
            GetFullStatus2 = 0xfc,
            GetOtpParam = 0x82,
            GotoSecurePosition = 0x84,
            HardStop = 0x85,
            ResetPosition = 0x86,
            ResetToDefault = 0x87,
            SetDualPosition = 0x88,
            SetMotorParam = 0x89,
            SetOtp = 0x90,
            SetPosition = 0x8B,
            SetStallParam = 0x96,
            SoftStop = 0x8f,
            RunVelocity = 0x97,
            TestBemf = 0x9f
        }

        /// <summary>
        /// Motor holding current
        /// </summary>
        public enum HoldingCurrent
        {
            /// <summary>
            /// 59 mA
            /// </summary>
            mA_59,

            /// <summary>
            /// 71 mA
            /// </summary>
            mA_71,

            /// <summary>
            /// 84 mA
            /// </summary>
            mA_84,

            /// <summary>
            /// 100 mA
            /// </summary>
            mA_100,

            /// <summary>
            /// 119 mA
            /// </summary>
            mA_119,

            /// <summary>
            /// 141 mA
            /// </summary>
            mA_141,

            /// <summary>
            /// 168 mA
            /// </summary>
            mA_168,

            /// <summary>
            /// 200 mA
            /// </summary>
            mA_200,

            /// <summary>
            /// 238 mA
            /// </summary>
            mA_238,

            /// <summary>
            /// 283 mA
            /// </summary>
            mA_283,

            /// <summary>
            /// 336 mA
            /// </summary>
            mA_336,

            /// <summary>
            /// 400 mA
            /// </summary>
            mA_400,

            /// <summary>
            /// 476 mA
            /// </summary>
            mA_476,

            /// <summary>
            /// 566 mA
            /// </summary>
            mA_566,

            /// <summary>
            /// 673 mA
            /// </summary>
            mA_673,

            /// <summary>
            /// 0 mA
            /// </summary>
            mA_0
        }

        /// <summary>
        /// Maximum motor velocity
        /// </summary>
        public enum MaxVelocityType
        {
            /// <summary>
            /// 99 steps/s
            /// </summary>
            StepsPerSecond_99,

            /// <summary>
            /// 136 steps/s
            /// </summary>
            StepsPerSecond_136,

            /// <summary>
            /// 167 steps/s
            /// </summary>
            StepsPerSecond_167,

            /// <summary>
            /// 197 steps/s
            /// </summary>
            StepsPerSecond_197,

            /// <summary>
            /// 213 steps/s
            /// </summary>
            StepsPerSecond_213,

            /// <summary>
            /// 228 steps/s
            /// </summary>
            StepsPerSecond_228,

            /// <summary>
            /// 243 steps/s
            /// </summary>
            StepsPerSecond_243,

            /// <summary>
            /// 273 steps/s
            /// </summary>
            StepsPerSecond_273,

            /// <summary>
            /// 303 steps/s
            /// </summary>
            StepsPerSecond_303,

            /// <summary>
            /// 334 steps/s
            /// </summary>
            StepsPerSecond_334,

            /// <summary>
            /// 364 steps/s
            /// </summary>
            StepsPerSecond_364,

            /// <summary>
            /// 395 steps/s
            /// </summary>
            StepsPerSecond_395,

            /// <summary>
            /// 456 steps/s
            /// </summary>
            StepsPerSecond_456,

            /// <summary>
            /// 546 steps/s
            /// </summary>
            StepsPerSecond_546,

            /// <summary>
            /// 729 steps/s
            /// </summary>
            StepsPerSecond_729,

            /// <summary>
            /// 973 steps/s
            /// </summary>
            StepsPerSecond_973
        }

        /// <summary>
        /// Motor running current
        /// </summary>
        public enum RunningCurrent
        {
            /// <summary>
            /// 59 mA
            /// </summary>
            mA_59,

            /// <summary>
            /// 71 mA
            /// </summary>
            mA_71,

            /// <summary>
            /// 84 mA
            /// </summary>
            mA_84,

            /// <summary>
            /// 100 mA
            /// </summary>
            mA_100,

            /// <summary>
            /// 119 mA
            /// </summary>
            mA_119,

            /// <summary>
            /// 141 mA
            /// </summary>
            mA_141,

            /// <summary>
            /// 168 mA
            /// </summary>
            mA_168,

            /// <summary>
            /// 200 mA
            /// </summary>
            mA_200,

            /// <summary>
            /// 238 mA
            /// </summary>
            mA_238,

            /// <summary>
            /// 283 mA
            /// </summary>
            mA_283,

            /// <summary>
            /// 336 mA
            /// </summary>
            mA_336,

            /// <summary>
            /// 400 mA
            /// </summary>
            mA_400,

            /// <summary>
            /// 476 mA
            /// </summary>
            mA_476,

            /// <summary>
            /// 566 mA
            /// </summary>
            mA_566,

            /// <summary>
            /// 673 mA
            /// </summary>
            mA_673,

            /// <summary>
            /// 800 mA
            /// </summary>
            mA_800
        };

        /// <summary>
        /// Step mode
        /// </summary>
        public enum StepModeType
        {
            /// <summary>
            /// Half-stepping
            /// </summary>
            HalfStepping,

            /// <summary>
            /// Quarter-stepping
            /// </summary>
            QuarterStepping,

            /// <summary>
            /// Eighth-stepping
            /// </summary>
            EighthStepping,

            /// <summary>
            /// Sixteenth-stepping
            /// </summary>
            SixteenthStepping
        }
    }
}
