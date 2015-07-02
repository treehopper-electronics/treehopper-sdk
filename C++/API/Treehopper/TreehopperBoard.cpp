#include "TreehopperBoard.h"

TreehopperBoard::TreehopperBoard(libusb_device* device)
: Pin1(this), 
Pin2(this), 
Pin3(this), 
Pin4(this), 
Pin5(this), 
Pin6(this), 
Pin7(this), 
Pin8(this), 
Pin9(this), 
Pin10(this), 
Pin11(this), 
Pin12(this), 
Pin13(this), 
Pin14(this)
{
	IsOpen = false;
	Device = device;

	libusb_open(Device, &Handle);
	unsigned char buffer[64];

	libusb_get_string_descriptor_ascii(Handle, 4, buffer, 128);
	Name = string((const char*)buffer);

	libusb_get_string_descriptor_ascii(Handle, 3, buffer, 128);
	SerialNumber = string((const char*)buffer);

	libusb_close(Handle);
	
	
}

TreehopperBoard::TreehopperBoard(string serialNumber)
: Pin1(this),
Pin2(this),
Pin3(this),
Pin4(this),
Pin5(this),
Pin6(this),
Pin7(this),
Pin8(this),
Pin9(this),
Pin10(this),
Pin11(this),
Pin12(this),
Pin13(this),
Pin14(this)
{
	libusb_init(NULL);
	libusb_device** devs;
	libusb_device *dev;
	int i = 0;
	ssize_t cnt;
	cnt = libusb_get_device_list(NULL, &devs);
	if (cnt < 0)
		return;

	while ((dev = devs[i++]) != NULL)
	{
		struct libusb_device_descriptor desc;
		int r = libusb_get_device_descriptor(dev, &desc);
		if (r < 0) {
			return;
		}
		if (desc.idProduct == pid && desc.idVendor == vid)
		{
			TreehopperBoard board(dev);
			if (board.SerialNumber == serialNumber || serialNumber.length() == 0)
			{
				Device = dev;
				Name = board.Name;
				SerialNumber = board.SerialNumber;
				return;
			}
		}
	}
}

void TreehopperBoard::Open()
{
	libusb_open(Device, &Handle);
	libusb_set_configuration(Handle, 1);
	libusb_claim_interface(Handle, 0);
	IsOpen = true;
	thread pinStateThread = thread(&TreehopperBoard::pinStateListener, this);
	pinStateThread.detach();
}

void TreehopperBoard::Close()
{
	IsOpen = false;
	libusb_close(Handle);
}

int TreehopperBoard::SendPinConfigCommand(uint8_t pinNumber, uint8_t* data, uint8_t len)
{
	PinConfigBuffer[0] = DevCmdPinConfig;
	PinConfigBuffer[1] = pinNumber;
	memcpy(&(PinConfigBuffer[2]), data, len);
	int actualLength;
	int returnCode = libusb_bulk_transfer(Handle, PinConfigEndpoint, PinConfigBuffer, 64, &actualLength, 100);
	if (returnCode < 0)
		return returnCode;
	else
		return actualLength;
}

static void LIBUSB_CALL BulkTransferCallback(struct libusb_transfer *transfer)
{
	TreehopperBoard* board = reinterpret_cast<TreehopperBoard*>(transfer->user_data);
	board->callback(transfer);
}

void TreehopperBoard::callback(struct libusb_transfer *transfer)
{
	if (transfer->status == LIBUSB_TRANSFER_COMPLETED)
	{
		if (PinStateBuffer[0] = DevCmdPinConfig)
		{
			Pin1.UpdateValue(PinStateBuffer[1], PinStateBuffer[2]);
			Pin2.UpdateValue(PinStateBuffer[3], PinStateBuffer[4]);
			Pin3.UpdateValue(PinStateBuffer[5], PinStateBuffer[6]);
			Pin4.UpdateValue(PinStateBuffer[7], PinStateBuffer[8]);
			Pin5.UpdateValue(PinStateBuffer[9], PinStateBuffer[10]);
			Pin6.UpdateValue(PinStateBuffer[11], PinStateBuffer[12]);
			Pin7.UpdateValue(PinStateBuffer[13], PinStateBuffer[14]);
			Pin8.UpdateValue(PinStateBuffer[15], PinStateBuffer[16]);
			Pin9.UpdateValue(PinStateBuffer[17], PinStateBuffer[18]);
			Pin10.UpdateValue(PinStateBuffer[19], PinStateBuffer[20]);
			Pin11.UpdateValue(PinStateBuffer[21], PinStateBuffer[22]);
			Pin12.UpdateValue(PinStateBuffer[23], PinStateBuffer[24]);
			Pin13.UpdateValue(PinStateBuffer[25], PinStateBuffer[26]);
			Pin14.UpdateValue(PinStateBuffer[27], PinStateBuffer[28]);
		}
		if (IsOpen)
			libusb_submit_transfer(transfer);
	}

}


void TreehopperBoard::pinStateListener()
{
	while (!IsOpen);
	
	transfer = libusb_alloc_transfer(0);
	libusb_fill_bulk_transfer(transfer, Handle, PinStateEndpoint, PinStateBuffer, 64, BulkTransferCallback, this, 5000);
	libusb_submit_transfer(transfer);
	while (IsOpen)
	{
		struct timeval tv;

		tv.tv_sec = 0;
		tv.tv_usec = 100; //TODO: Fix this value.

		libusb_handle_events_timeout(NULL, &tv);
	}
}
