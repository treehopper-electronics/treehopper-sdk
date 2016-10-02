package com.example.analogread;

import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;

import io.treehopper.api.PinMode;
import io.treehopper.api.TreehopperUsb;
import io.treehopper.api.android.TreehopperActivity;

public class MainActivity extends TreehopperActivity {

    private static final String TAG = "AnalogRead";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
    }

    TreehopperUsb board;

    @Override
    protected void boardAdded(TreehopperUsb boardAdded) {
        board = boardAdded;

        board.connect();
        if (!board.connect())
            return;

        new Thread() {
            @Override
            public void run() {
                Log.i(TAG, "Starting app with " + board.getSerialNumber());
                super.run();
                board.connect();
                board.Pins[0].setMode(PinMode.AnalogInput);
                while (board != null) {
                    board.setLed(!board.getLed());
                    Log.i(TAG, "Value: " + board.Pins[0].getAnalogValue());
                    try {
                        Thread.sleep(100);
                    } catch (Exception ex) {
                    }
                }
            }
        }.start();
    }

    @Override
    protected void boardRemoved(TreehopperUsb boardRemoved) {
        if(board == boardRemoved)
            board = null;
    }
}
