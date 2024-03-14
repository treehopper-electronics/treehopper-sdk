using System;
using System.Collections.Generic;
using System.Text;

namespace Treehopper.Libraries.Motors
{
    /// <summary>
    ///     Any stepper motor driver capable of position and velocity control
    /// </summary>
    public abstract class StepperMotorController
    {
        public enum StepperMode
        {
            PositionControl,
            VelocityControl,
        }

        public enum MicrostepResolution
        {
            Microstep1,
            Microstep2,
            Microstep4,
            Microstep8,
            Microstep16,
            Microstep32,
            Microstep64,
            Microstep128,
            Microstep256,
        }

        public abstract StepperMode Mode {  get; set; }

        public abstract MicrostepResolution Resolution { get; set; }

        public abstract bool Enabled { get; set; }

        public abstract int TargetPosition { get; set; }

        public abstract int TargetVelocity { get; set; }

        public abstract int ActualPosition { get; set; }

        public abstract int ActualVelocity { get; set; }

        public abstract int AccelerationLimit { get; set; }

        public abstract double HoldingCurrent {  get; set; }

        public abstract double RunningCurrent {  get; set; }

        public abstract void SetZeroPosition();
    }
}
