package io.treehopper.desktop.demos.sandbox;

import com.badlogic.gdx.math.Vector3;

import java.util.ArrayList;
import io.treehopper.desktop.*;
import io.treehopper.*;
import io.treehopper.libraries.io.adc.nau7802.ConversionRates;
import io.treehopper.libraries.io.adc.nau7802.Gains;
import io.treehopper.libraries.io.adc.nau7802.Nau7802;
import io.treehopper.libraries.sensors.inertial.adxl345.Adxl345;

public class Sandbox {
    public static void main(String[] args) {
        ConnectionService service = new ConnectionService();
        ArrayList<TreehopperUsb> boards = service.getBoards();
        TreehopperUsb board = boards.get(0);
        board.connect();

//        Adxl345 imu = new Adxl345(board.i2c, false, 100);
        Nau7802 adc = new Nau7802(board.i2c);

        adc.setGain(Gains.x1);
        adc.setConversionRate(ConversionRates.Sps_80);
        adc.calibrate();

        while(true)
        {
            System.out.println(adc.getAdcValue());
//            Vector3 dater = imu.getAccelerometer();
//            System.out.println(String.format("(%.2f, %.2f, %.2f)", dater.x, dater.y, dater.z));
//            try {
//                Thread.sleep(100);
//            } catch (InterruptedException e) {
//                e.printStackTrace();
//            }
        }

    }
}
