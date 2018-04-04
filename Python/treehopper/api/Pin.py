from abc import abstractmethod
from threading import Lock
from typing import TYPE_CHECKING, Dict

from treehopper.api.device_commands import DeviceCommands
from treehopper.api.interfaces import DigitalOut, Pwm, AdcPin, DigitalIn

if TYPE_CHECKING:
    from treehopper.api.treehopper_usb import TreehopperUsb, Pin


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
    """
    Built-in I/O pins

    \section quick Quick guide
    Once you have connected to a TreehopperUsb board, you can access pins through the \link TreehopperUsb.pins
    pins\endlink property of the board.

    You can manipulate pins directly:

        >>> board = find_boards()[0]
        >>> board.connect()
        >>> board.pins[3].mode = PinMode.PushPullOutput
        >>> board.pins[3].digital_value = True

    Or create reference variables:

        >>> board = find_boards()[0]
        >>> board.connect()
        >>> red_led = board.pins[7]
        >>> red_led.mode = PinMode.OpenDrainOutput
        >>> red_led.digital_value = False

    You can choose whether a pin should be a digital input, digital output, analog input, or soft-PWM output by
    setting the pin's \link Pin.mode mode\endlink property to one of the values in PinMode.

    You can set or retrieve the digital value of a pin by accessing the \link Pin.digital_value digital_value\endlink
    property. Note that writing to this property --- even if the pin is an input --- will implicitly change it into
    an output.

    If the pin is set as an analog input, you can access its data through any of the following properties:
     - \link Pin.analog_value analog_value\endlink: retrieve a normalized (0.0 - 1.0) pin value
     - \link Pin.analog_voltage analog_voltage\endlink: retrieve the voltage (0.0 - 3.3) on the pin
     - \link Pin.adc_value adc_value\endlink: retrieve the raw ADC value (0 - 4095) of the pin

    \section more More information
    This section dives into more details and electrical characteristics about %Treehopper's pins.

    \subsection mode Pin mode
    You can choose whether a pin should be a digital input, output, or analog input by setting the pin's Mode property.

    \subsection output Digital outputs
    All pins on %Treehopper support both push-pull and open-drain outputs. Writing a true or false to the pin's
    digital value will flush that value to the pin.
     - **Push-Pull**: Push-pull is the most commonly used output mode; when a pin is set to true, %Treehopper will
     attempt to drive the pin to logic HIGH (3.3V) — when a pin is set to false, %Treehopper will attempt to drive
     the pin to logic LOW (0V — ground).
     - **Open-Drain**: Open-drain outputs can only drive a strong logic LOW (0V); in the HIGH state, the pin is
     weakly pulled high.

    \subsubsection current Output current limitations
    %Treehopper's output impedance varies, but is roughly 100 ohm source and 50 ohm sink when supplying weaker loads,
    but increases as the load increases. In the worst-case scenario (when short-circuited), %Treehopper can source
    approximately 20 mA of current, and sink approximately 40 mA of current. The pin's drivers are rated for a
    maximum of 100 mA of output current, so you cannot damage the board by short-circuiting its output to ground or
    3.3V.

    While this is plenty of current for peripheral ICs and small indicator LEDs, do not expect to drive large arrays
    of LEDs, or low-impedance loads like motors, solenoids, or speakers directly from %Treehopper's pins. There are a
    wide variety of peripherals in the Treehopper.Libraries package for your language API that can be used for
    interfacing with these peripherals.

    \warning **To avoid damaging the device permanently, do not source or sink more than 400 mA of combined current
    out of the pins on the board!** Note that these limits have nothing to do with the 3.3V supply pins found on
    %Treehopper, which can comfortably source 500 mA --- or the unfused 5V pin, which has no imposed current limit (
    other than that of your computer).

    \subsection input Digital input
    %Treehopper's digital inputs are used to sample digital signals — i.e., signals that have either a <i>LOW</i> or
    <i>HIGH</i> state. Logic LOW (false) is considered a voltage less than or equal to 0.6V. Logic HIGH (true) is
    considered a voltage greater than or equal to 2.7V.

    %Treehopper pins are true 5V-tolerant signals; consequently, you do not need any sort of logic-level conversion
    or series-limiting resistor when using the pin as a digital input with a 5V source.

    You can access the most recent \link Pin.digital_value digital_value\endlink, or use the \link
    Pin.digital_value_changed digital_value_changed\endlink event to subscribe to change notifications.

    \subsection analog Analog inputs
    Each Treehopper pin can be read using the on-board 12-bit ADC. There is no limit to the total number of analog
    pins activated at any time.

    \subsubsection out Output Format
    When the pin is sampled and sent to the host, the value is simultaneously available to the user in three forms:
     - \link Pin.adc_value adc_value\endlink -- the raw, 12-bit result from conversion.
     - \link Pin.analog_value analog_value\endlink -- the normalized value of the ADC (from 0-1).
     - \link Pin.analog_voltage analog_voltage\endlink -- the actual voltage at the pin (taking into account the
     reference level).

    There are OnChanged events associated with each of these properties:
     - \link Pin.analog_voltage_changed analog_voltage_changed\endlink
     - \link Pin.analog_value_changed analog_value_changed\endlink
     - \link Pin.adc_value_changed adc_value_changed\endlink

    Plus thresholds for each of these events that give you fine-grained control over when the event will fire:
     - \link Pin.analog_voltage_threshold analog_voltage_threshold\endlink
     - \link Pin.analog_value_threshold analog_value_threshold\endlink
     - \link Pin.adc_value_threshold adc_value_threshold\endlink

    \subsubsection Reference Levels
    Each pin has a configurable \link Pin.reference_level reference_level\endlink that can be used to measure the pin
    against. The possible reference levels are:
     - 3.3V generated by the on-board LDO, rated at 1.5% accuracy (default).
     - 3.7V (effective) reference derived from the on-chip 1.85V reference.
     - 2.4V on-chip reference rated at 2.1% accuracy.
     - 1.85V on-chip reference.
     - 1.65V on-chip reference, 1.8% accurate.
     - 3.3V (effective) reference that is derived from the on-chip 1.65V reference.

    For most ratiometric applications --- i.e., when measuring a device whose output is ratioed to its power supply
    --- connect the sensor's power supply to the 3.3V supply pin the %Treehopper and use the default 3.3V reference.
    The other reference options are provided for advanced scenarios that involve reading from precision voltage
    outputs accurately.

    \subsection reads A note about pin reads
    All of %Treehopper's pins configured as digital or analog inputs are sampled continuously onboard; when any pin
    changes, this data is sent to the host device. When you access the digital or one of the analog value properties,
    you're accessing the last received data. This makes property reads instantaneous --- keeping your GUI or
    application running responsively.

    For almost all applications, changes to digital or analog inputs are to be reacted to (like with switches,
    interrupt outputs, encoders), or sampled (like with sensor outputs). Care must be taken, however, if you need to
    synchronize pin reads with other functions.

    For example, consider the case where you electrically short pins 0 and 1 together on the board, and then run this
    code:

        >>> pin0 = board.pins[0]
        >>> pin1 = board.pins[1]

        >>> pin0.mode = PinMode.PushPullOutput
        >>> pin1.mode = PinMode.DigitalInput

        >>> pin0.digital_value = 0
        >>> if pin1.digital_value == pin0.digital_value:
        >>>     # we generally won't get here, since pin1's DigitalValue
        >>>     # isn't explicitly read from the pin when we access it; it only returns
        >>>     # the last value read from a separate pin-reading thread
        >>>     pass


    A work around is to wait for two consecutive pin updates to be received before checking the pin's value. This can
    be accomplished by awaiting TreehopperUsb.AwaitPinUpdateAsync().

    However, pin updates are only sent to the computer when a pin's value changes, so if you wish to synchronously
    sample a pin that might not change, you should set an unused pin as an analog input, which will almost certainly
    guarantee a constant stream of pin updates:


        >>> board.pins[19].mode = PinMode.AnalogInput  # this will ensure we get continuous pin updates

        >>> pin0 = board.pins[0]
        >>> pin1 = board.pins[1]

        >>> pin0.Mode = PinMode.PushPullOutput
        >>> pin1.Mode = PinMode.DigitalInput

        >>> pin0.DigitalValue = 0
        >>> board.await_pin_update()  # this first report may have been captured before the output was written
        >>> board.await_pin_update()  # this report should have the effects of the digital output in it
        >>> if pin1.DigitalValue == pin0.DigitalValue:
        >>>     # we should always get here
        >>>     pass

    \section softpwm SoftPWM functionality
    Each %Treehopper pin can be used as a SoftPWM pin.

    \section performance Performance Considerations
    Writing values to (or changing pin modes of) Treehopper pins will flush to the OS's USB layer immediately,
    but there is no way of achieving guaranteed latency.

    Occasional writes (say, on the order of every 20 ms or more) will usually flush to the port within a few hundred
    microseconds. If your application is chatty, or the bus you're operating on has other devices (especially
    isochronous devices like webcams), you may see long periods (a millisecond or more) of delay.

    Analog pins take a relatively long time to sample; if you enable tons of analog inputs, the effective sampling
    rate will drop by up to two times.

    """

    ## \cond PRIVATE
    def __init__(self, board: 'TreehopperUsb', pin_number: int):
        AdcPin.__init__(self, 12, 3.3)
        DigitalIn.__init__(self)
        DigitalOut.__init__(self)
        self.name = "Pin {}".format(pin_number)
        self._reference_level = ReferenceLevel.Vref_3V3  # type: ReferenceLevel
        self._board = board
        self._number = pin_number
        self._mode = PinMode.Unassigned  # type: PinMode

    ## \endcond
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
            raise ValueError(
                "This pin is reserved; you must disable the peripheral using it before interacting with it")

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
        """Gets or sets the duty cycle of the PWM pin, from 0.0-1.0."""
        return self._board._soft_pwm_manager.get_duty_cycle(self)

    @duty_cycle.setter
    def duty_cycle(self, value):
        self._board._soft_pwm_manager.set_duty_cycle(self, value)

    @property
    def pulse_width(self):
        """Gets or sets the pulse width, in ms, of the pin."""
        return self._board._soft_pwm_manager.get_pulse_width(self)

    @pulse_width.setter
    def pulse_width(self, value):
        self._board._soft_pwm_manager.set_pulse_width(self, value)

    def enable_pwm(self):
        """Enable the PWM functionality of this pin."""
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


class SoftPwmPinConfig:
    def __init__(self):
        self.duty_cycle = 0
        self.pulse_width = 0
        self.use_pulse_width = True
        self.ticks = 0
        self.pin = []  # type: Pin


class SoftPwmManager:
    def __init__(self, board: "TreehopperUsb"):
        self._board = board
        self._pins: Dict[int, SoftPwmPinConfig] = dict()
        self._lock = Lock()
        self._resolution = 0.25

    def start_pin(self, pin: "Pin"):
        if pin in self._pins.keys():
            return

        config = SoftPwmPinConfig()
        config.pin = pin

        self._pins[pin.number] = config
        self.update_config()

    def stop_pin(self, pin: "Pin"):
        del self._pins[pin.number]
        self.update_config()

    def set_duty_cycle(self, pin: "Pin", duty: float):
        with self._lock:
            self._pins[pin.number].duty_cycle = duty
            self._pins[pin.number].use_pulse_width = False
            self.update_config()

    def set_pulse_width(self, pin: "Pin", pulse_width: float):
        with self._lock:
            self._pins[pin.number].pulse_width = pulse_width
            self._pins[pin.number].use_pulse_width = False
            self.update_config()

    def get_duty_cycle(self, pin: "Pin"):
        if not self._pins[pin.number]:
            return 0

        return self._pins[pin.number].duty_cycle

    def get_pulse_width(self, pin: "Pin"):
        if not self._pins[pin.number]:
            return 0

        return self._pins[pin.number].pulse_width

    def update_config(self):
        if len(self._pins) > 0:
            for config in self._pins.values():
                if config.use_pulse_width:
                    config.ticks = config.pulse_width / self._resolution
                    config.duty_cycle = config.ticks / 65535

                else:
                    config.ticks = config.duty_cycle * 65535
                    config.pulse_width = config.ticks * self._resolution

            ordered_values = list(self._pins.values())
            ordered_values.sort(key=lambda x: x.ticks)

            count = len(ordered_values) + 1
            config = [DeviceCommands.SoftPwmConfig, count]

            time = 0

            for j in range(count):
                if j < len(ordered_values):
                    ticks = ordered_values[j].ticks - time
                else:
                    ticks = 65535 - time

                tmr_val = int(65535 - ticks)
                if j == 0:
                    config.append(0)
                else:
                    config.append(ordered_values[j - 1].pin.number)

                config.append(tmr_val >> 8)
                config.append(tmr_val & 0xff)
                time += ticks

            self._board._send_peripheral_config_packet(config)

        else:
            self._board._send_peripheral_config_packet([DeviceCommands.SoftPwmConfig, 0])
