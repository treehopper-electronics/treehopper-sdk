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

	bool Utility::isBigEndian() {
		static const uint16_t m_endianCheck(0x00ff);
		return ( *((uint8_t*)&m_endianCheck) == 0x0);
	}
}
