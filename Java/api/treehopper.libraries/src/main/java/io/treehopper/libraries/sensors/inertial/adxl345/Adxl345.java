package io.treehopper.libraries.sensors.inertial.adxl345;

import com.badlogic.gdx.math.Vector3;
import io.treehopper.SMBusDevice;
import io.treehopper.interfaces.I2c;
import io.treehopper.libraries.SMBusRegisterManagerAdapter;
import io.treehopper.libraries.sensors.inertial.Accelerometer;

/**
 * Created by JayLocal on 4/29/2017.
 */
public class Adxl345 extends Accelerometer {
    private Adxl345Registers registers;

    public Adxl345(I2c i2c, boolean altAddress, int rate) {
        SMBusDevice dev = new SMBusDevice((byte) (!altAddress ? 0x53 : 0x1D), i2c, rate);
        registers = new Adxl345Registers(new SMBusRegisterManagerAdapter(dev));
        registers.powerCtl.sleep = 0;
        registers.powerCtl.measure = 1;
        registers.dataFormat.range = 0x03;
        registers.powerCtl.write();
        registers.dataFormat.write();
    }

    @Override
    public void update() {
        accelerometer = new Vector3();
        accelerometer.x = registers.dataX.value * 0.04f;
        accelerometer.y = registers.dataY.value * 0.04f;
        accelerometer.z = registers.dataZ.value * 0.04f;
    }
}
