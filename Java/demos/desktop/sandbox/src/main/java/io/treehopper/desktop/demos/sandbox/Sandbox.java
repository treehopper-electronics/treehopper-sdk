package io.treehopper.desktop.demos.sandbox;

import java.util.ArrayList;
import io.treehopper.desktop.*;
import io.treehopper.*;
import io.treehopper.libraries.displays.SevenSegmentDisplay;
import io.treehopper.libraries.displays.Tm1650;

public class Sandbox {
    public static void main(String[] args) {
        ConnectionService service = new ConnectionService();
        ArrayList<TreehopperUsb> boards = service.getBoards();
        TreehopperUsb board = boards.get(0);
        board.connect();

        Tm1650 driver = new Tm1650(board.i2c);
        SevenSegmentDisplay display = new SevenSegmentDisplay(driver.leds, false);
        display.setText("1234");
        board.disconnect();
    }
}
