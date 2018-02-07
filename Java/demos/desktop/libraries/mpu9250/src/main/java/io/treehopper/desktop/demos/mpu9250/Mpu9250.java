package io.treehopper.desktop.demos.mpu9250;

import com.badlogic.gdx.math.Vector3;

import java.util.ArrayList;
import java.util.List;

import io.treehopper.desktop.*;
import io.treehopper.*;
import io.treehopper.libraries.sensors.inertial.mpu6050.*;


public class Mpu9250 {
    public static void main(String[] args) throws InterruptedException {
        TreehopperUsb board = ConnectionService.getInstance().getBoards().get(0);
        board.connect();

        List<Mpu6050> imus;
        while(true) {
            imus = Mpu6050.Probe(board.i2c, true);
            if(imus.isEmpty()) {
                System.out.println("No MPU6050 or MPU9250 found.");
                Thread.sleep(500);
            } else {
                break;
            }
        }

        Mpu6050 imu = imus.get(0);
        imu.setAutoUpdateWhenPropertyRead(false);

        while(true)
        {
            imu.update();
            System.out.println(imu.getAccelerometer());
//            Vector3 dater = imu.getAccelerometer();
//            System.out.println(String.format("(%.2f, %.2f, %.2f)", dater.x, dater.y, dater.z));
                Thread.sleep(100);
        }
    }
}
