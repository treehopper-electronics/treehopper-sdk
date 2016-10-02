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
        public Amis30624(II2c module, Address HardwiredAddressPin) : this(module, (byte)HardwiredAddressPin)
        {

        }

        public Amis30624(II2c module, byte address)
        {
            dev = new SMBusDevice(address, module);
            GetStatus().ConfigureAwait(false);
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

        public async Task GetStatus()
        {
            var data = await dev.ReadBufferData((byte)Command.GetFullStatus1, 8);
            int irun = (data[0] >> 4) & 0x0f;
            runningCurrent = (RunningCurrent)irun;

            int ihold = data[0] & 0x0f;
            holdingCurrent = (HoldingCurrent)irun;

            int vmax = (data[1] >> 4) & 0x0f;
            maxVelocity = (MaxVelocity)vmax;

            int vmin = data[1] & 0x0f;
            minVelocityFactorThirtySeconds = (vmin == 0) ? 32 : vmin;

            int acc = data[2] & 0x0f;
            acceleration = (Acceleration)acc;

            accelShape = (data[2] & 0x80) > 0;

            int _stepMode = (data[2] >> 5) & 0x03;
            stepMode = (StepMode)_stepMode;

            shaftDirection = (data[2] & 0x10) > 0;



            data = await dev.ReadBufferData((byte)Command.GetFullStatus2, 8);

        }

        private bool shaftDirection;
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

        ushort secPos;
        bool pwmFreq;
        bool pwmjen;
        private void SetMotorParams()
        {
            byte[] data = new byte[7];
            data[0] = 0xff;
            data[1] = 0xff;
            data[2] = (byte)(((byte)runningCurrent << 4) | ((byte)holdingCurrent & 0x0f));
            data[3] = (byte)(((byte)maxVelocity << 4) | ((byte)minVelocityFactorThirtySeconds & 0x0f));
            data[4] = (byte)(((byte)(secPos >> 8) << 5) | ((shaftDirection ? 1 : 0) << 4) | ((byte)acceleration & 0x0f));
            data[5] = (byte)(secPos & 0xff);
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
