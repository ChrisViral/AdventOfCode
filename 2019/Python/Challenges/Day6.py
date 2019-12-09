from __future__ import annotations
import sys
from typing import List, Optional, Iterable, Pattern, Iterator
from utils import ParameterDict
import re as regex


class Planet(Iterable["Planet"]):
    """
    Planet object with orbiting children and a parent
    """

    # region Properties
    @property
    def name(self) -> str:
        return self._name

    @property
    def parent(self) -> Planet:
        return self._parent

    @parent.setter
    def parent(self, value: Planet) -> None:
        self._parent = value

    @property
    def children(self) -> List[Planet]:
        return self._children
    # endregion

    # region Constructor
    def __init__(self, name: str) -> None:
        """
        Creates a new planet with the specified name
        :param name: Name of the new planet
        """
        self._name: str = name
        self._children: List[Planet] = []
        self._parent: Optional[Planet] = None
        self._visited: bool = False
    # endregion

    # region Methods
    def add_planet(self, planet: Planet) -> None:
        """
        Adds the given planet to this planet's children list
        :param planet: The planet to add
        """
        planet.parent = self
        self._children.append(planet)

    def count_orbits(self, current: int = 0) -> int:
        """
        Counts the amount of direct and indirect orbits of this planet and all it's children
        :param current: The current quantity of orbits at this point
        :return: The total quantity of orbits from this planet
        """
        return current + sum(map(lambda c: c.count_orbits(current + 1), self._children))

    def shortest_transfer(self, target: str, current: int = 0) -> int:
        """
        Calculates the shortest orbital transfer from this planet's parent to the target's parent
        :param target: Planet to reach
        :param current: Current length of the transfer to reach this planet
        :return: The total length of the orbital transfer
        """
        self._visited = True
        if self._name == target:
            return current - 2
        for planet in [self._parent] + self._children:
            if planet is None or planet._visited:
                continue
            transfer: int = planet.shortest_transfer(target, current + 1)
            if transfer != -1:
                return transfer

        return -1

    def __iter__(self) -> Iterator[Planet]:
        return iter(self._children)

    def __repr__(self) -> str:
        return self._name
    # endregion


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # Planets dictionary and regex parsing pattern
    planets: ParameterDict[str, Planet] = ParameterDict(lambda k: Planet(k))
    orbit: Pattern = regex.compile(r"(\w+)\)(\w+)")

    # File read stub
    with open(args[1], "r") as f:
        low: str
        high: str
        for line in f:
            # For each line, create the planets and orbits
            low, high = orbit.search(line).groups()
            planets[low].add_planet(planets[high])

    # Print the necessary information
    print(planets["COM"].count_orbits())
    print(planets["YOU"].shortest_transfer("SAN"))


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
