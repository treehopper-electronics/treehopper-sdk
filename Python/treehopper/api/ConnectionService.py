import usb.core
import usb.util
from treehopper.api import TreehopperUsb


class ConnectionService:
    """Provides a service for discovering and instantiating attached Treehopper boards.

    Only one ConnectionService should be instantiated per session.
    """

    def __init__(self):
        self.boards = []  # type: list[TreehopperUsb]
        """The collection of attached boards"""

        for dev in usb.core.find(find_all=True, idVendor=0x10c4, idProduct=0x8a7e):
            self.boards.append(TreehopperUsb(dev))
