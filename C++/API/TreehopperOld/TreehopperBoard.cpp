#include "TreehopperBoard.h"


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
	Pins[0] = &Pin1;
	Pins[1] = &Pin2;
	Pins[2] = &Pin3;
	Pins[3] = &Pin4;
	Pins[4] = &Pin5;
	Pins[5] = &Pin6;
	Pins[6] = &Pin7;
	Pins[7] = &Pin8;
	Pins[8] = &Pin9;
	Pins[9] = &Pin10;
	Pins[10] = &Pin11;
	Pins[11] = &Pin12;
	Pins[12] = &Pin13;
	Pins[13] = &Pin14;
	


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
			// Get the name and serial number
			libusb_device_handle* candidate;

			libusb_open(dev, &candidate);
			unsigned char buffer[64];

			libusb_get_string_descriptor_ascii(candidate, 4, buffer, 128);
			string name = string((const char*)buffer);

			libusb_get_string_descriptor_ascii(candidate, 3, buffer, 128);
			string serial = string((const char*)buffer);

			libusb_close(candidate);


			if (serial == serialNumber || serialNumber.length() == 0)
			{
				Device = dev;
				Name = name;
				SerialNumber = serial;
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
	libusb_release_interface(Handle, 0);
	libusb_close(Handle);
	libusb_exit(NULL);
	
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
