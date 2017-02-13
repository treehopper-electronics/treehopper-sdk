#pragma once
#include "Treehopper.h"
#include "TreehopperUsb.h"
#include "UsbConnection.h"
#include <vector>

using namespace std;
namespace Treehopper 
{
	class TREEHOPPER_API ConnectionService
	{
	public:
		ConnectionService();
		~ConnectionService();
		vector<TreehopperUsb> boards;
	private:
		void scan();

	};
}

