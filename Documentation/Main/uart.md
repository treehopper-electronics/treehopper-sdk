\page uart UART
The UART peripheral allows you to send and receive standard-format RS-232-style asynchronous serial communications. 

## Pins
When the UART is enabled, the following pins will be unavailable for other use:
 - <b>TX</b> <i>(Transmit)</i>: This pin carries data from Treehopper to the device you've attached to the UART.
 - <b>RX</b> <i>(Receive)</i>: This pin carries data from the device to Treehopper.

Note that UART cross-over is a common problem when people are attaching devices together; always consult the documentation for the device you're attaching to Treehopper to ensure that the TX signal from Treehopper is flowing into the receive input (RX, DIN, etc) of the device, and vice-versa. Since you are unlikely to damage either device by misconnecting, it is a common troubleshooting practice to simply swap TX and RX if the system doesn't appear to be functioning properly.

## Implementation Details
Treehopper's UART is designed for average baud rates; the range of supported rates is 7813 baud to 2.4 Mbaud, though communication will be less reliable above 1-2 Mbaud.

Transmitting should be relatively straightforward; simply pass a byte array --- up to 63 characters long --- to the Send() function once the UART is enabled. Consult the appropriate language API for details.

Receiving data is more challenging, since Treehopper does not control when attached devices will send data. As soon as the UART peripheral is enabled, data reception begins. Treehopper can buffer up to 32 bytes of data before the host has to intervene to retreive it. 

At low bit-rates, it is plausible to receive data continuously without interruption. For example, at 9600 baud, the Read() function only need to finish execution every 33 milliseconds, which can easily be accomplished in most operating systems.

However, it's important to note that Treehopper's UART is not designed to fully replace standard CDC-class USB-to-serial converters; if 