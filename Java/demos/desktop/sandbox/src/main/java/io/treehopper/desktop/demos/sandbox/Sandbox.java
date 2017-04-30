package io.treehopper.desktop.demos.sandbox;

import com.badlogic.gdx.math.Vector3;

import java.util.ArrayList;
import io.treehopper.desktop.*;
import io.treehopper.*;
import io.treehopper.libraries.sensors.inertial.Adxl345;
import io.treehopper.libraries.sensors.inertial.Mpu6050;

public class Sandbox {
    public static void main(String[] args) {
        ConnectionService service = new ConnectionService();
        ArrayList<TreehopperUsb> boards = service.getBoards();
        TreehopperUsb board = boards.get(0);
        board.connect();

        Adxl345 imu = new Adxl345(board.i2c, false, 100);


        while(true)
        {
            Vector3 dater = imu.getAccelerometer();
            System.out.println(String.format("(%.2f, %.2f, %.2f)", dater.x, dater.y, dater.z));
            try {
                Thread.sleep(100);
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }

    }
}
