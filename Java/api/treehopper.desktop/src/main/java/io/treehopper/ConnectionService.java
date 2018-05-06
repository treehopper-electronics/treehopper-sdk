package io.treehopper;

import java.util.ArrayList;
import java.util.List;

import org.usb4java.*;

import io.treehopper.TreehopperUsb;

public class ConnectionService {
	// these are the Treehopper USB VID/PID
	private int result;

	private static final Context context  = new Context();

	// singleton stuff so we don't need a DI
	private static final ConnectionService instance = new ConnectionService();
	
	// this stores a collection of boards; the key is the serial number
	private ArrayList<TreehopperUsb> boards = new ArrayList<TreehopperUsb>();

    /**
     * Gets the ConnectionService instance for use
     * @return the ConnectionService instance to access
     */
	public static ConnectionService getInstance() {
		return instance;
	}

	Boolean isRunning = true;
	Thread eventThread;

	HotplugCallbackHandle myBogusCallbackHandle = new HotplugCallbackHandle();

	public ConnectionService() {
		result = LibUsb.init(context);

	    updateBoards();

		if (!LibUsb.hasCapability(LibUsb.CAP_HAS_HOTPLUG))
		{
			System.err.println("libusb doesn't support hotplug on this system. Call update() to update board list.");
		}

		eventThread = new Thread() {
			@Override
			public void run() {
				LibUsb.hotplugRegisterCallback(context, LibUsb.HOTPLUG_EVENT_DEVICE_ARRIVED | LibUsb.HOTPLUG_EVENT_DEVICE_LEFT, LibUsb.HOTPLUG_ENUMERATE, LibUsb.HOTPLUG_MATCH_ANY, LibUsb.HOTPLUG_MATCH_ANY, LibUsb.HOTPLUG_MATCH_ANY,  myCallback, null, myBogusCallbackHandle);

				while(isRunning) {
					LibUsb.handleEvents(context);
				}
			}
		};

		eventThread.start();
	}

	HotplugCallback myCallback = new HotplugCallback() {
		@Override
		public int processEvent(Context context, Device device, int event, Object userData) {
			if (event == LibUsb.HOTPLUG_EVENT_DEVICE_ARRIVED)
			{
				TreehopperUsb board = new TreehopperUsb(new UsbConnection(device));
				System.err.print("Adding: " + board);
				boards.add(board);
			}
			else if (event == LibUsb.HOTPLUG_EVENT_DEVICE_LEFT)
			{
				for(TreehopperUsb board : boards)
				{
					if(((UsbConnection)(board.getConnection())).getDevice() == device)
					{
						System.err.print("Removing: " + board);
						boards.remove(board);
					}
				}
			}

			return 0;
		}
	};

	public void updateBoards() {
		boards.clear();
		
		if (result != LibUsb.SUCCESS) throw new LibUsbException("Unable to initialize libusb.", result);

		DeviceList list = new DeviceList();
	    result = LibUsb.getDeviceList(context, list);
	    if (result < 0) throw new LibUsbException("Unable to get device list", result);

	    try
	    {
	        // Iterate over all devices and scan for the right one
	        for (Device device: list)
	        {
	            DeviceDescriptor descriptor = new DeviceDescriptor();
	            result = LibUsb.getDeviceDescriptor(device, descriptor);
	            if (result != LibUsb.SUCCESS) throw new LibUsbException("Unable to read device descriptor", result);
	            if (descriptor.idVendor() == (short)TreehopperUsb.Settings.getVid() && descriptor.idProduct() == (short)TreehopperUsb.Settings.getPid())
	            {
	            	LibUsb.refDevice(device);
	            	TreehopperUsb board = new TreehopperUsb(new UsbConnection(device));
	            	boards.add(board);
	            }
	        }
	    }
	    finally
	    {
	        // Ensure the allocated device list is freed
//	        LibUsb.freeDeviceList(list, true);
	    }
	}

	/**
	 * (Desktop) Gets a list of boards attached to this device
	 * @return a list of boards
	 */
	public List<TreehopperUsb> getBoards() {
		return boards;
	}

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

	@Override
	protected void finalize() throws Throwable {
		super.finalize();
		LibUsb.exit(context);
	}
}