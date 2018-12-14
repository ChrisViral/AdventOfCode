from __future__ import annotations
import sys
import re as regex
from typing import List, Dict, Set, Optional, Pattern
from enum import Enum
from Utils.Vector import Vector


class Direction(Vector, Enum):
    """
    Direction enum
    """

    # Enum members
    UP = 0, -1
    DOWN = 0, 1
    RIGHT = 1, 0
    LEFT = -1, 0
    NONE = 0, 0

    def turn_left(self) -> Direction:
        """
        Turns this direction left relative to it's current heading
        :return: The direction turned to the left
        """

        return Direction((self._y, -self._x))

    def turn_right(self) -> Direction:
        """
        Turns this direction right relative to it's current heading
        :return: The direction turned to the right
        """

        return Direction((-self._y, self._x))


class Rail(str, Enum):
    """
    Rail enum representation
    """

    # Enum members
    RAIL_VERTICAL = '|'
    RAIL_HORIZONTAL = '-'
    INTERSECTION = '+'
    CORNER_RIGHT = '/'
    CORNER_LEFT = '\\'
    NONE = ' '

    def turn(self, heading: Direction) -> Direction:
        """
        Turns the direction according to this rail corner
        :param heading: Current heading direction
        :return: Resulting heading direction. If the rail is not a corner rail, the heading stays the same.
        """

        # Turn correctly if on corner tile
        if self == Rail.CORNER_RIGHT:
            return Direction((-heading.y, -heading.x))
        elif self == Rail.CORNER_LEFT:
            return Direction((heading.y, heading.x))
        return heading


class Grid:
    """
    2D rail grid system
    """

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

    def __init__(self, data: List[str], width: int) -> None:
        """
        Creates a new rail Grid
        :param data: Data to create the grid from
        :param width: Maximum width of the grid
        """

        # Setting up the data
        self._width: int = width
        self._height: int = len(data)
        self._grid: List[List[Rail]] = []

        # Creating the grid
        for line in data:
            row: List[Rail] = [Rail.NONE] * self._width
            for x in range(len(line)):
                row[x] = Rail(line[x])
            self._grid.append(row)

    def __getitem__(self, pos: Vector) -> Rail:
        """
        Gets the rail object at the given Vector position in the Grid
        :param pos: Vector position in the grid
        :return: The rail value at the given position
        """

        return self._grid[pos.y][pos.x]


class Cart:
    """
    Cart evolving through the rail system
    """

    # String/Direction parsing dictionary
    _parse: Dict[str, Direction] = {'^': Direction.UP, 'v': Direction.DOWN, '>': Direction.RIGHT, '<': Direction.LEFT}

    @classmethod
    def create_cart(cls, x: int, y: int, heading: str) -> Optional[Cart]:
        """
        Factory method which creates a new cart
        :param x: X position in the grid
        :param y: Y position in the grid
        :param heading: String representation of the direction the cart is facing
        :return: The created cart, or None if the information was invalid
        """

        # Check if the heading is valid
        if heading in cls._parse:
            return Cart(Vector(x, y), cls._parse[heading])
        # If not, return nothing
        return None

    def __init__(self, position: Vector, direction: Direction) -> None:
        """
        Creates a new Cart
        :param position: Position of the cart in the grid
        :param direction: Direction this cart is facing
        """

        # Setup variables
        self._grid: Optional[Grid] = None
        self.pos: Vector = position
        self.direction: Direction = direction
        self._choice: int = 0

    def set_grid(self, grid: Grid) -> None:
        """
        Sets the grid this cart evolves in
        :param grid: Grid to set
        """

        self._grid = grid

    def update(self) -> None:
        """
        Updates the position of the cart by moving it in the grid
        """

        # Get current underneath Rail piece
        current: Rail = self._grid[self.pos]

        # If on corner tile, turn
        if current == Rail.CORNER_RIGHT or current == Rail.CORNER_LEFT:
            self.direction = current.turn(self.direction)
        # Handle intersections as well
        elif current == Rail.INTERSECTION:
            if self._choice == 0:
                self.direction = self.direction.turn_left()
            elif self._choice == 2:
                self.direction = self.direction.turn_right()
            self._choice = (self._choice + 1) % 3

        self.pos += self.direction


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    data: List[str] = []
    carts: List[Cart] = []
    pattern: Pattern = regex.compile(r"[\^v<>]")
    vertical: Pattern = regex.compile(r"[\^v]")
    horizontal: Pattern = regex.compile("[<>]")
    width: int = 0

    # File read stub
    with open(args[1], "r") as f:
        j: int
        line: str
        for j, line in enumerate(f):

            # Ignore empty lines
            line = line.rstrip()
            length: int = len(line)
            if length == 0:
                continue

            # Get max width
            width = max(width, length)

            # Get all carts
            for match in pattern.finditer(line):
                cart: Cart = Cart.create_cart(match.start(), j, match.group())
                if cart:
                    carts.append(cart)

            # Replace carts by rails
            line = regex.sub(vertical, '|', line)
            line = regex.sub(horizontal, '-', line)
            data.append(line)

    # Create and set grid
    grid: Grid = Grid(data, width)
    for cart in carts:
        cart.set_grid(grid)

    # As long as there is more than one car going around
    crashed: Set[Cart] = set()
    while len(carts) > 1:
        # Update carts in order, starting at the top left through the bottom right
        for cart in sorted(carts, key=lambda x: (x.pos.y, x.pos.y)):
            cart.update()
            for c in carts:
                # Check for collisions
                if c is not cart and c not in crashed and c.pos == cart.pos:
                    crashed.add(cart)
                    crashed.add(c)
                    print("Crashed happened at location", cart.pos)
                    break

        # Removed crashed carts
        while crashed:
            carts.remove(crashed.pop())

    # Print final cart
    print("Final cart location:", carts[0].pos)


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
