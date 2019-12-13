import sys
from typing import List, Set, Optional
from utils import Vector


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    asteroids: Set[Vector] = set()
    # File read stub
    with open(args[1], "r") as f:
        for y, line in enumerate(f):
            for x, c in enumerate(line.strip()):
                if c == "#":
                    asteroids.add(Vector(x, y))

    station: Vector = Vector(0, 0)
    best: int = 0
    for asteroid in asteroids:
        angles: Set[float] = set()

        for target in asteroids:
            if asteroid == target:
                continue

            angles.add(Vector.direction(target - asteroid))

        visible = len(angles)
        if visible > best:
            best = visible
            station = asteroid

    print(best)

    asteroids.remove(station)
    last_destroyed: Vector = Vector(0, 0)
    last_direction: Vector = Vector(-1E-6, -1)
    for _ in range(200):
        if len(asteroids) == 0:
            break
        smallest: float = 360.0
        to_destroy: Optional[Vector] = None
        for target in sorted(asteroids, key=lambda a: Vector.distance(station, a)):
            angle = Vector.angle(last_direction, target - station)

            if smallest > angle > 0:
                smallest = angle
                to_destroy = target

        if to_destroy is None:
            to_destroy = next(iter(asteroids))
        asteroids.remove(to_destroy)
        last_destroyed = to_destroy
        last_direction = to_destroy - station

    print(last_destroyed.x * 100 + last_destroyed.y)


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
