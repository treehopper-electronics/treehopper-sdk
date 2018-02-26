package io.treehopper.libraries.sensors.inertial.mpu6050;

import com.badlogic.gdx.math.Vector3;
import io.treehopper.SMBusDevice;
import io.treehopper.interfaces.I2c;
import io.treehopper.libraries.sensors.inertial.IAccelerometer;
import io.treehopper.libraries.sensors.inertial.IGyroscope;
import io.treehopper.libraries.sensors.temperature.TemperatureSensor;

import java.util.ArrayList;
import java.util.List;

/**
 * InvenSense MPU6050 6-DoF IMU
 */
public class Mpu6050 extends TemperatureSensor implements IAccelerometer, IGyroscope {

    protected Mpu6050Registers registers;
    private Vector3 accelerometer;
    private Vector3 gyroscope;
    public Mpu6050(I2c i2c, boolean addressPin, int ratekHz) {
        this.accelerometer = new Vector3();
        this.gyroscope = new Vector3();

        byte addr = (byte) 0x68;
        if (addressPin) {
            addr = (byte) 0x69;
        }

        registers = new Mpu6050Registers(new SMBusDevice(addr, i2c, ratekHz));
        registers.powerMgmt1.reset = 1;
        registers.powerMgmt1.write();
        registers.powerMgmt1.reset = 0;
        registers.powerMgmt1.sleep = 0;
        registers.powerMgmt1.write();
        registers.powerMgmt1.clockSel = 1;
        registers.powerMgmt1.write();
        registers.configuration.dlpf = 3;
        registers.configuration.write();
        registers.sampleRateDivider.value = 4;
        registers.sampleRateDivider.write();
        registers.accelConfig2.read();
        registers.accelConfig2.accelFchoice = 0;
        registers.accelConfig2.dlpfCfg = 3;
        registers.accelConfig2.write();
        setAccelScale(AccelScales.Fs_2g);
        setGyroScale(GyroScales.Dps_250);
    }

    public static List<Mpu6050> Probe(I2c i2c, boolean includeMpu9250) {
        List<Mpu6050> devs = new ArrayList<>();

        try {
            SMBusDevice dev = new SMBusDevice((byte) 0x68, i2c, 100);
            byte whoAmI = dev.readByteData((byte) 0x75);
            if (whoAmI == (byte) 0x68 || (whoAmI == 0x71 && includeMpu9250)) {
                devs.add(new Mpu6050(i2c, false, 100));
            }
        } catch (Exception ex) {
        }

        try {
            SMBusDevice dev = new SMBusDevice((byte) 0x69, i2c, 100);
            byte whoAmI = dev.readByteData((byte) 0x75);
            if (whoAmI == (byte) 0x68 || (whoAmI == 0x71 && includeMpu9250)) {
                devs.add(new Mpu6050(i2c, true, 100));
            }
        } catch (Exception ex) {
        }

        return devs;
    }

    public Vector3 getAccelerometer() {
        if (autoUpdateWhenPropertyRead)
            update();

        return accelerometer;
    }

    public Vector3 getGyroscope() {
        if (autoUpdateWhenPropertyRead)
            update();

        return gyroscope;
    }

    public AccelScales getAccelScale() {
        return AccelScales.values()[registers.accelConfig.accelScale];
    }

    public void setAccelScale(AccelScales value) {
        registers.accelConfig.accelScale = value.getVal();
        registers.accelConfig.write();
    }

    public int getAccelScaleValue() {
        return (int) (2 * Math.pow(2, registers.accelConfig.accelScale));
    }

    public void setAccelScaleValue(int value) {
        registers.accelConfig.accelScale = (int) (Math.log(value) / Math.log(2) - 1);
        registers.accelConfig.write();
    }

    public GyroScales getGyroScale() {
        return GyroScales.values()[registers.gyroConfig.gyroScale];
    }

    public void setGyroScale(GyroScales value) {
        registers.gyroConfig.gyroScale = value.getVal();
        registers.gyroConfig.write();
    }

    public int getGyroScaleValue() {
        return (int) (250 * Math.pow(2, registers.gyroConfig.gyroScale));
    }

    public void setGyroScaleValue(int value) {
        registers.gyroConfig.gyroScale = (int) (Math.log(value / 250) / Math.log(2));
        registers.gyroConfig.write();
    }

    public void update() {
        registers.readRange(registers.accel_x, registers.gyro_z);
        this.accelerometer.x = (float) (registers.accel_x.value * getAccelScaleValue() / 32768.0);
        this.accelerometer.y = (float) (registers.accel_y.value * getAccelScaleValue() / 32768.0);
        this.accelerometer.z = (float) (registers.accel_z.value * getAccelScaleValue() / 32768.0);

        this.gyroscope.x = (float) (registers.gyro_x.value * getGyroScaleValue() / 32768.0);
        this.gyroscope.y = (float) (registers.gyro_x.value * getGyroScaleValue() / 32768.0);
        this.gyroscope.z = (float) (registers.gyro_x.value * getGyroScaleValue() / 32768.0);

        this.celsius = registers.temp.value / 333.87 + 21.0;
    }
}
