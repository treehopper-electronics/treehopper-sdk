from treehopper.utils import EventHandler


class AdcPin:
    """
    Represents an ADC pin

    :ivar analog_value_changed: Fires whenever the analog value change exceeds analog_value_threshold
    """
    def __init__(self, bit_depth, max_voltage):
        self._bit_depth = bit_depth
        self._adc_value = 0
        self._old_adc_value = 0
        self._old_analog_value = 0
        self._old_analog_voltage = 0
        self.max_voltage = max_voltage                    # type: float
        
        #: Fires whenever the voltage change exceeds analog_voltage_threshold
        self.analog_voltage_changed = EventHandler(self)  # type: EventHandler
        

        self.analog_value_changed = EventHandler(self)    # type: EventHandler
        """Fires whenever the analog value change exceeds analog_value_threshold"""

        self.adc_value_changed = EventHandler(self)       # type: EventHandler
        """Fires whenever the ADC value change exceeds adc_value_threshold"""

        self.adc_value_threshold = 4                      # type: int
        """The ADC value threshold used for the adc_value_changed event"""

        self.analog_value_threshold = 0.05                # type: float
        """Analog value threshold used for the analog_value_changed event"""

        self.analog_voltage_threshold = 0.1               # type: float
        """Analog voltage threshold used for the analog_voltage_changed event"""

    @property
    def adc_value(self) -> int:
        """Immediately returns the last ADC value (0-4095) captured from the pin

        :type: int        
        """
        return self._adc_value

    @property
    def analog_voltage(self) -> float:
        """Immediately returns the last analog voltage (0-3.6V) captured from the pin

        :type: float
        """
        return self.analog_value * self.max_voltage

    @property
    def analog_value(self) -> float:
        """Immediately returns the uniform last analog value (0-1) captured from the pin

        :type: float
        """
        return self._adc_value / ((1 << self._bit_depth) - 1)

    def _update_value(self, adc_value):
        # self._old_analog_voltage = self.analog_voltage
        # self._old_analog_value = self.analog_value

        self._adc_value = adc_value

        if abs(self.analog_voltage - self._old_analog_voltage) > self.analog_voltage_threshold:
            self._old_analog_voltage = self.analog_voltage
            self.analog_voltage_changed(self.analog_voltage)

        if abs(self.analog_value - self._old_analog_value) > self.analog_value_threshold:
            self._old_analog_value = self.analog_value
            self.analog_value_changed(self.analog_value)

        if abs(adc_value - self._old_adc_value) > self.adc_value_threshold:
            self._old_adc_value = self.adc_value
            self.adc_value_changed(adc_value)
