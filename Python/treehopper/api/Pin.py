from treehopper.api.DigitalPin import DigitalIn, DigitalOut
from treehopper.api.AdcPin import AdcPin
from treehopper.api.Pwm import Pwm
from abc import abstractmethod
from typing import TYPE_CHECKING
if TYPE_CHECKING:
    from treehopper.api.TreehopperUsb import TreehopperUsb


class PinMode:
    Reserved, DigitalInput, PushPullOutput, OpenDrainOutput, AnalogInput, SoftPwm, Unassigned = range(7)


class PinConfigCommands:
    Reserved, MakeDigitalInput, MakePushPullOutput, MakeOpenDrainOutput, MakeAnalogInput, SetDigitalValue = range(6)


class ReferenceLevel:
    Vref_3V3, Vref_1V65, Vref_1V8, Vref_2V4, Vref_3V3Derived, Vref_3V6 = range(6)


class SpiChipSelectPin(DigitalOut):
    @property
    @abstractmethod
    def spi_module(self):
        pass

    @property
    @abstractmethod
    def number(self):
        pass


class Pin(AdcPin, DigitalIn, SpiChipSelectPin, Pwm):
    def __init__(self, board: 'TreehopperUsb', pin_number: int):
        AdcPin.__init__(self, 12, 3.3)
        DigitalIn.__init__(self)
        DigitalOut.__init__(self)
        self.name = "Pin {}".format(pin_number)
        self._reference_level = ReferenceLevel.Vref_3V3  #type: ReferenceLevel
        self._board = board
        self._number = pin_number
        self._mode = PinMode.Unassigned                  # type: PinMode

    @property
    def spi_module(self):
        return self._board.spi

    @property
    def number(self):
        """Get the pin number"""
        return self._number

    @property
    def digital_value(self) -> bool:
        """Digital value for the pin.

        :type: bool        
        :getter: Returns the last digital value received
        :setter: Sets the pin's value immediately
        """
        return self._digital_value

    @digital_value.setter
    def digital_value(self, value):
        self._digital_value = value
        self._board._send_pin_config([self._number, PinConfigCommands.SetDigitalValue, self._digital_value])

    @property
    def reference_level(self) -> ReferenceLevel:
        """Reference level for the pin.

        :type: ReferenceLevel        
        :getter: Returns the current ReferenceLevel
        :setter: Sets the desired ReferenceLevel
        """
        return self._reference_level

    @reference_level.setter
    def reference_level(self, reference_level: ReferenceLevel):
        if self._reference_level == reference_level:
            return

        self._reference_level = reference_level
        if self.mode == PinMode.AnalogInput:
            self.make_analog_in()

    def make_analog_in(self):
        """Make the pin an analog input"""
        self._board._send_pin_config([self._number, PinConfigCommands.MakeAnalogInput, self._reference_level])
        self._mode = PinMode.AnalogInput

    def make_digital_open_drain_out(self):
        """Make the pin an open-drain output"""
        self._board._send_pin_config([self._number, PinConfigCommands.MakeOpenDrainOutput])
        self._mode = PinMode.OpenDrainOutput

    def make_digital_push_pull_out(self):
        """Make the pin a push-pull output"""
        self._board._send_pin_config([self._number, PinConfigCommands.MakePushPullOutput])
        self._mode = PinMode.PushPullOutput

    def make_digital_in(self):
        """Make the pin a digital input"""
        self._board._send_pin_config([self._number, PinConfigCommands.MakeDigitalInput])
        self._mode = PinMode.DigitalInput

    def _update_value(self, b0, b1):
        if self.mode == PinMode.AnalogInput:
            AdcPin._update_value(self, b0 << 8 | b1)

        elif self.mode == PinMode.DigitalInput:
            DigitalIn._update_value(self, b0)

    @property
    def mode(self) -> PinMode:
        """The pin's mode.

        :type: PinMode
        :getter: Returns the current mode
        :setter: Sets the pin's mode
        """
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

        elif value == PinMode.SoftPwm:
            self.enable_pwm()

    @property
    def duty_cycle(self):
        return self._board._soft_pwm_manager.get_duty_cycle(self)

    @duty_cycle.setter
    def duty_cycle(self, value):
        self._board._soft_pwm_manager.set_duty_cycle(self, value)

    @property
    def pulse_width(self):
        return self._board._soft_pwm_manager.get_pulse_width(self)

    @pulse_width.setter
    def pulse_width(self, value):
        self._board._soft_pwm_manager.set_pulse_width(self, value)

    def enable_pwm(self):
        self._mode = PinMode.SoftPwm
        self._board._send_pin_config([self._number, PinConfigCommands.MakePushPullOutput])  #
        self._board._soft_pwm_manager.start_pin(self)

    def __str__(self):
        if self.mode == PinMode.AnalogInput:
            return "{}: Analog input, {:0.3f} volts".format(self.name, self.analog_voltage)
        elif self.mode == PinMode.DigitalInput:
            return "{}: Digital input, {}".format(self.name, self.digital_value)
        elif self.mode == PinMode.PushPullOutput:
            return "{}: Digital push-pull, {}".format(self.name, self.digital_value)
        elif self.mode == PinMode.OpenDrainOutput:
            return "{}: Digital open-drain, {}".format(self.name, self.digital_value)
