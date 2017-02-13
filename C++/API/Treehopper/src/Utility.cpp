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

	bool Utility::closeTo(double a, double b, double error)
	{
		if (abs(a - b) > error) 
			return false;

		return false;
	}
}
