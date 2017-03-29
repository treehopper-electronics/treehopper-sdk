package io.treehopper.desktop.demos.sandbox;

import com.badlogic.gdx.math.Vector3;

import java.util.ArrayList;
import io.treehopper.desktop.*;
import io.treehopper.*;
import io.treehopper.libraries.displays.SevenSegmentDisplay;
import io.treehopper.libraries.displays.Tm1650;
import io.treehopper.libraries.input.RotaryEncoder;
import io.treehopper.libraries.motors.SeeedGroveI2cMotorDriver;
import io.treehopper.libraries.sensors.imu.Mpu6050;
import io.treehopper.libraries.sensors.temperature.Mcp9808;

public class Sandbox {
    public static void main(String[] args) {
        ConnectionService service = new ConnectionService();
        ArrayList<TreehopperUsb> boards = service.getBoards();
        TreehopperUsb board = boards.get(0);
        board.connect();

        Mpu6050 imu = new Mpu6050(board.i2c, false, 100);


        while(true)
        {
            imu.update();
            Vector3 dater = imu.getAccelerometer();
            System.out.println(String.format("(%.2f, %.2f, %.2f)", dater.x, dater.y, dater.z));
//            System.out.println(temp.getTemperatureFahrenheit());
//            try {
//                Thread.sleep(1000);
//            } catch (InterruptedException e) {
//                e.printStackTrace();
//            }
        }

    }
}
