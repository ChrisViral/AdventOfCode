from __future__ import annotations
from Utils.Vector import Vector
from typing import List, Tuple, Callable, TypeVar, Collection, Iterable, Iterator, Union, Any
import undefined

# Type aliases
T = TypeVar("T")
Position = Union[Vector, Tuple[int, int]]


class Grid(Collection[T]):
    """
    Generic 2D grid/array
    """

    # region Properties
    @property
    def width(self) -> int:
        """
        Grid's width
        :return: Grid's width
        """

        return self._width

    @property
    def height(self) -> int:
        """
        Grid's height
        :return: Grid's height
        """

        return self._height
    # endregion

    # region Static methods
    @staticmethod
    def populate_new(width: int, height: int, data: Iterable, converter: Callable[[Any], T]) -> Grid[T]:
        """
        Creates a new grid with the given size, then populates it with the given data
        :param width: Width of the grid
        :param height: Height of the grid
        :param data: Data to populate the grid with
        :param converter: Data to Grid element conversion function
        :return: The created Grid
        """

        grid: Grid[T] = Grid(width, height)
        grid.populate_grid(data, converter)
        return grid
    # endregion

    # region Methods
    def __init__(self, width: int, height: int, default: T = undefined) -> None:
        """
        Creates a new rail Grid
        :param width: Width of the grid
        :param height: Height of the grid
        :param default: Default start value of the grid. If none is specified, undefined is used
        """

        # Setup grid
        self._width: int = width
        self._height: int = height
        self._grid: List[List[T]] = [[default] * width for _ in range(height)]

    def populate_grid(self, data: Iterable, converter: Callable[[Any], T]) -> None:
        """
        Populates the whole grid with the data from the given iterable
        :param data: Data to populate the grid with
        :param converter: Data to Grid object conversion function
        """

        # Get data
        i: Iterator = iter(data)
        for y in range(self._height):
            for x in range(self._width):
                self._grid[y][x] = converter(next(i))

    def __getitem__(self, pos: Position) -> T:
        """
        Gets the object at the given position in the Grid
        :param pos: Vector or tuple indicating the position in the grid
        :return: The value at the given position
        """

        if isinstance(pos, Vector):
            return self._grid[pos.y][pos.x]
        else:
            return self._grid[pos[0]][pos[1]]

    def __setitem__(self, pos: Position, value: T) -> None:
        """
        Sets the object at the given position in the grid
        :param pos: Vector or tuple indicating the position in the grid
        :param value: Value to set
        """

        if isinstance(pos, Vector):
            self._grid[pos.y][pos.x] = value
        else:
            self._grid[pos[0]][pos[1]] = value

    def __contains__(self, item: T) -> bool:
        """
        Checks if a given item exists in the grid
        :param item: Object to check for
        :return: True if the object exists in the grid, False otherwise
        """

        return any(item in row for row in self._grid)

    def __iter__(self) -> Iterator[T]:
        """
        Loops through all the elements in the grid, starting at the top left corner,
        and going row by row to the last element at the bottom right
        :return: An iterator over the grid
        """

        for y in range(self._height):
            row: List[T] = self._grid[y]
            for x in range(self._width):
                yield row[x]

    def __len__(self) -> int:
        """
        Length of this 2D grid
        :return: Length of the grid
        """

        return self._width * self._height

    def __str__(self) -> str:
        """
        Nicely formatted string representation of the grid
        :return: A string representation of the grid
        """

        return "\n".join(" ".join(map(str, row)) for row in self._grid)
    # endregion
