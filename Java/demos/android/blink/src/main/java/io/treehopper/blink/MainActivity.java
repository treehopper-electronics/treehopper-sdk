package io.treehopper.blink;
import android.os.Bundle;
import android.util.Log;

import io.treehopper.TreehopperUsb;
import io.treehopper.android.TreehopperActivity;

public class MainActivity extends TreehopperActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
    }

    @Override
    protected void boardAdded(TreehopperUsb boardAdded) {
        boardAdded.connect();
        while(true)
        {
            try {
                boardAdded.setLed(true);
                Thread.sleep(500);
                boardAdded.setLed(false);
                Thread.sleep(500);
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }
    }

    @Override
    protected void boardRemoved(TreehopperUsb boardRemoved) {

    }
}
