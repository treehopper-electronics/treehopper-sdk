#pragma once
#include <stdint.h>
#include <vector>
#include <stdint.h>
#include <libusb.h>

using namespace std;

#ifdef TREEHOPPER_EXPORTS
#define EXPORT __declspec(dllexport)
#else
#define EXPORT __declspec(dllimport)
#endif

class TreehopperBoard;

class EXPORT TreehopperManager
{
public:
	TreehopperManager();
	vector<TreehopperBoard>* ScanForDevices();
private:
	libusb_device **devs;
	vector<TreehopperBoard> BoardList;
};
