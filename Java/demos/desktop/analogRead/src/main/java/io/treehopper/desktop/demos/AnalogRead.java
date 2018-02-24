package io.treehopper.desktop.demos;

import java.util.ArrayList;
import io.treehopper.desktop.*;
import io.treehopper.*;
import io.treehopper.Pin;
import io.treehopper.enums.PinMode;

public class AnalogRead {
    public static void main(String[] args) throws InterruptedException {
        while(true) {
            if(ConnectionService.getInstance().getBoards().size() == 0) continue;
            TreehopperUsb board = ConnectionService.getInstance().getBoards().get(0);
            board.connect();
            Pin adcPin = board.pins[10];
            adcPin.setMode(PinMode.AnalogInput);

            while (true) {
                double value = adcPin.getAnalogVoltage();
                System.out.println(value);
                Thread.sleep(100);
            }
        }
    }
}
