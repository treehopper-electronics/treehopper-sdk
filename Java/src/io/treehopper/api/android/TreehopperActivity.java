package io.treehopper.api.android;

import android.content.Intent;
import android.content.IntentFilter;
import android.hardware.usb.UsbDevice;
import android.hardware.usb.UsbManager;
import android.os.Bundle;
import android.support.v7.app.AppCompatActivity;

import io.treehopper.api.TreehopperEventsListener;
import io.treehopper.api.TreehopperUsb;

/**
 * Created by jay on 12/28/2015.
 */
public abstract class TreehopperActivity extends AppCompatActivity {

    protected ConnectionService connectionService = ConnectionService.getInstance();

    TreehopperEventsListener listener = new TreehopperEventsListener() {
        @Override
        public void onBoardAdded(TreehopperUsb board) {
            boardAdded(board);
        }

        @Override
        public void onBoardRemoved(TreehopperUsb board) {
            boardRemoved(board);
        }
    };

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        connectionService.setContext(getApplicationContext());
    }

    @Override
    protected void onStop() {
        super.onStop();
        connectionService.removeEventsListener(listener);
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        connectionService.removeEventsListener(listener);
    }

    @Override
    protected void onPause() {
        super.onPause();
        unregisterReceiver(connectionService);
        connectionService.removeEventsListener(listener);
    }

    @Override
    protected void onStart() {
        super.onStart();
        connectionService.addEventsListener(listener);
        connectionService.scan();
    }

    @Override
    protected void onResume() {
        super.onResume();

        IntentFilter filter = new IntentFilter();
        filter.addAction(UsbManager.ACTION_USB_DEVICE_DETACHED);
        registerReceiver(connectionService, filter);

        connectionService.addEventsListener(listener);
        connectionService.scan();

        Intent intent = getIntent();
        if (intent != null)
        {
            if (intent.getAction().equals(UsbManager.ACTION_USB_DEVICE_ATTACHED))
            {
                UsbDevice usbDevice = (UsbDevice)intent.getParcelableExtra(UsbManager.EXTRA_DEVICE);
                ConnectionService.getInstance().deviceAdded(usbDevice);
            }
        }
    }

    protected abstract void boardAdded(TreehopperUsb boardAdded);
    protected abstract void boardRemoved(TreehopperUsb boardRemoved);


}
