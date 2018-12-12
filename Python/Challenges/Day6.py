from __future__ import annotations
import sys
import re as regex
from dataclasses import dataclass
from typing import List, Set, Pattern
from itertools import chain


# Point coordinate structure
@dataclass
class Point:
    """
    2D Point coordinate class
    """

    # Coordinates
    _x: int
    _y: int

    @property
    def x(self) -> int:
        """
        X value of this coordinate
        :return: The X value
        """

        return self._x

    @property
    def y(self) -> int:
        """
        Y value of this coordinate
        :return: The Y value
        """

        return self._y

    @staticmethod
    def distance(a: Point, b: Point) -> int:
        """
        Calculates the Manhattan distance between two points
        :param a: First point
        :param b: Second point
        :return:  The resulting, positive Manhattan distance between these points
        """

        return abs(a.x - b.x) + abs(a.y - b.y)


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # Get structure variables
    pattern: Pattern = regex.compile(r"(\d+), (\d+)")
    positions: List[Point] = []

    # File read stub
    with open(args[1], "r") as f:
        for line in f:
            point: Point = Point(*map(int, pattern.search(line).groups()))
            positions.append(point)

    # Set grid
    width: int = max(p.x for p in positions)
    height: int = max(p.y for p in positions)
    grid: List[List[int]] = [[0] * height for _ in range(width)]
    edges: Set[int] = set()
    counts: List[int] = [0] * len(positions)

    # Loop through every position in the grid
    for x in range(width):
        for y in range(height):
            # Set current position
            pos: Point = Point(x, y)
            closest: Point
            smallest: int = width + height

            # Loop through known points
            for i, point in enumerate(positions):
                # Calculate distance
                dist: int = Point.distance(pos, point)
                # New closest
                if dist < smallest:
                    grid[x][y] = i
                    smallest = dist
                    closest = point
                # Two points match, ignore
                elif dist == smallest:
                    grid[x][y] = -1

            # Points not ignored
            i: int = grid[x][y]
            if i != -1:
                # Point on an edge, and index to edge points
                if x == 0 or x == width - 1 or y == 0 or y == height - 1:
                    edges.add(i)
                    counts[i] = 0

                # If not an edge point, increment count
                elif i not in edges:
                    counts[i] += 1

    # Print maximum
    print("Part one max area:", max(counts))

    # Calculate distance sum
    for x in range(width):
        for y in range(height):
            pos: Point = Point(x, y)
            grid[x][y] = sum(Point.distance(pos, p) for p in positions)

    area: int = sum(1 for i in chain.from_iterable(grid) if i < 10000)
    print("Part two safe area:", area)


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
