package io.treehopper.desktop.demos;

import io.treehopper.*;
import io.treehopper.desktop.*;
import java.util.ArrayList;

public class Blink {
    public static void main(String[] args) {

        System.out.println("Hello World");

        ConnectionService service = new ConnectionService();
        ArrayList<TreehopperUsb> boards = service.getBoards();
        TreehopperUsb board = boards.get(0);
        board.connect();


        while(true){
            board.setLed(!board.isLed());
            try {
                Thread.sleep(100);
            } catch (InterruptedException e) {
                // TODO Auto-generated catch block
                e.printStackTrace();
            }
        }
    }
}
