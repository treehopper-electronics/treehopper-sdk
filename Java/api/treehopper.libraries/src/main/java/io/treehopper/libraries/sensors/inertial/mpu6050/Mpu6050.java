package io.treehopper.libraries.sensors.inertial.mpu6050;

import com.badlogic.gdx.math.Vector3;

import io.treehopper.SMBusDevice;
import io.treehopper.interfaces.I2c;
import io.treehopper.libraries.sensors.inertial.IAccelerometer;
import io.treehopper.libraries.sensors.inertial.IGyroscope;
import io.treehopper.libraries.sensors.temperature.TemperatureSensor;

/**
 * InvenSense MPU6050 6-DoF IMU
 */
public class Mpu6050 extends TemperatureSensor implements IAccelerometer, IGyroscope {
	private Vector3 accelerometer;
	private Vector3 gyroscope;
	private Vector3 accelOffset;
	private Vector3 gyroOffset;
	private double temperature;
	Mpu6050Registers registers;

    public Mpu6050(I2c i2c, boolean addressPin, int ratekHz) {
    	this.accelerometer = new Vector3();
    	this.gyroscope = new Vector3();
    	this.accelOffset = new Vector3();
    	this.gyroOffset = new Vector3();

    	byte addr = (byte) 0x68;
    	if(addressPin) {
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
    }
    
	public SMBusDevice getDev() {
		return dev;
	}

	public void setDev(SMBusDevice dev) {
		this.dev = dev;
	}

	public Vector3 getAccelerometer() {
		return accelerometer;
	}

	public void setAccelerometer(Vector3 accelerometer) {
		this.accelerometer = accelerometer;
	}

	public Vector3 getGyroscope() {
		return gyroscope;
	}

	public void setGyroscope(Vector3 gyroscope) {
		this.gyroscope = gyroscope;
	}

	public int getAccelScale() {
		return accelScale;
	}

	public void setAccellScale(int accellScale) {
		this.accelScale = accellScale;
	}

	public int getGyrosScale() {
		return gyroScale;
	}

	public void setGyrosScale(int gyrosScale) {
		this.gyroScale = gyrosScale;
	}

	@Override
	public double getTemperatureCelsius() {
		// TODO Auto-generated method stub
		return 0;
	}
    
    public void update() {
        registers.read_range(registers.accel_x, registers.gyro_z)
        this.accelerometer.x = registers.accel_x.value * self.accel_scale / 32768.0;
        this.accelerometer.x = registers.accel_y.value * self.accel_scale / 32768.0;
        this.accelerometer.x = registers.accel_z.value * self.accel_scale / 32768.0;

        this.gyroscope.x = registers.gyro_x.value;
        this.gyroscope.y = registers.gyro_x.value;
        this.gyroscope.z = registers.gyro_x.value;
                

        celsius = registers.temp.value / 333.87 + 21.0

    	byte[] data = getDev().readBufferData(ACCEL_XOUT_H, 14);
    	double accelScale = getAccelScale();
    	double gyroScale = getGyrosScale();
    	
    	getAccelerometer().x = (float)(((short)((data[0] & 0xff) << 8 |(data[1] & 0xff))/32768.0) * accelScale - getAccelOffset().x);
    	getAccelerometer().y = (float)(((short)((data[2] & 0xff) << 8 | (data[3] & 0xff))/32768.0) * accelScale - getAccelOffset().y);
    	getAccelerometer().z = (float)(((short)((data[4] & 0xff) << 8 | (data[5] & 0xff))/32768.0) * accelScale - getAccelOffset().z);
    	
    	setTemperature((data[6] << 8 | data[7]) / 333.87 + 21.0);
    	
    	getGyroscope().x = (float)(((short)((data[8] & 0xff) << 8 | (data[9] & 0xff))/32768.0) * gyroScale - getGyroOffset().x);
    	getGyroscope().y = (float)(((short)((data[10] & 0xff) << 8 | (data[11] & 0xff))/32768.0) * gyroScale - getGyroOffset().y);
    	getGyroscope().z = (float)(((short)((data[12] & 0xff) << 8 | (data[13] & 0xff))/32768.0) * gyroScale - getGyroOffset().z);
    }

	public Vector3 getAccelOffset() {
		return accelOffset;
	}

	public void setAccellOffset(Vector3 accellOffset) {
		this.accelOffset = accellOffset;
	}

	public int getGyroScale() {
		return gyroScale;
	}

	public void setGyroScale(int gyroScale) {
		this.gyroScale = gyroScale;
	}

	public double getTemperature() {
		return temperature;
	}

	public void setTemperature(double temperature) {
		this.temperature = temperature;
	}

	public Vector3 getGyroOffset() {
		return gyroOffset;
	}

	public void setGyroOffset(Vector3 gyroOffset) {
		this.gyroOffset = gyroOffset;
	}
}
