from treehopper.utils import EventHandler


class AdcPin:
    def __init__(self, bit_depth, max_voltage):
        self._bit_depth = bit_depth
        self._max_voltage = max_voltage
        self._adc_value = 0
        self.analog_voltage_changed = EventHandler(self)
        self.analog_value_changed = EventHandler(self)
        self.adc_value_changed = EventHandler(self)
        pass

    @property
    def adc_value(self):
        return self._adc_value

    @property
    def analog_voltage(self):
        return self.analog_value * self._max_voltage

    @property
    def analog_value(self):
        return self._adc_value / ((1 << self._bit_depth) - 1)

    @property
    def analog_voltage_changed_threshold(self):
        pass

    @property
    def adc_value_changed_threshold(self):
        pass

    @property
    def analog_value_changed_threshold(self):
        pass

    def _update_value(self, adc_value):
        self._adc_value = adc_value

