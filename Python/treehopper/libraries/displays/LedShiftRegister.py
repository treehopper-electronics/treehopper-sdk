from treehopper.libraries.displays import LedDriver
from treehopper.libraries.io.expander.ChainableShiftRegisterOutput import ChainableShiftRegisterOutput


class LedShiftRegister(ChainableShiftRegisterOutput, LedDriver):
    def __init__(self):
        super().__init__()
