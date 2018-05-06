package io.treehopper;

import android.app.PendingIntent;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.hardware.usb.*;
import android.util.Log;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Map;

import io.treehopper.events.TreehopperEventsHandler;
import io.treehopper.TreehopperUsb;

/** Discovers %Treehopper boards attached to your device.
 This documentation set covers the %ConnectionService class found in both **Treehopper.Desktop**, and **Treehopper.Android** packages.

 \note %ConnectionService should always be accessed through its singleton property, getInstance(). Do not create instances of %ConnectionService yourself.

 ## Basic usage
 There are two ways to access discovered boards. If you simply want to wait until the first Treehopper board is attached
 to the computer, the getFirstDevice() method will return an awaitable task with a result that contains the board:

 \code{.java}
 TreehopperUsb board = ConnectionService.getInstance().getFirstDevice();
 \endcode

 ## Advanced usage
 For simple applications, you can retrieve a board instance with getFirstDevice(), however, if you'd like to present the user with a list of devices from which to choose, you can reference the getBoards() property.

 \warning Board discovery on Android is done asynchronously, so getBoards() will generally return an empty collection on application start-up. Inherit from TreehopperActivity or manually add the ConnectionService callback plumbing to receive boardAdded and boardRemoved callbacks.
 */
public class ConnectionService extends BroadcastReceiver {

    private static final String TAG = "ConnectionService";

    private static final ConnectionService instance = new ConnectionService();

    /**
     * Gets the ConnectionService instance for use
     * @return the ConnectionService instance to access
     */
    public static ConnectionService getInstance() {
        return instance;
    }

    public static final String ActionUsbPermission = "io.treehopper.android.USB_PERMISSION";

    private UsbManager manager;

    private PendingIntent pendingIntent;

    public Context getContext() {
        return context;
    }

    public void setContext(Context context) {
        this.context = context;
    }

    private Context context;

    public ConnectionService() {

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

    /**
     * (Android) Gets a HashMap of boards connected to this device
     * @return a HashMap of boards
     */
    public HashMap<String, TreehopperUsb> getBoards() {
        return boards;
    }

    /**
     * Gets the first device attached
     * @return the first attached TreehopperUsb board
     *
     * This function will block until a device is attached
     */
    public TreehopperUsb getFirstDevice()
    {
        while(boards.size() == 0)
        {
            try {
                Thread.sleep(100);
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }

        return boards.get(0);
    }

    private ArrayList<TreehopperEventsHandler> listeners = new ArrayList<TreehopperEventsHandler>();

    public void addEventsListener(TreehopperEventsHandler listener) {
        if (listeners.contains(listener)) // no double-subscribing, plz
            return;
        listeners.add(listener);
    }

    public void removeEventsListener(TreehopperEventsHandler listener) {
        if (!listeners.contains(listener))
            return;
        listeners.remove(listener);
    }


    public void deviceAdded(UsbDevice device) {
        createTreehopperFromDevice(device);
    }

    public void deviceRemoved(UsbDevice device) {
        Log.i(TAG, "deviceRemoved called");
        if (device == null) {
            // ugh, Android didn't tell us which device was removed, but if we only have one connected, we'll remove it
            if (boards.size() == 1) {
                Iterator<TreehopperUsb> it = boards.values().iterator();
                TreehopperUsb removedBoard = it.next();
                removedBoard.disconnect();
                for (TreehopperEventsHandler listener : listeners) {
                    listener.onBoardRemoved(removedBoard);
                }
                boards.clear();
            }

        } else {
            if (device.getVendorId() == 0x10c4 && device.getProductId() == 0x8a7e) {
                TreehopperUsb removedBoard = boards.get(device.getSerialNumber());
                if (removedBoard == null)
                    return;
                if (!boards.containsKey(removedBoard.getSerialNumber())) // we already removed it! thanks, though!
                    return;
                removedBoard.disconnect();
                boards.remove(device.getSerialNumber());
                Log.i(TAG, "calling " + listeners.size() + " listeners");
                for (TreehopperEventsHandler listener : listeners) {
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


                manager.requestPermission(device, pendingIntent);

            }
        }
    }

    @Override
    public void onReceive(Context context, Intent intent) {
        if (intent.getAction() == UsbManager.ACTION_USB_DEVICE_DETACHED) {
            UsbDevice usbDevice = (UsbDevice) intent.getParcelableExtra(UsbManager.EXTRA_DEVICE);
            deviceRemoved(usbDevice);
        }

        if (intent.getAction() == UsbManager.ACTION_USB_DEVICE_ATTACHED) {
            UsbDevice usbDevice = (UsbDevice) intent.getParcelableExtra(UsbManager.EXTRA_DEVICE);
            createTreehopperFromDevice(usbDevice);
        }

        if (intent.getAction() == ActionUsbPermission) {
            synchronized (this) {
                UsbDevice device = (UsbDevice) intent.getParcelableExtra(UsbManager.EXTRA_DEVICE);

                if (intent.getBooleanExtra(UsbManager.EXTRA_PERMISSION_GRANTED, false)) {
                    if (device != null) {
                        Log.i(TAG, "calling " + listeners.size() + " listeners.");
                        for (TreehopperEventsHandler listener : listeners) {
                            listener.onBoardAdded(boards.get(device.getSerialNumber()));
                        }
                    }
                } else {
                    Log.d(TAG, "permission denied for device " + device);
                }
            }
        }
    }

    public void scan() {
        if (context == null)
            return;

        pendingIntent = PendingIntent.getBroadcast(context, 0, new Intent(ActionUsbPermission), 0);
        IntentFilter filter = new IntentFilter(ActionUsbPermission);
        context.registerReceiver(this, filter);

        manager = (UsbManager) context.getSystemService(context.USB_SERVICE);
        HashMap<String, UsbDevice> deviceList = manager.getDeviceList();
        for (Map.Entry<String, UsbDevice> entry : deviceList.entrySet()) {
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
