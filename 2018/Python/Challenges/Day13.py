from __future__ import annotations
import sys
import re as regex
from typing import List, Dict, Set, Optional, Pattern
from enum import Enum
from itertools import chain
from Utils import Vector, Direction, Grid


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
        self._grid: Optional[Grid[Rail]] = None
        self.pos: Vector = position
        self.direction: Direction = direction
        self._choice: int = 0

    def set_grid(self, grid: Grid[Rail]) -> None:
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
        i: int
        line: str
        for i, line in enumerate(f):

            # Ignore blank lines
            if len(line.strip()) == 0:
                continue

            # Remove line terminator
            line = line.rstrip("\n")

            # Get all carts
            for match in pattern.finditer(line):
                cart: Cart = Cart.create_cart(match.start(), i, match.group())
                if cart:
                    carts.append(cart)

            # Replace carts by rails
            line = regex.sub(vertical, '|', line)
            line = regex.sub(horizontal, '-', line)
            data.append(line)

    # Create and set grid
    grid: Grid[Rail] = Grid.populate_new(len(data[0]), len(data), chain.from_iterable(data), Rail)
    for cart in carts:
        cart.set_grid(grid)

    # As long as there is more than one car going around
    crashed: Set[Cart] = set()
    while len(carts) > 1:
        # Update carts in order, starting at the top left through the bottom right
        for cart in sorted(carts, key=lambda x: x.pos):
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
