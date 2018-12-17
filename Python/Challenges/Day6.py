from __future__ import annotations
import sys
import re as regex
from typing import List, Set, Pattern
from Utils.Vector import Vector
from Utils.Grid import Grid


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # Get structure variables
    pattern: Pattern = regex.compile(r"(\d+), (\d+)")
    positions: List[Vector] = []

    # File read stub
    with open(args[1], "r") as f:
        for line in f:
            point: Vector = Vector(*map(int, pattern.search(line).groups()))
            positions.append(point)

    # Set grid
    width: int = max(p.x for p in positions)
    height: int = max(p.y for p in positions)
    grid: Grid[int] = Grid(width, height, 0)
    edges: Set[int] = set()
    counts: List[int] = [0] * len(positions)

    # Loop through every position in the grid
    for x in range(width):
        for y in range(height):
            # Set current position
            pos: Vector = Vector(x, y)
            closest: Vector
            smallest: int = width + height

            # Loop through known points
            for i, point in enumerate(positions):
                # Calculate distance
                dist: int = Vector.distance_rectilinear(pos, point)
                # New closest
                if dist < smallest:
                    grid[pos] = i
                    smallest = dist
                    closest = point
                # Two points match, ignore
                elif dist == smallest:
                    grid[pos] = -1

            # Points not ignored
            i: int = grid[pos]
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
            pos: Vector = Vector(x, y)
            grid[pos] = sum(Vector.distance_rectilinear(pos, p) for p in positions)

    area: int = sum(1 for i in grid if i < 10000)
    print("Part two safe area:", area)


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
