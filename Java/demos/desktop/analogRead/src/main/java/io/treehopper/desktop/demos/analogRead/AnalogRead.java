package io.treehopper.desktop.demos.analogRead;

import java.util.ArrayList;
import io.treehopper.desktop.*;
import io.treehopper.*;

public class AnalogRead {
    public static void main(String[] args) {
        ConnectionService service = new ConnectionService();
        ArrayList<TreehopperUsb> boards = service.getBoards();
        TreehopperUsb hopper = boards.get(0);
        hopper.connect();

        Pin adcPin = hopper.pins[10];
        adcPin.setMode(PinMode.AnalogInput);

        while (true) {
            double value = adcPin.getAdcValue();
            System.out.println(value);
            try {
                Thread.sleep(100);
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }
    }
}
