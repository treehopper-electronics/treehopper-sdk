from treehopper.api import I2C, DigitalIn, EventHandler
from treehopper.api.utility import Utility
from treehopper.libraries import SMBusDevice
from treehopper.libraries.sensors import Pollable
from treehopper.libraries.sensors.optical.isl29125_registers import Isl29125Registers, SampleDepths, Ranges, \
    InterruptSelections, Modes
from treehopper.libraries.smbus_register_manager_adapter import SMBusRegisterManagerAdapter


class Isl29125(Pollable):
    """Renesas ISL29125 RGB light sensor The Renesas ISL29125 is a light sensor that outputs 12-bit or 16-bit red,
    green, and blue lux readings. The sensor has two ranges — 5.7 mlux to 375 lux, and 0.152 lux to 10,000 lux.

    You can access the values of each color component through the #red, #green, and #blue properties. These have already
    been converted from raw ADC values into actual lux.

    By default, the driver will operate in 16-bit mode in the 0. 152 to 10,000 lux range, sampling all three
    channels. The per-channel 16-bit sample time is more than 100 ms, so it can take almost a third of a second to
    get an RGB reading from the sensor in its default configuration. To increase the sampling rate, you can change the #sample_depth
    property to decrease the sampling resolution to 12 bits; this will decrease the per-color sample
    time to less than 7 ms. You can also change the #Mode property to only sample a subset of color channels. While
    the interrupt pin is optional, we recommend using it to avoid needlessly polling the(relatively slow-to-update)
    sensor. Pass in a Pin reference to the constructor, disable #AutoUpdateWhenPropertyRead, and attach a callback to
     \link Isl29125.interrupt_received interrupt_received\endlink. By default, your callback will be invoked whenever
     a full conversion has completed; otherwise, you can set #low_threshold, #high_threshold,
     and #interrupt_selection to fine-tune when interrupts are triggered.
    """
    def __init__(self, i2c: I2C, interrupt: DigitalIn = None, rate=100):
        """Construct a new ISL29125 object"""
        super().__init__()

        ## Fires whenever new color data has been received in response to an interrupt
        self.interrupt_received = EventHandler(self)

        self._registers = Isl29125Registers(SMBusRegisterManagerAdapter(SMBusDevice(0x44, i2c, rate)))

        # check device ID
        self._registers.deviceId.read()
        if self._registers.deviceId.value != 0x7D:
            Utility.error("No ISL29125 found.", True)

        # reset device
        self._registers.deviceReset.value = 0x46
        self._registers.deviceReset.write()

        self._registers.config1.mode = Modes.GreenRedBlue
        self._registers.config1.range = Ranges.Lux_10000
        self._registers.config1.write()

        # optional interrupt setup
        if interrupt is not None:
            interrupt.make_digital_in()
            interrupt.digital_value_changed += self._interrupt_handler

            self.interrupt_selection = InterruptSelections.Green

            # set the default thresholds to 0 so we get an interrupt on every ADC read by default
            self.low_threshold = 0
            self.high_threshold = 0

            # clear the (potentially pending) interrupt
            self._registers.status.read()

    def _interrupt_handler(self, sender, value):
        if not value:
            # interrupts occur on the falling edge
            self.update()
            self.interrupt_received(self.red, self.green, self.blue)

    def _value_to_lux(self, value):
        if self._registers.config1.range == Ranges.Lux_10000:
            value *= 10000
        else:
            value *= 375

        if self._registers.config1.sampleDepth == SampleDepths.Bits_12:
            value /= 4095
        else:
            value /= 65535

        return value

    @property
    def red(self):
        """Gets the amount of red, expressed in lux, measured by the sensor."""
        if self.auto_update_when_property_read:
            self._registers.redData.read()

        return self._value_to_lux(self._registers.redData.value)

    @property
    def green(self):
        """Gets the amount of green, expressed in lux, measured by the sensor."""
        if self.auto_update_when_property_read:
            self._registers.greenData.read()

        return self._value_to_lux(self._registers.greenData.value)

    @property
    def blue(self):
        """Gets the amount of blue, expressed in lux, measured by the sensor."""
        if self.auto_update_when_property_read:
            self._registers.blueData.read()

        return self._value_to_lux(self._registers.blueData.value)

    @property
    def mode(self):
        """Gets or sets the operating mode and channel selection for the sensor. Defaults to RGB."""
        return self._registers.config1.mode

    @mode.setter
    def mode(self, value):
        self._registers.config1.mode = value
        self._registers.config1.write()

    @property
    def sample_depth(self):
        """Gets or sets the sample depth (12-bit or 16-bit) to use for the sensor. Defaults to 16-bit."""
        return self._registers.config1.sampleDepth

    @sample_depth.setter
    def sample_depth(self, value):
        self._registers.config1.sampleDepth = value
        self._registers.config1.write()

    @property
    def range(self):
        """Gets or sets the lux range to use for the sensor. Defaults to 10,000 lux."""
        return self._registers.config1.range

    @range.setter
    def range(self, value):
        self._registers.config1.range = value
        self._registers.config1.write()

    @property
    def high_threshold(self):
        """Gets or sets the high threshold to use with the interrupt pin. Defaults to 0."""
        return self._registers.highThreshold.value

    @high_threshold.setter
    def high_threshold(self, value):
        self._registers.highThreshold.value = value
        self._registers.highThreshold.write()

    @property
    def low_threshold(self):
        """Gets or sets the low threshold to use with the interrupt pin. Defaults to 0."""
        return self._registers.lowThreshold.value

    @low_threshold.setter
    def low_threshold(self, value):
        self._registers.lowThreshold.value = value
        self._registers.lowThreshold.write()

    @property
    def interrupt_selection(self):
        """Gets or sets the channel to use for the interrupt threshold comparrison. Defaults to green."""
        return self._registers.config3.interruptSelection

    @interrupt_selection.setter
    def interrupt_selection(self, value):
        self._registers.config3.interruptSelection = value
        self._registers.config3.write()

    def update(self):
        """Fetches data from the sensor and updates the Red, Green, and Blue properties."""
        self._registers.readRange(self._registers.status, self._registers.blueData)
