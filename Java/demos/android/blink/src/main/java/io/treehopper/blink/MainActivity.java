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
        boardAdded.setLed(true);
        boardAdded.setLed(false);
    }

    @Override
    protected void boardRemoved(TreehopperUsb boardRemoved) {

    }
}
