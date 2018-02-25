import abc


class Flushable(abc.ABC):
    """
    Base class for any type of output device that can be flushed.

    Many output devices --- like LED drivers, GPIO port expanders, or displays --- are "flushable" devices. They contain
    individually-addressable components (such as individual GPIO pins): in certain applications, the user may wish
    to address them individually; while in other applications, the user may want to configure the individual components,
    and then flush the changes to the device in one fell swoop. This is more performant, but less intuitive.

    Peripherals that expose a Flushable base class let users have it both ways: by default, the device is in
    "auto_flush" mode, so any changes to the peripheral's properties are written out to the device immediately. However,
    when auto_flush is set to False, changes to the peripheral's properties are only flushed out to the device when
    the user calls the flush() method.
    """
    def __init__(self, parent=None):
        self.auto_flush = True
        """Whether changes to the peripheral should be written out immediately to the device."""
        self.parent = parent
        """The parent of this device.
        
        Many Flushable devices support chaining (e.g., shift registers chained together using SPI). In this case,
        all devices must be updated simultaneously. In general, Flushable classes are expected to function in a way 
        that hides these implementation details from the user, such that updating any particular device in the chain
        will perform the appropriate flushing to guarantee the device has the correct state.
        
        However, to reduce the chattiness on the bus, high-level classes --- such as LedCollection --- can consult the 
        parent property of the Flushable devices to determine update behavior.
        
        If the parent property is set, these high-level classes will only call the flush() method on the parent object, 
        since it is assumed that doing so will update all other devices in the chain. 
        """

    @abc.abstractmethod
    def flush(self, force=False):
        """
        Flush the current property values to the physical hardware.

        :param force: whether to force an update, even if the library thinks the hardware's current state matches the
        state defined by the properties in the class.

        This method only needs to be called when auto_flush is False (the default value is True). When auto_flush is
        True, the device will automatically update each time a property is changed. Otherwise, call flush() whenever
        you wish to update the device.

        Some libraries keep track of the state of the physical hardware, and will only update properties that do not
        appear to have changed. However, if the device is externally reset (or on initial power-up), the user should set
        force=True in the flush() command, to force-write all changes to the device.

        :return: None
        """
        pass
