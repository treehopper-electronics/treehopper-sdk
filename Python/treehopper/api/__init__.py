"""Base Treehopper API for Python.

This module provides digital and analog I/O, hardware and software PWM, I2C, SPI, and UART support.
"""
## @namespace treehopper.api

from treehopper.api.Pin import Pin, PinMode, AdcPin, DigitalIn, DigitalOut, ReferenceLevel
from treehopper.api.DeviceCommands import DeviceCommands
from treehopper.api.HardwarePwm import HardwarePwm
from treehopper.api.Pwm import Pwm
from treehopper.api.TreehopperUsb import TreehopperUsb
from treehopper.api.find_boards import find_boards
from treehopper.api.I2c import I2c
from treehopper.api.HardwareSpi import HardwareSpi
from treehopper.api.HardwareI2c import HardwareI2c
from treehopper.api.HardwareUart import HardwareUart

__all__ = ['find_boards', 'TreehopperUsb', 'DeviceCommands', 'HardwarePwm', 'Pin', 'PinMode', 'AdcPin', 'DigitalIn',
           'DigitalOut', 'ReferenceLevel', 'I2c', 'HardwareSpi', 'Spi', 'HardwareUart', 'HardwareI2c']
