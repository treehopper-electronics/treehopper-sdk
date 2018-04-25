#pragma once
#include <string>

#include "Treehopper.h"
#include <stdexcept>

using namespace std;
namespace Treehopper 
{
	class TREEHOPPER_API Utility
	{
	public:
		static void error(runtime_error& message, bool fatal = false);
		static void error(string message, bool fatal = false)
		{
            auto err = runtime_error(message);
            error(err, fatal);
		}
		static bool closeTo(double a, double b, double error = 0.001);
		static bool isBigEndian();
		static int nthOccurrence(const std::wstring& str, const std::wstring& findMe, int nth);
	};
}
