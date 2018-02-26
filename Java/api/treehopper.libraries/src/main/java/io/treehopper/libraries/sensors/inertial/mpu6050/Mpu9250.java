package io.treehopper.libraries.sensors.inertial.mpu6050;

import com.badlogic.gdx.math.Vector3;
import io.treehopper.interfaces.I2c;
import io.treehopper.libraries.sensors.magnetic.IMagnetometer;
import io.treehopper.libraries.sensors.magnetic.ak8975.Ak8975;

public class Mpu9250 extends Mpu6050 implements IMagnetometer {
    private Ak8975 mag;
    private boolean enableMagnetometer = true;

    public Mpu9250(I2c i2c, boolean addressPin, int rate) {
        super(i2c, addressPin, rate);
        mag = new Ak8975(i2c);
        mag.setAutoUpdateWhenPropertyRead(false);
        registers.intPinCfg.bypassEn = 1;
        registers.intPinCfg.latchIntEn = 1;
        registers.intPinCfg.write();
    }

    @Override
    public void update() {
        super.update();

        if (!enableMagnetometer) return;
        mag.update();
    }

    public boolean isEnableMagnetometer() {
        return enableMagnetometer;
    }

    public void setEnableMagnetometer(boolean enableMagnetometer) {
        this.enableMagnetometer = enableMagnetometer;
    }

    @Override
    public Vector3 getMagnetometer() {
        if (isAutoUpdateWhenPropertyRead())
            update();

        return mag.getMagnetometer();
    }
}
