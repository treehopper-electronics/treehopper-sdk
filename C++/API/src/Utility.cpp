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

	// Credit: https://stackoverflow.com/questions/18972258/index-of-nth-occurrence-of-the-string
	int Utility::nthOccurrence(const std::wstring& str, const std::wstring& findMe, int nth)
	{
		size_t  pos = 0;
		int     cnt = 0;

		while (cnt != nth)
		{
			pos += 1;
			pos = str.find(findMe, pos);
			if (pos == std::string::npos)
				return -1;
			cnt++;
		}
		return pos;
	}
}
