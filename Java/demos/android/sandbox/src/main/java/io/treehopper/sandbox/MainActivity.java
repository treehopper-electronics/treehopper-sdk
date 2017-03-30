package io.treehopper.sandbox;

import android.os.Bundle;
import android.util.Log;

import com.badlogic.gdx.math.Vector3;

import io.treehopper.TreehopperUsb;
import io.treehopper.android.TreehopperActivity;
import io.treehopper.libraries.sensors.imu.Mpu6050;

public class MainActivity extends TreehopperActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
    }

    @Override
    protected void boardAdded(TreehopperUsb board) {
        board.connect();

        Mpu6050 imu = new Mpu6050(board.i2c, false, 100);
        while(true)
        {
            imu.update();
            Vector3 dater = imu.getAccelerometer();
            Log.d("Treehopper", "Temperature: " + dater.toString());
        }

//        SeeedGroveI2cMotorDriver driver = new SeeedGroveI2cMotorDriver(board.i2c);
//        driver.setMotor1(1);
//        driver.setMotor2(1);
    }

    @Override
    protected void boardRemoved(TreehopperUsb board) {

    }
}
