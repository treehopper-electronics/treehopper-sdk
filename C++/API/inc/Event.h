#pragma once

#include <functional>
#include <vector>
#include <algorithm>
#include <iostream>

namespace Treehopper {
    using namespace std;

    /** Provides a simple C#-style observer pattern event queue.
    This class provides two public operators -- Event::operator+= and Event::operator-= -- which allows user code to subscribe and unsubscribe from these events.

    For example, to subscribe to the Pin::pinChanged event using a lambda expression,
    \code{.cpp}
    board.pins[4].pinChanged += [](DigitalIn& sender, PinChangedEventArgs& e) {
        cout << "Pin 4 event: " << e.newValue << endl;
    };
    \endcode

    You may also subscribe to events by passing regular named functions, i.e.:
    \code{.cpp}
    void pinHandler(DigitalIn& sender, PinChangedEventArgs& e) {
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

     <b>Note that for proper binding (and unbinding), the method signature must match precisely</b> (i.e., both parameters must be passed by reference).

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
    template<class Sender, class EventArgs>
    class Event {
        friend Sender;
    public:
        /** Create a new Event that belongs to a Sender, and takes an EventArgs event object type.
        @param[in] sender The parent object that fired this event.
        */
        Event<Sender, EventArgs>(Sender &sender) : sender(sender), handlers() {
        }

        size_t getAddress(std::function<void(Sender &, EventArgs &)> f) {
            typedef void (fnType)(Sender &, EventArgs &);
            fnType **fnPointer = f.template target<fnType *>();
            if (fnPointer == NULL) return 0;
            return (size_t) *fnPointer;
        }

        /** Subscribe to an event */
        void operator+=(function<void(Sender &sender, EventArgs &e)> handler) {
            handlers.push_back(handler);
        }

        void operator-=(function<void(Sender &sender, EventArgs &e)> handler) {
            int handlerAddress = getAddress(handler);
            if (handlerAddress == 0) // we can't get the function's address
            {
                return;
            }

            int idx = -1;
            for (int i = 0; i < handlers.size(); i++) {
                if (handlerAddress == getAddress(handlers[i])) {
                    idx = i;
                }
            }

            if (idx >= 0)
                handlers.erase(handlers.begin() + idx);
            else
                cerr << "Couldn't find an event handler to remove. Check your method's signature.";
        }

    protected:
        vector<function<void(Sender &sender, EventArgs &e)>> handlers;

        void invoke(EventArgs arg) {
            for (auto pin1inputHandler : handlers) {
                pin1inputHandler(sender, arg);
            }
        }

        Sender &sender;
    };
}

