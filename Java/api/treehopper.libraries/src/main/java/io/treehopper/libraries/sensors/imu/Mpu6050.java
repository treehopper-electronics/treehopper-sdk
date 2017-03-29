package io.treehopper.libraries.sensors.imu;

import com.badlogic.gdx.math.Vector3;

import io.treehopper.SMBusDevice;
import io.treehopper.interfaces.I2c;
import io.treehopper.libraries.sensors.temperature.TemperatureSensor;

public class Mpu6050 extends TemperatureSensor implements IAccelerometer, IGyroscope{
	private SMBusDevice dev;
	private Vector3 accelerometer;
	private Vector3 gyroscope;
	private Vector3 accelOffset;
	private Vector3 gyroOffset;
	private int accelScale; // 2,4,8,16 g
	private int gyroScale; // 250,500,1000,2000 DPS
	private double temperature;
	
    private static final byte 
    SELF_TEST_X = 0x0D,
    SELF_TEST_Y = 0x0E,
    SELF_TEST_Z = 0x0F,
    SELF_TEST_A = 0x10,
    SMPLRT_DIV = 0x19,
    CONFIG = 0x1A,
    GYRO_CONFIG = 0x1B,
    ACCEL_CONFIG = 0x1C,
    ACCEL_CONFIG2 = 0x1D,
    FIFO_EN = 0x23,
    I2C_MST_CTRL = 0x24,

    I2C_SLV0_ADDR = 0x25,
    I2C_SLV0_REG = 0x26,
    I2C_SLV0_CTRL = 0x27,

    I2C_SLV1_ADDR = 0x28,
    I2C_SLV1_REG = 0x29,
    I2C_SLV1_CTRL = 0x2A,

    I2C_SLV2_ADDR = 0x2B,
    I2C_SLV2_REG = 0x2C,
    I2C_SLV2_CTRL = 0x2D,

    I2C_SLV3_ADDR = 0x2E,
    I2C_SLV3_REG = 0x2F,
    I2C_SLV3_CTRL = 0x30,

    I2C_SLV4_ADDR = 0x31,
    I2C_SLV4_REG = 0x32,
    I2C_SLV4_DO = 0x33,
    I2C_SLV4_CTRL = 0x34,
    I2C_SLV4_DI = 0x35,

    I2C_MST_STATUS = 0x36,
    INT_PIN_CFG = 0x37,
    INT_ENABLE = 0x38,
    INT_STATUS = 0x3A,

    ACCEL_XOUT_H = 0x3B,
    ACCEL_XOUT_L = 0x3C,
    ACCEL_YOUT_H = 0x3D,
    ACCEL_YOUT_L = 0x3E,
    ACCEL_ZOUT_H = 0x3F,
    ACCEL_ZOUT_L = 0x40,

    TEMP_OUT_H = 0x41,
    TEMP_OUT_L = 0x42,

    GYRO_XOUT_H = 0x43,
    GYRO_XOUT_L = 0x44,
    GYRO_YOUT_H = 0x45,
    GYRO_YOUT_L = 0x46,
    GYRO_ZOUT_H = 0x47,
    GYRO_ZOUT_L = 0x48,

    EXT_SENS_DATA_00 = 0x49,
    EXT_SENS_DATA_01 = 0x4A,
    EXT_SENS_DATA_02 = 0x4B,
    EXT_SENS_DATA_03 = 0x4C,
    EXT_SENS_DATA_04 = 0x4D,
    EXT_SENS_DATA_05 = 0x4E,
    EXT_SENS_DATA_06 = 0x4F,
    EXT_SENS_DATA_07 = 0x50,
    EXT_SENS_DATA_08 = 0x51,
    EXT_SENS_DATA_09 = 0x52,
    EXT_SENS_DATA_10 = 0x53,
    EXT_SENS_DATA_11 = 0x54,
    EXT_SENS_DATA_12 = 0x55,
    EXT_SENS_DATA_13 = 0x56,
    EXT_SENS_DATA_14 = 0x57,
    EXT_SENS_DATA_15 = 0x58,
    EXT_SENS_DATA_16 = 0x59,
    EXT_SENS_DATA_17 = 0x5A,
    EXT_SENS_DATA_18 = 0x5B,
    EXT_SENS_DATA_19 = 0x5C,
    EXT_SENS_DATA_20 = 0x5D,
    EXT_SENS_DATA_21 = 0x5E,
    EXT_SENS_DATA_22 = 0x5F,
    EXT_SENS_DATA_23 = 0x60,

    I2C_SLV0_D0 = 0x63,
    I2C_SLV1_DO = 0x64,
    I2C_SLV2_DO = 0x65,
    I2C_SLV3_D0 = 0x66,
    I2C_MST_DELAY_CTRL = 0x67,
    SIGNAL_PATH_RESET = 0x68,
    USER_CTRL = 0x6A,
    PWR_MGMT_1 = 0x6B,
    PWR_MGMT_2 = 0x6C,
    FIFO_COUNTH = 0x72,
    FIFO_COUNTL = 0x73,
    FIFO_R_W = 0x74,
    WHO_AM_I = 0x75;

    public Mpu6050(I2c i2c, boolean addressPin, int ratekHz) {
    	this.accelerometer = new Vector3();
    	this.gyroscope = new Vector3();
    	this.accelOffset = new Vector3();
    	this.gyroOffset = new Vector3();
    	
    	byte addr = (byte) 0x68;
    	if(addressPin) {
    		addr = (byte) 0x69;
    	}
    	this.dev = new SMBusDevice(addr, i2c ,ratekHz);
    	

    	
    	dev.WriteByteData(PWR_MGMT_1, (byte)0x00); // Wake up
    	try {
			Thread.sleep(100);
		} catch (InterruptedException e) {
			e.printStackTrace();
		}
    	dev.WriteByteData(PWR_MGMT_1, (byte)0x01); // Auto select clock source to be PLL gyroscope reference if ready else
    	try {
			Thread.sleep(200);
		} catch (InterruptedException e) {
			e.printStackTrace();
		}
    	
    	dev.WriteByteData(CONFIG, (byte) 0x03);
    	dev.WriteByteData(SMPLRT_DIV, (byte) 0x04);
    	
    	this.accelScale = 2;
    	this.gyroScale = 250;
    	
    	byte accelConfigResult = dev.ReadByteData((byte) ACCEL_CONFIG2);
    	accelConfigResult &= (byte)~0x0f & (byte)0xff;
    	accelConfigResult |= 0x03;
    	dev.WriteByteData(ACCEL_CONFIG2, accelConfigResult);
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
    	byte[] data = getDev().ReadBufferData(ACCEL_XOUT_H, 14);
    	double accelScale = getAccelScale();
    	double gyroScale = getGyrosScale();
    	
    	getAccelerometer().x = (float)(((short)((data[0] & 0xff) << 8 |(data[1] & 0xff))/32768.0) * accelScale - getAccelOffset().x);
    	getAccelerometer().y = (float)(((short)((data[2] & 0xff) << 8 | (data[3] & 0xff))/32768.0) * accelScale - getAccelOffset().y);
    	getAccelerometer().z = (float)(((short)((data[4] & 0xff) << 8 | (data[5] & 0xff))/32768.0) * accelScale - getAccelOffset().z);
    	
    	setTemperature((data[6] << 8 | data[7]) / 333.87 + 21.0);
    	
    	getGyroscope().x = (float)(((short)((data[8] & 0xff) << 8 | (data[9] & 0xff))) * gyroScale - getGyroOffset().x);
    	getGyroscope().y = (float)(((short)((data[10] & 0xff) << 8 | (data[11] & 0xff))) * gyroScale - getGyroOffset().y);
    	getGyroscope().z = (float)(((short)((data[12] & 0xff) << 8 | (data[13] & 0xff))) * gyroScale - getGyroOffset().z);
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
