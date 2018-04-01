"""Base Treehopper API for Python.

This module provides digital and analog I/O, hardware and software PWM, I2C, SPI, and UART support.
"""
## @namespace treehopper.api

from treehopper.api.interfaces import *
from treehopper.api.pin import Pin, PinMode, ReferenceLevel
from treehopper.api.device_commands import DeviceCommands
from treehopper.api.pwm import HardwarePwm, HardwarePwmFrequency
from treehopper.api.treehopper_usb import TreehopperUsb
from treehopper.api.find_boards import find_boards
from treehopper.api.i2c import I2C
from treehopper.api.spi import HardwareSpi
from treehopper.api.i2c import HardwareI2C
from treehopper.api.uart import HardwareUart

__all__ = ['find_boards', 'TreehopperUsb',
           'Pin', 'PinMode', 'ReferenceLevel',
           'HardwareSpi', 'Spi', 'ChipSelectMode', 'SpiMode', 'SpiBurstMode',
           'HardwareI2C', 'I2C',
           'HardwareUart','Uart', 'OneWire',
           'HardwarePwm', 'HardwarePwmFrequency',
           ]
