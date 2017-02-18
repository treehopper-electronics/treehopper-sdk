#pragma once

#include <functional>
#include <vector>
#include <algorithm>

namespace Treehopper
{
	using namespace std;
	
	template<class Sender, class EventArgs>

	class Event
	{
		friend Sender;
	public:
		Event<Sender, EventArgs>(Sender* sender) : handlers()
		{
			this->sender = sender;
		}

		void operator+=(function<void(Sender* sender, EventArgs e)> pin1inputHandler)
		{
			handlers.push_back(pin1inputHandler);
		}

		void operator-=(function<void(Sender* sender, EventArgs e)> pin1inputHandler)
		{
			int idx = -1;
			for(int i=0;i<handlers.size();i++)
			{
				void* testHandlerAddress = *(handlers[i]).target<void(*)(Sender* sender, EventArgs e)>();
				void* handlerAddress = *(pin1inputHandler).target<void(*)(Sender* sender, EventArgs e)>();
				if (testHandlerAddress == handlerAddress)
					idx = i;
			}
			if (idx >= 0)
				handlers.erase(handlers.begin() + idx);
		}

	protected:
		vector<function<void(Sender* sender, EventArgs e)>> handlers;

		void invoke(EventArgs arg)
		{
			for (auto pin1inputHandler : handlers)
			{
				pin1inputHandler(sender, arg);
			}
		}

		Sender* sender;
	};
}

