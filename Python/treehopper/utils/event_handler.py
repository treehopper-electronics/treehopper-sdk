"""General purpose event handling routines.

Public domain code, originally from Marcus von Appen:
https://bitbucket.org/marcusva/python-utils/overview

"""

__all__ = ["EventHandler"]


class EventHandler(object):
    """An event handling class, which manages callbacks to be executed.

    \section using Subscribing to events
    This class allows you to publish events that others can subscribe to. Multiple subscribers are supported.

    This class uses += and -= syntax for subscribing/unsubscribing to events.

    You may either declare a plain function as a handler, or you can use a lambda. Regardless, the first argument
    with which your method will be called is the "sender" of the event, which is always the instance of the class the
    %EventHandler is in (it's *not* the %EventHandler object itself).

    The raw event value(s) are published starting at the second positional argument of this event handler.

    Here's an example of subscribing to the Pin.analog_voltage_changed event using a lambda:

        >>> board = find_boards()[0]
        >>> board.connect()
        >>> pin = board.pins[0]
        >>> pin.mode = PinMode.AnalogInput
        >>> pin.analog_voltage_changed += lambda sender, value: print(value)

    You can also use plain functions as event handlers:

        >>> def my_event_handler(sender, value):
        >>>     print(value)
        >>>
        >>> pin.analog_voltage_changed += my_event_handler

    \section publishing Publishing events
    You can use this module in your own classes to publish events. Simply initialize a variable as an EventHandler:

        >>> my_event = EventHandler(self)

    Then call the event just like a function:

        >>> my_event(0.5)

    Subscribers to your event will be called in order they subscribed.
    """

    def __init__(self, sender):
        """Construct a new %EventHandler"""
        self._callbacks = []
        self._sender = sender

    def __call__(self, *args):
        """Executes all callbacks.

        Executes all connected callbacks in the order of addition,
        passing the sender of the %EventHandler as first argument and the
        optional args as second, third, ... argument to them.
        """
        return [callback(self._sender, *args) for callback in self._callbacks]

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
        return len(self._callbacks)

    def __getitem__(self, index):
        """Gets the callback at the specified index."""
        return self._callbacks[index]

    def __setitem__(self, index, value):
        """Sets the callback at the specified index."""
        self._callbacks[index] = value

    def __delitem__(self, index):
        """Deletes the callback at the specified index."""
        del self._callbacks[index]

    def add(self, callback):
        """Adds a callback to the %EventHandler."""
        if not callable(callback):
            raise TypeError("callback mus be callable")
        self._callbacks.append(callback)

    def remove(self, callback):
        """Removes a callback from the %EventHandler."""
        self._callbacks.remove(callback)
