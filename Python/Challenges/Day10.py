from __future__ import annotations
import sys
import re as regex
from typing import List, Tuple, Pattern
from Utils import Vector


class Particle:
    """
    An object representing a particle, with a speed and position
    """

    def __init__(self, px: int, py: int, vx: int, vy: int) -> None:
        """
        Creates a new Particle from a given position and velocity
        :param px: X position
        :param py: Y Position
        :param vx: X Velocity
        :param vy: Y Velocity
        """

        self.position: Vector = Vector(px, py)
        self.velocity: Vector = Vector(vx, vy)

    def update(self, seconds: int = 1) -> None:
        """
        Updates the particles position to the given amount of seconds
        :param seconds: Time in seconds to move the particle's position by
        """

        self.position += (self.velocity * seconds)


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # Get patten and setup particle list
    pattern: Pattern = regex.compile(r"position=<([-| ]\d+), ([-| ]\d+)> velocity=<([-| ]\d+), ([-| ]\d+)>")
    particles: List[Particle] = []

    # File read stub
    with open(args[1], "r") as f:
        for line in f:
            particle: Particle = Particle(*map(int, pattern.search(line).groups()))
            particles.append(particle)

    # Loop forward time until the particles are close enough to each other
    seconds: int = 0
    diffx, diffy = get_diff(particles)
    while diffx > 100 or diffy > 100:
        # Determine the time jump according to how far apart the particles are
        jump: int = min(diffx, diffy) // 25
        seconds += jump
        # Update all particles
        for p in particles:
            p.update(jump)

        # Get new diff
        diffx, diffy = get_diff(particles)

    # Print the next ten seconds
    for _ in range(10):
        # Get draw bounds
        max_x, min_x, max_y, min_y = get_extremums(particles)
        print(f"After {seconds} seconds:")
        # Loop through bounds
        data: str = ""
        for y in range(min_y, max_y + 1):
            for x in range(min_x, max_x + 1):
                # u25A0 is the unicode black square character
                data += u"\u25A0" if any(p.position.x == x and p.position.y == y for p in particles) else " "
            data += "\n"
        # Print resulting data
        print(data)

        # Update by another second
        seconds += 1
        for p in particles:
            p.update()


def get_diff(particles: List[Particle]) -> Tuple[int, int]:
    """
    Gets the difference between the highest and lowest points in both dimensions for all provided particles
    :param particles: Particles to get the difference for
    :return: A tuple containing the x diff, then the y diff
    """

    # Get extremums and return diff
    max_x, min_x, max_y, min_y = get_extremums(particles)
    return max_x - min_x, max_y - min_y


def get_extremums(particles: List[Particle]) -> Tuple[int, int, int, int]:
    """
    Gets the maximums and minimums in both dimensions for all the supplied particles
    :param particles: Particles to get the extremums for
    :return: A tuple containing in sequence, the max x value, min x value, max y value, and min y value
    """

    max_x: int = max(p.position.x for p in particles)
    min_x: int = min(p.position.x for p in particles)
    max_y: int = max(p.position.y for p in particles)
    min_y: int = min(p.position.y for p in particles)
    return max_x, min_x, max_y, min_y


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
