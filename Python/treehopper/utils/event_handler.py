"""General purpose event handling routines.

Public domain code, originally from Marcus von Appen:
https://bitbucket.org/marcusva/python-utils/overview

"""

__all__ = ["EventHandler"]


class EventHandler(object):
    """A simple event handling class, which manages callbacks to be
    executed.

    Attributes:
        callbacks: a list of callbacks that have registered to this event
        sender: the sender (captured by the constructor)

    \section using Subscribing to events
    This class uses C# syntax for subscribing/unsubscribing to events. You may either declare a plain function as a handler, or you can use a lambda.

    Regardless, the first argument with which your method will be called is the "sender" of the event, which is always the class the %EventHandler is instantiated inside (it's *not* the %EventHandler object itself).

    The second argument is the event. Unlike our strongly-typed EventArgs classes in C#, Java, and C++, this event class is more Pythonic: raw event values are published as the second argument of this event handler, removing the need to unpack the event.

    Because this is not strongly typed, be sure to read the documentation so you know what type to expect.

    Here's an example of subscribing to the Pin.analog_voltage_changed event using a lambda:

        >>> board = find_boards()[0]
        >>> board.connect()
        >>> pin = board.pins[0]
        >>> pin.mode = PinMode.AnalogInput
        >>> pin.analog_voltage_changed += lambda sender, value: print(value)

    Though you could just as easily pass the name of a function in place of the `lambda` in-line statement. Imagine having a ``, then passing it to the event:

        >>> def my_event_handler(sender, value):
        >>>     print(value)
        >>>
        >>> pin.analog_voltage_changed += my_event_handler

    \section publishing Publishing events
    You can use this module in your own classes to publish events. Simply initialize a variable as an    EventHandler:

        >>> my_event = EventHandler(self)

    Then call the event just like a function:

        >>> my_event(0.5)

    """

    def __init__(self, sender):
        """Construct a new %EventHandler"""
        self.callbacks = []
        self.sender = sender

    def __call__(self, *args):
        """Executes all callbacks.

        Executes all connected callbacks in the order of addition,
        passing the sender of the %EventHandler as first argument and the
        optional args as second, third, ... argument to them.
        """
        return [callback(self.sender, *args) for callback in self.callbacks]

    def __iadd__(self, callback):
        """Adds a callback to the %EventHandler."""
        self.add(callback)
        return self

    def __isub__(self, callback):
        """Removes a callback from the %EventHandler."""
        self.remove(callback)
        return self

    def __len__(self):
        """Gets the amount of callbacks connected to the %EventHandler."""
        return len(self.callbacks)

    def __getitem__(self, index):
        """Gets the callback at the specified index."""
        return self.callbacks[index]

    def __setitem__(self, index, value):
        """Sets the callback at the specified index."""
        self.callbacks[index] = value

    def __delitem__(self, index):
        """Deletes the callback at the specified index."""
        del self.callbacks[index]

    def add(self, callback):
        """Adds a callback to the %EventHandler."""
        if not callable(callback):
            raise TypeError("callback mus be callable")
        self.callbacks.append(callback)

    def remove(self, callback):
        """Removes a callback from the %EventHandler."""
        self.callbacks.remove(callback)
