#include "stdafx.h"
#include "Utility.h"
#include "Settings.h"
#include <iostream>
namespace Treehopper 
{
	void Utility::error(runtime_error message)
	{
		if (Settings::getInstance().throwExceptions)
		{
			throw message;
		}

		if(Settings::getInstance().printExceptions)
		{
			cerr << message.what();
		}
	}
}
