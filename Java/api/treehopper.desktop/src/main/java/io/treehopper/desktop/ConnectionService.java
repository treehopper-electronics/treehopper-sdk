package io.treehopper.desktop;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

import javax.usb.UsbDevice;
import javax.usb.UsbDeviceDescriptor;
import javax.usb.UsbException;
import javax.usb.UsbHostManager;
import javax.usb.UsbHub;

import org.usb4java.*;

import io.treehopper.TreehopperUsb;

/**
 * javax-usb ConnectionService
 */
public class ConnectionService {
	// these are the Treehopper USB VID/PID
	private short vendorId = (short)0x10c4;
	private short productId = (short)0x8a7e;
	private int result;

	private static final Context context  = new Context();

	// singleton stuff so we don't need a DI
	private static final ConnectionService instance = new ConnectionService();
	
	// this stores a collection of boards; the key is the serial number
	private ArrayList<TreehopperUsb> boards = new ArrayList<TreehopperUsb>();
	
	public static ConnectionService getInstance() {
		return instance;
	}
	
	public ConnectionService() {
		result = LibUsb.init(context);
	    updateBoards();
	}

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
	            if (descriptor.idVendor() == vendorId && descriptor.idProduct() == productId) 
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

	public ArrayList<TreehopperUsb> getBoards() {
//		updateBoards();
		return boards;
	}

}