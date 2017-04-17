#pragma once

#include <functional>
#include <vector>
#include <algorithm>

namespace Treehopper
{
	using namespace std;
	
	/** Provides a simple C#-style observer pattern event queue.
	This class provides two public operators -- Event::operator+= and Event::operator-= -- which allows user code to subscribe and unsubscribe from these events.

	For example, to subscribe to the Pin::pinChanged event using a lambda expression,
	\code{.cpp}
	board.pins[4].pinChanged += [](DigitalIn* sender, PinChangedEventArgs e) {
		cout << "Pin 4 event: " << e.newValue << endl;
	};
	\endcode

	You may also subscribe to events by passing regular named functions, i.e.:
	\code{.cpp}
	void pinHandler(DigitalIn* sender, PinChangedEventArgs e) {
	cout << "Pin 4 event: " << e.newValue << endl;
	}

	void setup() {
		board.pins[4].pinChanged += pinHandler;
		...
	}
	\endcode

	If you wish to unsubscribe to an event, pass the same function to Event::operator-=:
	\code{.cpp}
	...

	void done() {
		board.pins[4].pinChanged -= pinHandler;
	}
	\endcode

	This is all that is needed to consume events; if you wish to use this class in your own code to publish events, start by creating an EventArgs struct with all the parameters you wish to pass to subscribers. By convention, we use the suffix "EventArgs" in the name of the struct, however this is not required:
	\code{.cpp}
	struct ReadingReceivedEventArgs
	{
	public:
		double newValue;
	};
	\endcode
	Next, add an Event member to your class:
	\code{.cpp}
	class MyClass
	{
		Event<MyClass, ReadingReceivedEventArgs> readingReceived;
	}
	\endcode

	When you actually want to fire an event, use the Event::invoke() method:
	\code{.cpp}
		ReadingReceivedEventArgs event;
		event.newValue = getData();
		readingReceived.invoke(event);
	\endcode
	*/
    template<class Sender, class EventArgs> class Event
	{
		friend Sender;
	public:
		/** Create a new Event that belongs to a Sender, and takes an EventArgs event object type. 
		@param[in] sender The parent object that fired this event.
		*/
		Event<Sender, EventArgs>(Sender& sender) : sender(sender), handlers()
		{
		}

		/** Subscribe to an event */
		void operator+=(function<void(Sender& sender, EventArgs& e)> handler)
		{
			handlers.push_back(handler);
		}

		void operator-=(function<void(Sender& sender, EventArgs& e)> handler)
		{
			int idx = -1;
			for (int i = 0; i < handlers.size(); i++ )
			{
//				void* testHandlerAddress = *(handlers[i]).target<void(*)(Sender& sender, EventArgs& e)>();
//				void* handlerAddress = *(handler).target<void(*)(Sender& sender, EventArgs& e)>();
                void* testHandlerAddress = *(handlers[i]).target();
                void* handlerAddress = *(handler).target();
				if (testHandlerAddress == handlerAddress)
					idx = i;
			}
			if (idx >= 0)
				handlers.erase(handlers.begin() + idx);
		}

	protected:
		vector<function<void(Sender& sender, EventArgs& e)>> handlers;

		void invoke(EventArgs arg)
		{
			for (auto pin1inputHandler : handlers)
			{
				pin1inputHandler(sender, arg);
			}
		}

		Sender& sender;
	};
}

