#pragma once
#include <functional>
#ifdef TREEHOPPER_EXPORTS
#define EXPORT __declspec(dllexport)
#else
#define EXPORT __declspec(dllimport)
#endif
using namespace std;

template<typename T>
class Property
{
public:
	function<T()> Getter;
	function<void(T)> Setter;
	void TestMethod();
	// setter
	Property<T>& operator=(T const &val)
	{
		if(Setter != NULL)
			Setter(val);
		return *this;
	};

	operator const T&() const
	{
		if (Getter != NULL)
			return Getter();
		else
			return NULL;
	};

private:

};