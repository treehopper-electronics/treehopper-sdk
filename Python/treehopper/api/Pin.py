# from treehopper.api.TreehopperUsb import TreehopperUsb
from treehopper.api.DigitalPin import DigitalIn, DigitalOut
from treehopper.api.AdcPin import AdcPin
from treehopper.api.PinConfigCommands import PinConfigCommands
from treehopper.api.PinMode import PinMode

class Pin(AdcPin, DigitalIn, DigitalOut):
    def __init__(self, board, pin_number : int):
        AdcPin.__init__(self, 12, 3.3)
        DigitalIn.__init__(self)
        DigitalOut.__init__(self)

        self._board = board
        self._number = pin_number
        self._mode = PinMode.Unassigned  # type: PinMode

    @property
    def digital_value(self) -> bool:
        return self._digital_value

    @digital_value.setter
    def digital_value(self, value):
        self._digital_value = value
        self._board._send_pin_config([self._number, PinConfigCommands.SetDigitalValue, self._digital_value])

    def make_analog_in(self):
        self._board._send_pin_config([self._number, PinConfigCommands.MakeAnalogInput])

    def make_digital_open_drain_out(self):
        self._board._send_pin_config([self._number, PinConfigCommands.MakeOpenDrainOutput])

    def make_digital_push_pull_out(self):
        self._board._send_pin_config([self._number, PinConfigCommands.MakePushPullOutput])

    def make_digital_in(self):
        self._board._send_pin_config([self._number, PinConfigCommands.MakeDigitalInput])

    def _update_value(self, b0, b1):
        if self.mode == PinMode.AnalogInput:
            AdcPin._update_value(self, b0 << 8 | b1)

        elif self.mode == PinMode.DigitalInput:
            self._digital_value = b0

    @property
    def mode(self) -> PinMode:
        return self._mode

    @mode.setter
    def mode(self, value: PinMode):
        if self._mode == value:
            return

        if self._mode == PinMode.Reserved and value != PinMode.Unassigned:
            raise ValueError("This pin is reserved; you must disable the peripheral using it before interacting with it")

        self._mode = value

        if value == PinMode.AnalogInput:
            self.make_analog_in()

        elif value == PinMode.DigitalInput:
            self.make_digital_in()

        elif value == PinMode.OpenDrainOutput:
            self.make_digital_open_drain_out()
            self._digital_value = False

        elif value == PinMode.PushPullOutput:
            self.make_digital_push_pull_out()
            self._digital_value = False
