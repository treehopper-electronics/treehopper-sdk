package io.treehopper.api.android;

import android.app.PendingIntent;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.hardware.usb.*;
import android.util.Log;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Map;

import io.treehopper.api.TreehopperEventsListener;
import io.treehopper.api.TreehopperUsb;

/**
 * Created by jay on 12/27/2015.
 */
public class ConnectionService extends BroadcastReceiver {

    private static final String TAG = "ConnectionService";

    private static final ConnectionService instance = new ConnectionService();

    public static ConnectionService getInstance()
    {
        return instance;
    }

    public static final String ActionUsbPermission = "com.treehopper.library.USB_PERMISSION";

    private UsbManager manager;

    private PendingIntent pendingIntent;

    public Context getContext() {
        return context;
    }

    public void setContext(Context context) {
        this.context = context;
    }

    private Context context;

    public ConnectionService()
    {

    }

//    TreehopperApp app;
//
//    Thread appThread;
//    @Override
//    public void onReceive(Context context, Intent intent) {
//        if(intent.getAction() == ConnectionService.ActionUsbPermission) {
//            if(intent.getBooleanExtra(UsbManager.EXTRA_PERMISSION_GRANTED, false)) {
//                if(app == null)
//                    return;
//
//                final TreehopperUsb board = boards.get(0);
//                if(!app.shouldConnect(board.getName(),board.getSerialNumber()))
//                    return;
//
//                appThread = new Thread()
//                {
//                    @Override
//                    public void run() {
//                        super.run();
//                        board.connect();
//                        app.setup(board);
//                        while(board.isConnected()) {
//                            try {
//                                app.loop(board);
//                            } catch(Exception ex) {
//
//                            }
//                        }
//                    }
//                };
//                appThread.start();
////                for(BoardAddedListener listener : listeners) {
////                    listener.onBoardAdded(boards.get(0));
////                }
//            }
//        }
//    }
//
//    public void initialize(Context context, TreehopperApp theApp)
//    {
//        this.app = theApp;
//        this.context = context;
//        start();
//        final TreehopperUsb board = boards.get(0);
//        if(!app.shouldConnect(board.getName(),board.getSerialNumber()))
//            return;
//
//        appThread = new Thread()
//        {
//            @Override
//            public void run() {
//                super.run();
//                board.connect();
//                app.setup(board);
//                while(board.isConnected()) {
//                    try {
//                        app.loop(board);
//                    } catch(Exception ex) {
//
//                    }
//                }
//            }
//        };
//        appThread.start();
//    }

    private HashMap<String, TreehopperUsb> boards = new HashMap<String, TreehopperUsb>();
    public HashMap<String, TreehopperUsb> getBoards() {
        return boards;
    }

    private ArrayList<TreehopperEventsListener> listeners = new ArrayList<TreehopperEventsListener>();
    public void addEventsListener(TreehopperEventsListener listener)
    {
        if(listeners.contains(listener)) // no double-subscribing, plz
            return;
        listeners.add(listener);
    }

    public void removeEventsListener(TreehopperEventsListener listener)
    {
        if(!listeners.contains(listener))
            return;
        listeners.remove(listener);
    }


    public void deviceAdded(UsbDevice device)
    {
        createTreehopperFromDevice(device);
    }

    public void deviceRemoved(UsbDevice device)
    {
        Log.i(TAG, "deviceRemoved called");
        if(device == null) {
            // ugh, Android didn't tell us which device was removed, but if we only have one connected, we'll remove it
            if(boards.size() == 1)
            {
                Iterator<TreehopperUsb> it = boards.values().iterator();
                TreehopperUsb removedBoard = it.next();
                removedBoard.disconnect();
                for(TreehopperEventsListener listener : listeners)
                {
                    listener.onBoardRemoved(removedBoard);
                }
                boards.clear();
            }

        } else {
            if(device.getVendorId() == 0x10c4 && device.getProductId() == 0x8a7e) {
                TreehopperUsb removedBoard = boards.get(device.getSerialNumber());
                if(removedBoard == null)
                    return;
                if(!boards.containsKey(removedBoard.getSerialNumber())) // we already removed it! thanks, though!
                    return;
                removedBoard.disconnect();
                boards.remove(device.getSerialNumber());
                Log.i(TAG, "calling " + listeners.size() + " listeners");
                for(TreehopperEventsListener listener : listeners)
                {
                    listener.onBoardRemoved(removedBoard);
                }
            }
        }

    }

    private void createTreehopperFromDevice(UsbDevice device) {
        Log.i(TAG, "createTreehopperFromDevice called");
        if (device.getVendorId() == 0x10c4 && device.getProductId() == 0x8a7e) {
            if (boards.containsKey(device.getSerialNumber())) {
                Log.i(TAG, "device found. Not adding to list.");
            } else {
                Log.i(TAG, "device not found. Adding.");
                TreehopperUsb board = new TreehopperUsb(new UsbConnection(device, manager));

                boards.put(device.getSerialNumber(), board);
                Log.i(TAG, "Added new board (name=" + board.getName() + ", serial=" + board.getSerialNumber() + "). Total number of boards: " + boards.size());
                ;
                Log.i(TAG, "calling " + listeners.size() + " listeners.");
                for (TreehopperEventsListener listener : listeners) {
                    listener.onBoardAdded(board);
                }
            }
        }
    }


    @Override
    public void onReceive(Context context, Intent intent) {
        if(intent.getAction() == UsbManager.ACTION_USB_DEVICE_DETACHED)
        {
            UsbDevice usbDevice = (UsbDevice)intent.getParcelableExtra(UsbManager.EXTRA_DEVICE);
            deviceRemoved(usbDevice);
        }
    }

    public void scan() {
            if(context == null)
        return;

//        pendingIntent = PendingIntent.getBroadcast(context, 0, new Intent(ActionUsbPermission), 0);
//        IntentFilter filter = new IntentFilter(ActionUsbPermission);
//        context.registerReceiver(this, filter);
        manager = (UsbManager)context.getSystemService(context.USB_SERVICE);
        HashMap<String, UsbDevice> deviceList = manager.getDeviceList();
        for(Map.Entry<String, UsbDevice> entry : deviceList.entrySet()) {
            UsbDevice device = entry.getValue();
            createTreehopperFromDevice(device);
        }
    }

//    public void start() {
//        if(context == null)
//            return;
////            throw new Exception("You must call setContext with the application's context before starting the connection manager");
//
////        pendingIntent = PendingIntent.getBroadcast(context, 0, new Intent(ActionUsbPermission), 0);
////        IntentFilter filter = new IntentFilter(ActionUsbPermission);
////        context.registerReceiver(this, filter);
//        manager = (UsbManager)context.getSystemService(context.USB_SERVICE);
//        HashMap<String, UsbDevice> deviceList = manager.getDeviceList();
//        for(Map.Entry<String, UsbDevice> entry : deviceList.entrySet()) {
//            UsbDevice device = entry.getValue();
//            if(device.getVendorId() == 0x10c4 && device.getProductId() == 0x8a7e) {
//                boards.add(new TreehopperUsb(new UsbConnection(device, manager)));
//
//
////                manager.requestPermission(device, pendingIntent);
//
//            }
//        }
//    }
}
