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
	private byte pinReportEndpoint; // javax-usb endpoint for receiving
											// pin reports
	private byte peripheralResponseEndpoint; // for receiving peripheral
													// data (i2C, SPI, etc)
	private byte pinConfigEndpoint; // ... for configuring pins
	private byte peripheralConfigEndpoint; // ... for configuring the
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

	public UsbConnection(Device device) {
		this.device = device;
		this.connected = false;
		this.pinListenerThreadRunning = false;

		// http://javax-usb.sourceforge.net/jdoc/javax/usb/UsbDevice.html
		
		// TODO: set serial number
		//			this.serialNumber = device.getSerialNumberString();
		//			this.name = device.getProductString();

	}

	public boolean open() {
		if (connected)
			return true;
		
		int result = LibUsb.open(device, deviceHandle);
		if (result != LibUsb.SUCCESS) throw new LibUsbException("Unable to open USB device", result);

		LibUsb.claimInterface(deviceHandle, 0);
		

//		UsbConfiguration configuration = device.getActiveUsbConfiguration();
//		iface = configuration.getUsbInterface((byte) 0);
//		try {
//			iface.claim();
//			connected = true;
//		} catch (UsbClaimException e) {
//			// TODO Auto-generated catch block
//			e.printStackTrace();
//		} catch (UsbNotActiveException e) {
//			// TODO Auto-generated catch block
//			e.printStackTrace();
//		} catch (UsbDisconnectedException e) {
//			// TODO Auto-generated catch block
//			e.printStackTrace();
//		} catch (UsbException e) {
//			// TODO Auto-generated catch block
//			e.printStackTrace();
//		}

		// next, get the four endpoints for Treehopper
		
		// TODO: final initializers for all these:
		pinReportEndpoint = (byte)0x81;
		peripheralResponseEndpoint = (byte)0x82;
		pinConfigEndpoint = (byte)0x01;
		peripheralConfigEndpoint = (byte)0x02;


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
		
//		return readPeripheralBuffer.array();
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

}