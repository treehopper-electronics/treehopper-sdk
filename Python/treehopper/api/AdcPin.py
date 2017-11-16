from treehopper.utils import EventHandler


class AdcPin:
    def __init__(self, bit_depth, max_voltage):
        self._bit_depth = bit_depth
        self._adc_value = 0
        self._old_adc_value = 0
        self._old_analog_value = 0
        self._old_analog_voltage = 0
        self.max_voltage = max_voltage                    # type: float
        self.analog_voltage_changed = EventHandler(self)  # type: EventHandler
        self.analog_value_changed = EventHandler(self)    # type: EventHandler
        self.adc_value_changed = EventHandler(self)       # type: EventHandler
        self.adc_value_threshold = 4                      # type: int
        self.analog_value_threshold = 0.05                # type: float
        self.analog_voltage_threshold = 0.1               # type: float

    @property
    def adc_value(self) -> int:
        return self._adc_value

    @property
    def analog_voltage(self) -> float:
        return self.analog_value * self.max_voltage

    @property
    def analog_value(self) -> float:
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
