package io.treehopper.desktop;

import io.treehopper.TreehopperUsb;

import java.io.UnsupportedEncodingException;

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

import io.treehopper.interfaces.Connection;

public class UsbConnection implements Connection {

	private UsbDevice device; // private reference to the javax-usb device

	// interface
	private UsbInterface iface;

	// endpoints
	private UsbEndpoint pinReportEndpoint; // javax-usb endpoint for receiving
											// pin reports
	private UsbEndpoint peripheralResponseEndpoint; // for receiving peripheral
													// data (i2C, SPI, etc)
	private UsbEndpoint pinConfigEndpoint; // ... for configuring pins
	private UsbEndpoint peripheralConfigEndpoint; // ... for configuring the
													// peripheral (including
													// LED)

	// pipes
	private UsbPipe pinReportPipe;
	private UsbPipe peripheralResponsePipe;
	private UsbPipe pinConfigPipe;
	private UsbPipe peripheralConfigPipe;

	private Thread pinListenerThread;
	private boolean pinListenerThreadRunning;
	private TreehopperUsb board;
	private boolean connected;
	
	private String serialNumber;
	private String name;

	public UsbConnection(UsbDevice device) {
		this.device = device;
		this.connected = false;
		this.pinListenerThreadRunning = false;

		// http://javax-usb.sourceforge.net/jdoc/javax/usb/UsbDevice.html
		try {
			this.serialNumber = device.getSerialNumberString();
			this.name = device.getProductString();
		} catch (UnsupportedEncodingException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (UsbDisconnectedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (UsbException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

	public boolean open() {
		if (connected)
			return true;

		UsbConfiguration configuration = device.getActiveUsbConfiguration();
		iface = configuration.getUsbInterface((byte) 0);
		try {
			iface.claim();
			connected = true;
		} catch (UsbClaimException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (UsbNotActiveException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (UsbDisconnectedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (UsbException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

		// next, get the four endpoints for Treehopper
		pinReportEndpoint = iface.getUsbEndpoint((byte)0x81);
		peripheralResponseEndpoint = iface.getUsbEndpoint((byte)0x82);
		pinConfigEndpoint = iface.getUsbEndpoint((byte)0x01);
		peripheralConfigEndpoint = iface.getUsbEndpoint((byte)0x02);

		// in JavaX-Usb, communication happens through pipes, so get those
		pinReportPipe = pinReportEndpoint.getUsbPipe();
		peripheralResponsePipe = peripheralResponseEndpoint.getUsbPipe();
		pinConfigPipe = pinConfigEndpoint.getUsbPipe();
		peripheralConfigPipe = peripheralConfigEndpoint.getUsbPipe();

		// before we communicate, we have to open the pipes
		try {
			pinReportPipe.open();
			peripheralResponsePipe.open();
			pinConfigPipe.open();
			peripheralConfigPipe.open();
		} catch (UsbNotActiveException e) {
			e.printStackTrace();
		} catch (UsbNotClaimedException e) {
			e.printStackTrace();
		} catch (UsbDisconnectedException e) {
			e.printStackTrace();
		} catch (UsbException e) {
			e.printStackTrace();
		}


		// we need to start a thread to constantly read from the pinReportEndpoint
		pinListenerThread = new Thread() {
			@Override
			public void run() {
				byte[] data = new byte[41];
				pinListenerThreadRunning = true;
				while (pinListenerThreadRunning) {
					try {
						int received = pinReportPipe.syncSubmit(data);
					} catch (UsbNotActiveException e) {
						e.printStackTrace();
					} catch (UsbNotOpenException e) {
						e.printStackTrace();
					} catch (IllegalArgumentException e) {
						e.printStackTrace();
					} catch (UsbDisconnectedException e) {
						e.printStackTrace();
					} catch (UsbException e) {
						e.printStackTrace();
					}
					if (board != null)
						board.onPinReportReceived(data);
				}
			}
		};
		pinListenerThread.start();
		return connected;
	}

	public void sendDataPinConfigChannel(byte[] data) {
		try {
			pinConfigPipe.syncSubmit(data);
		} catch (UsbNotActiveException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (UsbNotOpenException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IllegalArgumentException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (UsbDisconnectedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (UsbException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}

	public void sendDataPeripheralChannel(byte[] data) {
		try {
			peripheralConfigPipe.syncSubmit(data);
		} catch (UsbNotActiveException e) {
			e.printStackTrace();
		} catch (UsbNotOpenException e) {
			e.printStackTrace();
		} catch (IllegalArgumentException e) {
			e.printStackTrace();
		} catch (UsbDisconnectedException e) {
			e.printStackTrace();
		} catch (UsbException e) {
			e.printStackTrace();
		}
	}

	@Override
	public byte[] readPeripheralResponsePacket(int numBytesToRead) {
		byte[] data = new byte[numBytesToRead];
		try {
			int received = peripheralResponsePipe.syncSubmit(data);
		} catch (UsbNotActiveException e) {
			e.printStackTrace();
		} catch (UsbNotOpenException e) {
			e.printStackTrace();
		} catch (IllegalArgumentException e) {
			e.printStackTrace();
		} catch (UsbDisconnectedException e) {
			e.printStackTrace();
		} catch (UsbException e) {
			e.printStackTrace();
		}
		return data;
	}

	public boolean isConnected() {
		return connected;
	}

	public void close() {
		pinListenerThreadRunning = false;
		try {
			pinListenerThread.join(); // wait for the thread to finish
		} catch (InterruptedException e) {
			e.printStackTrace();
		}

		connected = false;

		try {
			pinReportPipe.close();
			peripheralResponsePipe.close();
			pinConfigPipe.close();
			peripheralConfigPipe.close();

			iface.release();
		} catch (UsbNotActiveException e) {
			e.printStackTrace();
		} catch (UsbNotOpenException e) {
			e.printStackTrace();
		} catch (UsbDisconnectedException e) {
			e.printStackTrace();
		} catch (UsbException e) {
			e.printStackTrace();
		}
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