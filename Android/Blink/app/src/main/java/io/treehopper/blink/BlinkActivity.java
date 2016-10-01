package io.treehopper.blink;

import android.os.Bundle;
import android.support.design.widget.FloatingActionButton;
import android.support.design.widget.Snackbar;
import android.support.v7.widget.Toolbar;
import android.util.Log;
import android.view.View;
import android.view.Menu;
import android.view.MenuItem;

import io.treehopper.api.*;
import io.treehopper.api.android.TreehopperActivity;

public class BlinkActivity extends TreehopperActivity {

    TreehopperUsb board;
    Thread app;

    private static final String TAG = "BlinkApp";

    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_blink);
        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);
//        FloatingActionButton fab = (FloatingActionButton) findViewById(R.id.fab);
//        fab.setOnClickListener(new View.OnClickListener() {
//            @Override
//            public void onClick(View view) {
//                Snackbar.make(view, "Replace with your own action", Snackbar.LENGTH_LONG)
//                        .setAction("Action", null).show();
//            }
//        });
    }

    public String hello = "hello world";

    @Override
    protected void onStop() {
        super.onStop();
        if (app != null)
            app.interrupt();
    }

    @Override
    protected void boardAdded(TreehopperUsb newBoard) {
        this.board = newBoard;
        if (!board.connect())
            return;
        app = new Thread() {
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
        };
        app.start();
    }

    @Override
    protected void boardRemoved(TreehopperUsb board) {
        this.board = null;
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_blink, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();

        //noinspection SimplifiableIfStatement
        if (id == R.id.action_settings) {
            return true;
        }

        return super.onOptionsItemSelected(item);
    }
}
