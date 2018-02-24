package io.treehopper.desktop.demos;

import io.treehopper.*;
import io.treehopper.desktop.*;
import java.util.ArrayList;

public class Blink {
    public static void main(String[] args) throws InterruptedException {
        while(true)
        {
            if(ConnectionService.getInstance().getBoards().size() == 0) continue;
            TreehopperUsb board = ConnectionService.getInstance().getBoards().get(0);
            board.connect();
            while(board.getConnected()){
                board.setLed(!board.isLed());
                Thread.sleep(100);
            }
        }
    }
}
