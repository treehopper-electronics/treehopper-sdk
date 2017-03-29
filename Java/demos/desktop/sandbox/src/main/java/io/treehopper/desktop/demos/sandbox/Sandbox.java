package io.treehopper.desktop.demos.sandbox;

import java.util.ArrayList;
import io.treehopper.desktop.*;
import io.treehopper.*;
import io.treehopper.libraries.displays.SevenSegmentDisplay;
import io.treehopper.libraries.displays.Tm1650;
import io.treehopper.libraries.input.RotaryEncoder;
import io.treehopper.libraries.motors.SeeedGroveI2cMotorDriver;
import io.treehopper.libraries.sensors.temperature.Mcp9808;

public class Sandbox {
    public static void main(String[] args) {
        ConnectionService service = new ConnectionService();
        ArrayList<TreehopperUsb> boards = service.getBoards();
        TreehopperUsb board = boards.get(0);
        board.connect();

        SeeedGroveI2cMotorDriver driver = new SeeedGroveI2cMotorDriver(board.i2c);

        driver.setMotor1(1.0);

        while(true)
        {
//            System.out.println(temp.getTemperatureFahrenheit());
//            try {
//                Thread.sleep(1000);
//            } catch (InterruptedException e) {
//                e.printStackTrace();
//            }
        }

    }
}
