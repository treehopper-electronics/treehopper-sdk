# from treehopper.api.TreehopperUsb import TreehopperUsb
from treehopper.api.DigitalPin import DigitalIn, DigitalOut
from treehopper.api.AdcPin import AdcPin

class PinMode:
    Reserved, DigitalInput, PushPullOutput, OpenDrainOutput, AnalogInput, Unassigned = range(6)


class PinConfigCommands:
    Reserved, MakeDigitalInput, MakePushPullOutput, MakeOpenDrainOutput, MakeAnalogInput, SetDigitalValue = range(6)


class ReferenceLevel:
    Vref_3V3, Vref_1V65, Vref_1V8, Vref_2V4, Vref_3V3Derived, Vref_3V6 = range(6)


class Pin(AdcPin, DigitalIn, DigitalOut):
    def __init__(self, board, pin_number : int):
        AdcPin.__init__(self, 12, 3.3)
        DigitalIn.__init__(self)
        DigitalOut.__init__(self)
        self.name = "Pin {}".format(pin_number)
        self._reference_level = ReferenceLevel.Vref_3V3  #type: ReferenceLevel
        self._board = board
        self._number = pin_number
        self._mode = PinMode.Unassigned                  # type: PinMode

    @property
    def number(self):
        return self._number

    @property
    def digital_value(self) -> bool:
        return self._digital_value

    @digital_value.setter
    def digital_value(self, value):
        self._digital_value = value
        self._board._send_pin_config([self._number, PinConfigCommands.SetDigitalValue, self._digital_value])

    @property
    def reference_level(self) -> ReferenceLevel:
        return self._reference_level

    @reference_level.setter
    def reference_level(self, reference_level: ReferenceLevel):
        if self._reference_level == reference_level:
            return

        self._reference_level = reference_level
        if self.mode == PinMode.AnalogInput:
            self.make_analog_in()

    def make_analog_in(self):
        self._board._send_pin_config([self._number, PinConfigCommands.MakeAnalogInput, self._reference_level])

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
            DigitalIn._update_value(self, b0)

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

    def __str__(self):
        if self.mode == PinMode.AnalogInput:
            return "{}: Analog input, {:0.3f} volts".format(self.name, self.analog_voltage)
        elif self.mode == PinMode.DigitalInput:
            return "{}: Digital input, {}".format(self.name, self.digital_value)
        elif self.mode == PinMode.PushPullOutput:
            return "{}: Digital push-pull, {}".format(self.name, self.digital_value)
        elif self.mode == PinMode.OpenDrainOutput:
            return "{}: Digital open-drain, {}".format(self.name, self.digital_value)
