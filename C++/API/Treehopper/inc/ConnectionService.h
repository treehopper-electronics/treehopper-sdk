#pragma once
#include "Treehopper.h"
#include "TreehopperUsb.h"
#include "UsbConnection.h"
#include <vector>

using namespace std;
namespace Treehopper 
{
	/** Provides TreehopperUsb discovery and factory duties. */
	class TREEHOPPER_API ConnectionService
	{
	public:
		ConnectionService();
		~ConnectionService();

		/** Get the ConnectionService instance to use for TreehopperUsb discovery. */
		static ConnectionService& instance()
		{
			static ConnectionService instance;
			return instance;
		}

		vector<TreehopperUsb> boards;
	private:
		void scan();
	};
}

