#pragma once
namespace Treehopper
{
	namespace Libraries
	{
		class Pollable
		{
		public:
			Pollable();
			~Pollable();
			bool autoUpdateWhenPropertyRead;
			virtual void update() = 0;
		};
	}
}
