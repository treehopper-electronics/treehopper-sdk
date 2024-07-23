from abc import abstractmethod, ABC


class Pollable(ABC):
    def __init__(self):
        ## determines whether reading from a property will invoke update()
        #
        # When True, reading from a property will tacitly call update() --- a potentially slow operation.
        # When False, make sure to manually invoke update() before reading from this sensor's properties.
        self.auto_update_when_property_read = True

    @abstractmethod
    def update(self):
        """Request an update from the sensor"""
        pass
