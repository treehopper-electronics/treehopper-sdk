package io.treehopper.desktop.demos;

import io.treehopper.TreehopperUsb;
import io.treehopper.desktop.ConnectionService;
import io.treehopper.libraries.sensors.pressure.bmp280.Bme280;
import io.treehopper.libraries.sensors.pressure.bmp280.Bmp280;

import java.util.List;

public class bmp280 {
    public static void main(String[] args) throws InterruptedException {
        TreehopperUsb board = ConnectionService.getInstance().getBoards().get(0);
        board.connect();

        List<Bmp280> sensors;
        while(true) {
            sensors = Bmp280.Probe(board.i2c, true);
            if(sensors.isEmpty()) {
                System.out.println("No BMP280 or BME280 found.");
                Thread.sleep(500);
            } else {
                break;
            }
        }

        Bmp280 sensor = sensors.get(0);
        sensor.setAutoUpdateWhenPropertyRead(false);

        while(true)
        {
            sensor.update();
            System.out.println("Altitude: " + sensor.getAltitude() + "m");
            Thread.sleep(100);
        }
    }
}
