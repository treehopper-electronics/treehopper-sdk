package io.treehopper.libraries.motors;

import io.treehopper.SMBusDevice;
import io.treehopper.interfaces.I2c;

/**
 * Seeed Grove I2c Motor Driver
 */
public class SeeedGroveI2cMotorDriver {
    enum Registers
    {
        MotorSpeedSet(0x82),
        PwmFrequencySet(0x84),
        DirectionSet(0xaa),
        MotorSetA(0xa1),
        MotorSetB(0xa5),
        Nothing(0x01);

        Registers(int reg) {
            this.val = reg;
        }
        public byte getVal() {
            return (byte) val;
        }
        int val;
    }

    enum MotorSetDirection
    {
        BothClockwise(0x0a),
        BothCounterClockwise(0x05),
        Motor1ClockwiseMotor2CounterClockwise(0x06),
        Motor1CounterClockwiseMotor2Clockwise(0x09);

        MotorSetDirection(int val) {
            this.val = val;
        }
        public byte getVal() {
            return (byte)val;
        }
        int val;

    }

    /**
     * The driver frequency
     */
    public enum PrescalerFrequency
    {
        Freq_31372Hz(0x01),
        Freq_3921Hz(0x02),
        Freq_490Hz(0x03),
        Freq_122Hz(0x04),
        Freq_30Hz(0x05);

        PrescalerFrequency(int val) {
            this.val = val;
        }
        public byte getVal() {
            return (byte)val;
        }
        int val;
    }

    /**
     * Construct a new Seeed Grove I2c Motor Driver, with the address specified by the state of the DIP switches
     * @param i2c the I2c module the driver is attached to
     * @param a1 A1
     * @param a2 A2
     * @param a3 A3
     * @param a4 A4
     */
    public SeeedGroveI2cMotorDriver(I2c i2c, boolean a1, boolean a2, boolean a3, boolean a4)
    {
        this(i2c, (byte)(((a1 ? 1 : 0) << 3) | ((a2 ? 1 : 0) << 2) | ((a3 ? 1 : 0) << 1) | ((a4 ? 1 : 0) << 0)));
    }

    /**
     * Construct a new Seeed Grove I2c Motor Driver, with the address specified
     * @param i2c the I2c module the driver is attached to.
     * @param address the address of the driver
     */
    public SeeedGroveI2cMotorDriver(I2c i2c, byte address)
    {
        this.dev = new SMBusDevice((byte)(address), i2c);
        setFrequency(PrescalerFrequency.Freq_3921Hz);
    }

    /**
     * Construct a new Seeed Grove I2c Motor Driver with the default address
     * @param i2c the I2c module the driver is attached to
     */
    public SeeedGroveI2cMotorDriver(I2c i2c)
    {
        this(i2c, (byte)0x0f);
    }

    private double motor1 = 0;
    private double motor2 = 0;
    private PrescalerFrequency frequency;
    private SMBusDevice dev;

    /**
     * Get the speed -- from -1.0 to 1.0 -- of Motor 1
     * @return the speed
     */
    public double getMotor1() {
        return motor1;
    }

    /**
     * Set the speed -- from -1.0 to 1.0 -- of Motor 1
     * @param motor1 the speed of Motor 1
     */
    public void setMotor1(double motor1) {
        this.motor1 = motor1;
        update();
    }

    /**
     * Get the speed -- from -1.0 to 1.0 -- of Motor 2
     * @return the speed
     */
    public double getMotor2() {
        return motor2;
    }

    /**
     * Set the speed -- from -1.0 to 1.0 -- of Motor 2
     * @param motor2 the speed of Motor 2
     */
    public void setMotor2(double motor2) {
        this.motor2 = motor2;
        update();
    }

    /**
     * Get the driver frequency
     * @return the driver frequency
     */
    public PrescalerFrequency getFrequency() {
        return frequency;
    }

    /**
     * Set the driver frequency
     * @param frequency the driver frequency
     */
    public void setFrequency(PrescalerFrequency frequency) {
        this.frequency = frequency;
        dev.WriteBufferData(Registers.PwmFrequencySet.getVal(), new byte[] { frequency.getVal(), 0x01 });
    }


    private void update()
    {
        byte m1speed = (byte)Math.round(Math.abs(motor1) * 255);
        byte m2speed = (byte)Math.round(Math.abs(motor2) * 255);
        dev.WriteBufferData(Registers.MotorSpeedSet.getVal(), new byte[] { m1speed, m2speed });

        if (motor1 >= 0 && motor2 >= 0)
            dev.WriteBufferData(Registers.DirectionSet.getVal(), new byte[] { MotorSetDirection.BothClockwise.getVal(), 0x01 });
        else if (motor1 >= 0 && motor2 < 0)
            dev.WriteBufferData(Registers.DirectionSet.getVal(), new byte[] { MotorSetDirection.Motor1ClockwiseMotor2CounterClockwise.getVal(), 0x01 });
        else if (motor1 < 0 && motor2 < 0)
            dev.WriteBufferData(Registers.DirectionSet.getVal(), new byte[] { MotorSetDirection.BothCounterClockwise.getVal(), 0x01 });
        else if (motor1 < 0 && motor2 >= 0)
            dev.WriteBufferData(Registers.DirectionSet.getVal(), new byte[] { MotorSetDirection.Motor1CounterClockwiseMotor2Clockwise.getVal(), 0x01 });
    }
}
