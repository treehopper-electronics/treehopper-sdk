from typing import List

from treehopper.api import TreehopperUsb
import usb.core
import usb.util


def find_boards() -> List[TreehopperUsb]:
    boards = []
    for dev in usb.core.find(find_all=True, idVendor=0x10c4, idProduct=0x8a7e):
        boards.append(TreehopperUsb(dev))

    return boards
