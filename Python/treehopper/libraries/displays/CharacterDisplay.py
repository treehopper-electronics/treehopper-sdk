from treehopper.libraries.Flushable import Flushable
from abc import ABC, abstractmethod

class CharacterDisplay(Flushable):
    def __init__(self, columns: int, rows: int):
        self.cols = columns  # type: int
        self.rows = rows     # type: rows
        self.lines = []      # type list[string]
        self._cursor_left = 0
        self._cursor_top = 0

        for i in range(rows):
            self.lines.append("")

    def flush(self, force=False):
        pass

    def cursor_left(self):
        return self._cursor_left

    def cursor_left(self, value):
        if self._cursor_left == value:
            return

        self._cursor_left = value
        self._update_cursor()

    def cursor_top(self):
        return self._cursor_top

    def cursor_top(self, value):
        if self._cursor_top == value:
            return

        self._cursor_top = value
        self._update_cursor()

    def set_cursor(self, left: int, top: int):
        self._cursor_left = left
        self._cursor_top = top
        self._update_cursor()

    def clear(self):
        self._cursor_left = 0
        self._cursor_top = 0
        self._clear()

    @abstractmethod
    def _clear(self):
        pass

    @abstractmethod
    def _update_cursor(self):
        pass
