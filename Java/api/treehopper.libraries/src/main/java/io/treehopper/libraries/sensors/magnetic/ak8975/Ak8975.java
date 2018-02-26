package io.treehopper.libraries.sensors.magnetic.ak8975;

import io.treehopper.SMBusDevice;
import io.treehopper.interfaces.I2c;
import io.treehopper.libraries.sensors.magnetic.Magnetometer;

public class Ak8975 extends Magnetometer {
    Ak8975Registers _registers;
    private SMBusDevice dev;

    public Ak8975(I2c i2c) {
        dev = new SMBusDevice((byte) 0x0c, i2c);
        _registers = new Ak8975Registers(dev);
    }

    @Override
    public void update() {
        _registers.control.mode = 1;
        _registers.control.write();
        while (true) {
            _registers.status1.read();
            if (_registers.status1.drdy == 1)
                break;
        }

        _registers.readRange(_registers.hx, _registers.hz);
        _magnetometer.x = _registers.hx.value;
        _magnetometer.y = _registers.hy.value;
        _magnetometer.z = _registers.hz.value;
    }
}
