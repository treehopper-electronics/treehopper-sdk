#pragma once
#include <string>
#include "Treehopper.h"

using namespace std;
namespace Treehopper 
{
	class TREEHOPPER_API Utility
	{
	public:
		static void error(runtime_error message);
		static bool closeTo(double a, double b, double error = 0.001);
	};
}
