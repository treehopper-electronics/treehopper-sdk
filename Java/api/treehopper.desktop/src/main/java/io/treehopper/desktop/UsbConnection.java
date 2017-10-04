package io.treehopper.desktop;

import io.treehopper.TreehopperUsb;

import java.io.UnsupportedEncodingException;
import java.nio.ByteBuffer;
import java.nio.IntBuffer;

import javax.usb.UsbAbortException;
import javax.usb.UsbClaimException;
import javax.usb.UsbConfiguration;
import javax.usb.UsbDevice;
import javax.usb.UsbDisconnectedException;
import javax.usb.UsbEndpoint;
import javax.usb.UsbException;
import javax.usb.UsbInterface;
import javax.usb.UsbNotActiveException;
import javax.usb.UsbNotClaimedException;
import javax.usb.UsbNotOpenException;
import javax.usb.UsbPipe;
import org.usb4java.*;

import io.treehopper.interfaces.Connection;

/**
 * javax-usb Connection
 */
public class UsbConnection implements Connection {

	private Device device; // private reference to the javax-usb device
	
	private DeviceHandle deviceHandle = new DeviceHandle();
	
	// interface
	private UsbInterface iface;

	// endpoints
	private final byte pinReportEndpoint = (byte)0x81; // javax-usb endpoint for receiving
											// pin reports
	private final byte peripheralResponseEndpoint = (byte)0x82; // for receiving peripheral
													// data (i2C, SPI, etc)
	private final byte pinConfigEndpoint = (byte)0x01; // ... for configuring pins
	private final byte peripheralConfigEndpoint = (byte)0x02; // ... for configuring the
													// peripheral (including
													// LED)
	
	private ByteBuffer pinListenerThreadBuffer;
	private ByteBuffer sendConfigBuffer;
	private ByteBuffer sendPeripheralBuffer;
	private ByteBuffer readPeripheralBuffer;
	

	private Thread pinListenerThread;
	private boolean pinListenerThreadRunning;
	private TreehopperUsb board;
	private boolean connected;
	
	private String serialNumber;
	private String name;
	
	private int result;

	public UsbConnection(Device device) {
		this.device = device;
		this.connected = false;
		this.pinListenerThreadRunning = false;
//		this.serialNumber = "";
//		this.name = "";
		
		int result = LibUsb.open(device, deviceHandle);
		if (result != LibUsb.SUCCESS) throw new LibUsbException("Unable to open USB device", result);
		
		DeviceDescriptor descriptor = new DeviceDescriptor();
		int result1 = LibUsb.getDeviceDescriptor(device, descriptor);
		if(result1 != LibUsb.SUCCESS) throw new LibUsbException("Unable to read device descriptor", result1);
//
		String serialNo = LibUsb.getStringDescriptor(deviceHandle, descriptor.iSerialNumber());
		String name = LibUsb.getStringDescriptor(deviceHandle, descriptor.iProduct());
		
		this.serialNumber = serialNo;
		this.name = name;
		
		LibUsb.close(deviceHandle);
	}

	public boolean open() {
		if (connected)
			return true;
		
		int result = LibUsb.open(device, deviceHandle);
		if (result != LibUsb.SUCCESS) throw new LibUsbException("Unable to open USB device", result);

		LibUsb.claimInterface(deviceHandle, 0);
		// we need to start a thread to constantly read from the pinReportEndpoint
		pinListenerThread = new Thread() {
			@Override
			public void run() {
				IntBuffer numBytesTransfered = IntBuffer.allocate(1);
				
				
				pinListenerThreadBuffer = ByteBuffer.allocateDirect(41);
				pinListenerThreadRunning = true;
				while (pinListenerThreadRunning) {
					
					LibUsb.bulkTransfer(deviceHandle,
				               pinReportEndpoint,
				               pinListenerThreadBuffer,
				               numBytesTransfered,
				               1000);
					if (board != null && numBytesTransfered.get(0) == 41) {
						byte[] byteData = new byte[41];
						pinListenerThreadBuffer.rewind();
						pinListenerThreadBuffer.get(byteData);
						board.onPinReportReceived(byteData);
					}
				}
			}
		};
		pinListenerThread.start();
		return connected;
	}

	public void sendDataPinConfigChannel(byte[] data) {
		sendConfigBuffer = ByteBuffer.allocateDirect(data.length);
		sendConfigBuffer.put(data);
		IntBuffer numBytesTransfered = IntBuffer.allocate(1);
		LibUsb.bulkTransfer(deviceHandle,
	               pinConfigEndpoint,
	               sendConfigBuffer,
	               numBytesTransfered,
	               1000);
	}

	public void sendDataPeripheralChannel(byte[] data) {
		
		sendPeripheralBuffer = ByteBuffer.allocateDirect(data.length);
		sendPeripheralBuffer.put(data);
		IntBuffer numBytesTransfered = IntBuffer.allocate(1);
		LibUsb.bulkTransfer(deviceHandle,
	               peripheralConfigEndpoint,
	               sendPeripheralBuffer,
	               numBytesTransfered,
	               1000);
	}

	@Override
	public byte[] readPeripheralResponsePacket(int numBytesToRead) {
		readPeripheralBuffer = ByteBuffer.allocateDirect(numBytesToRead);
		IntBuffer numBytesTransfered = IntBuffer.allocate(1);
		LibUsb.bulkTransfer(deviceHandle,
	               peripheralResponseEndpoint,
	               readPeripheralBuffer,
	               numBytesTransfered,
	               1000);
		
		byte[] byteData = new byte[numBytesToRead];
		readPeripheralBuffer.rewind();
		readPeripheralBuffer.get(byteData);
		return byteData;
	}

	public boolean isConnected() {
		return connected;
	}

	public void close() {
		pinListenerThreadRunning = false;
//		pinReportPipe.abortAllSubmissions(); // TODO

		try {
			pinListenerThread.join(); // wait for the thread to finish
		} catch (InterruptedException e) {
			e.printStackTrace();
		}

		connected = false;
		
		LibUsb.releaseInterface(deviceHandle, 0);
		LibUsb.close(deviceHandle);
	}

	// these three methods should be refactored into base class
	public String getSerialNumber() {
		return serialNumber;
	}
	public String getName() {
		return name;
	}
	public void setPinReportListener(TreehopperUsb board) {
		this.board = board;
	}

	public void setSerialNumber(String serialNumber) {
		this.serialNumber = serialNumber;
	}

	public void setName(String name) {
		this.name = name;
	}

}