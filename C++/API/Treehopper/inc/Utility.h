#pragma once
#include <string>
#include "Treehopper.h"

using namespace std;
namespace Treehopper 
{
	class TREEHOPPER_API Utility
	{
	public:
		static void error(runtime_error& message, bool fatal = false);
		static void error(string message, bool fatal = false)
		{
			error(runtime_error(message), fatal);
		}
		static bool closeTo(double a, double b, double error = 0.001);
	};
}
