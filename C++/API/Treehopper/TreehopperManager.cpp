#include "TreehopperManager.h"
#include "TreehopperBoard.h"
TreehopperManager::TreehopperManager()
{
	int r;
	r = libusb_init(NULL);
}

vector<TreehopperBoard>* TreehopperManager::ScanForDevices()
{
	ssize_t cnt;
	cnt = libusb_get_device_list(NULL, &devs);
	if (cnt < 0)
		return &BoardList;

	libusb_device *dev;
	int i = 0;
	while ((dev = devs[i++]) != NULL)
	{
		struct libusb_device_descriptor desc;
		int r = libusb_get_device_descriptor(dev, &desc);
		if (r < 0) {
			return &BoardList;
		}
		if (desc.idProduct == TreehopperBoard::pid && desc.idVendor == TreehopperBoard::vid)
		{
			BoardList.push_back(TreehopperBoard(dev));
		}
	}

	return &BoardList;
}