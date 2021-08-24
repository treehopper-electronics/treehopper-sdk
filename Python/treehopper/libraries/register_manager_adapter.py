import abc


class RegisterManagerAdapter(abc.ABC):
    @abc.abstractmethod
    def read(self, address, width):
        pass

    @abc.abstractmethod
    def write(self, address, data):
        pass
