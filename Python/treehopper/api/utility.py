from treehopper.api.settings import Settings


class Utility:
    @staticmethod
    def error(message, fatal = False):
        print(message)

        if Settings.throw_exceptions:
            raise RuntimeError(message)