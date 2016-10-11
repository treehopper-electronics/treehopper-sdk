package example.io.treehopper.api.java;

import java.util.ArrayList;
import java.util.HashMap;

import io.treehopper.api.java.ConnectionService;
import io.treehopper.api.java.UsbConnection;
import io.treehopper.api.Pin;
import io.treehopper.api.PinMode;
import io.treehopper.api.TreehopperUsb;

public class test {

	public static void main(String[] args) {
		// TODO Auto-generated method stub
		System.out.println("Hello World");

		ConnectionService service = new ConnectionService();
		ArrayList<TreehopperUsb> boards = service.getBoards();
		TreehopperUsb hopper = boards.get(0);
		hopper.connect();
//		while(true){
//			hopper.setLed(true);
//			try {
//				Thread.sleep(1000);
//			} catch (InterruptedException e) {
//				// TODO Auto-generated catch block
//				e.printStackTrace();
//			}
//			hopper.setLed(false);
//			try {
//				Thread.sleep(1000);
//			} catch (InterruptedException e) {
//				// TODO Auto-generated catch block
//				e.printStackTrace();
//			}
//		}
		Pin pin = hopper.Pins[10];
		pin.setMode(PinMode.PushPullOutput);
//		while(true){
//			pin.setDigitalValue(true);
//			try {
//				Thread.sleep(1000);
//			} catch (InterruptedException e) {
//				// TODO Auto-generated catch block
//				e.printStackTrace();
//			}
//			pin.setDigitalValue(false);
//			try {
//				Thread.sleep(1000);
//			} catch (InterruptedException e) {
//				// TODO Auto-generated catch block
//				e.printStackTrace();
//			}
//		}
		while(true){
			Pin pin2 = hopper.Pins[1];
			pin2.setMode(PinMode.AnalogInput);
			double value = pin2.getAdcValue();
			System.out.println(value);
			try {
				Thread.sleep(100);
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
	}

}
