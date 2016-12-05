package io.treehopper.desktop.demos.sandbox;

import java.util.ArrayList;
import io.treehopper.desktop.*;
import io.treehopper.*;

public class Sandbox {
    public static void main(String[] args) {
        ConnectionService service = new ConnectionService();
        ArrayList<TreehopperUsb> boards = service.getBoards();
        TreehopperUsb board = boards.get(0);
        board.connect();

        board.i2c.setEnabled(true);

        board.i2c.sendReceive((byte)0x24, new byte[] { (byte)0x01 }, (byte)0);
        board.i2c.sendReceive((byte)0x25, new byte[] { (byte)0x01 }, (byte)0);
        board.i2c.sendReceive((byte)0x26, new byte[] { (byte)0x01 }, (byte)0);
        board.i2c.sendReceive((byte)0x27, new byte[] { (byte)0x01 }, (byte)0);

        board.i2c.sendReceive((byte)0x34, new byte[] { (byte)0xff }, (byte)0);
        board.i2c.sendReceive((byte)0x35, new byte[] { (byte)0xff }, (byte)0);
        board.i2c.sendReceive((byte)0x36, new byte[] { (byte)0xff }, (byte)0);
        board.i2c.sendReceive((byte)0x37, new byte[] { (byte)0xff }, (byte)0);
    }
}
