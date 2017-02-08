#pragma once
#include <stdint.h>
#include <vector>
#include <stdint.h>
#include <libusb.h>

using namespace std;

#ifdef TREEHOPPER_STATIC_LINK
#define TREEHOPPER_API
#else
#ifdef TREEHOPPER_EXPORTS
#define TREEHOPPER_API __declspec(dllexport)
#else
#define TREEHOPPER_API __declspec(dllimport)
#endif
#endif

class TreehopperBoard;

class TREEHOPPER_API TreehopperManager
{
public:
	TreehopperManager();
	vector<TreehopperBoard>* ScanForDevices();
private:
	libusb_device **devs;
	vector<TreehopperBoard> BoardList;
};
