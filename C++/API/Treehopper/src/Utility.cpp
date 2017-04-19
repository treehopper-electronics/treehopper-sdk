#include "Utility.h"
#include "Settings.h"
#include <iostream>
#include <cmath>
namespace Treehopper 
{
	void Utility::error(runtime_error& message, bool fatal)
	{
		if (Settings::instance().throwExceptions || fatal)
		{
			throw message;
		}

		if(Settings::instance().printExceptions)
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
