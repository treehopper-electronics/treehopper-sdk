package io.treehopper.desktop;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

import javax.usb.UsbDevice;
import javax.usb.UsbDeviceDescriptor;
import javax.usb.UsbException;
import javax.usb.UsbHostManager;
import javax.usb.UsbHub;

import io.treehopper.TreehopperUsb;

// This is a super simple ConnectionService. For now, just scan connected devices when the class is
// instantiated. In the future, monitor for new devices continuously

public class ConnectionService {
	// these are the Treehopper USB VID/PID
	private int vendorId = 0x10c4;
	private int productId = 0x8a7e;

	// singleton stuff so we don't need a DI
	private static final ConnectionService instance = new ConnectionService();
	
	// this stores a collection of boards; the key is the serial number
	private ArrayList<TreehopperUsb> boards = new ArrayList<TreehopperUsb>();
	
	public static ConnectionService getInstance() {
		return instance;
	}

	public ConnectionService() {
		try {
			recursiveTreehopperSearch(UsbHostManager.getUsbServices().getRootUsbHub());
		} catch (SecurityException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (UsbException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

	// We might have USB hubs attached to USB hubs, so we need to recursively
	// walk through
	// everything to make sure we find all our devices
	private void recursiveTreehopperSearch(UsbHub hub) {
		for (UsbDevice device : (List<UsbDevice>) hub.getAttachedUsbDevices()) {
			UsbDeviceDescriptor desc = device.getUsbDeviceDescriptor();
//			System.out.println("Vendor ID: " + Integer.toHexString(desc.idVendor()));
//			System.out.println("Product ID: " + Integer.toHexString(desc.idProduct()));
			if ((desc.idVendor() & 0xFFFF) == vendorId && (desc.idProduct() & 0xFFFF) == productId) {
				// we found a board!
				// make a new io.treehopper.api.javax.UsbConnection
				UsbConnection newConnection = new UsbConnection(device);
				// make a new TreehopperUsb board
				TreehopperUsb newBoard = new TreehopperUsb(newConnection);
				// add it to our hash of boards (with the serial number as the
				// key!)
//				String serialNumber = newConnection.getSerialNumber();
				boards.add(newBoard);

			} else if (( device).isUsbHub()) // we found another hub. Recursively
											// call this function
			{
				recursiveTreehopperSearch((UsbHub) device);
			}
		}
	}

	public ArrayList<TreehopperUsb> getBoards() {
		return boards;
	}

}