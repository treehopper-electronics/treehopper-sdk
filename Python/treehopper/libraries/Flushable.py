import abc


class Flushable(abc.ABC):
    def __init__(self, parent=None):
        self.auto_flush = True
        self.parent = parent

    @abc.abstractmethod
    def flush(self, force=False):
        pass
