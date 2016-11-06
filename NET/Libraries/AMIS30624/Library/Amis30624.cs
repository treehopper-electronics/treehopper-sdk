using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Treehopper;

namespace Treehopper.Libraries.Amis30624
{
    public class Amis30624
    {
        private SMBusDevice dev;
        public Amis30624(II2c module, Address HardwiredAddressPin, int speed = 400) : this(module, (byte)HardwiredAddressPin, speed)
        {

        }

        public Amis30624(II2c module, byte address, int speed)
        {
            dev = new SMBusDevice(address, module, speed);
            ResetToDefault().ConfigureAwait(false);
            GetFullStatus().ConfigureAwait(false);
        }

        public Task ResetToDefault()
        {
            return dev.WriteByte((byte)Command.ResetToDefault);
        }


        public Task SoftStop()
        {
            return dev.WriteByte((byte)Command.SoftStop);
        }


        public Task HardStop()
        {
            return dev.WriteByte((byte)Command.HardStop);
        }


        public Task RunVelocity()
        {
            return dev.WriteByte((byte)Command.RunVelocity);
        }

        public async Task GetFullStatus()
        {
            var data = await dev.ReadBufferData((byte)Command.GetFullStatus1, 9);
            int irun = (data[1] >> 4) & 0x0f;
            runningCurrent = (RunningCurrent)irun;

            int ihold = data[1] & 0x0f;
            holdingCurrent = (HoldingCurrent)irun;

            int vmax = (data[2] >> 4) & 0x0f;
            maxVelocity = (MaxVelocity)vmax;

            int vmin = data[2] & 0x0f;
            minVelocityFactorThirtySeconds = (vmin == 0) ? 32 : vmin;

            int acc = data[3] & 0x0f;
            acceleration = (Acceleration)acc;

            accelShape = (data[3] & 0x80) > 0;

            int _stepMode = (data[3] >> 5) & 0x03;
            stepMode = (StepMode)_stepMode;

            shaftDirection = (data[3] & 0x10) > 0;

            await GetPositionStatus();
        }

        public async Task GetPositionStatus()
        {
            var data = await dev.ReadBufferData((byte)Command.GetFullStatus2, 8);
            ActualPosition = (short)(data[1] << 8 | data[2]);
            targetPosition = (short)(data[3] << 8 | data[4]);
            securePosition = (short)(data[5]);

        }

        public short ActualPosition { get; private set; }

        private short targetPosition;
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
        public int MinVelocityFactorThirtySeconds {
            get { return minVelocityFactorThirtySeconds; }
            set
            {
                if (value == minVelocityFactorThirtySeconds) return;
                minVelocityFactorThirtySeconds = value;
                SetMotorParams();
            }
        }

        private Acceleration acceleration;
        public Acceleration Acceleration
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
        public RunningCurrent RunningCurrent
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
        public HoldingCurrent HoldingCurrent
        {
            get { return holdingCurrent; }
            set
            {
                if (holdingCurrent == value) return;
                holdingCurrent = value;
                SetMotorParams();
            }
        }

        private MaxVelocity maxVelocity;
        public MaxVelocity MaxVelocity
        {
            get { return maxVelocity;  }
            set
            {
                if (maxVelocity == value) return;
                maxVelocity = value;
                SetMotorParams();
            }
        }



        private StepMode stepMode;
        public StepMode StepMode
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

    }

}
