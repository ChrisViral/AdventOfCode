import sys
import operator
from typing import List, Set, Tuple
from dataclasses import dataclass
from copy import deepcopy
import math
import re as regex

# State type hint
State = Tuple[int, int]


@dataclass
class Moon:
    """
    Represents a Moon, with its position and velocity
    """
    position: List[int]
    velocity: List[int]

    def to_state(self, axis: int) -> State:
        """
        Returns the state tuple for a given axis
        :param axis: Axis (x, y, or z) to get the state for
        :return: A tuple representing the position and velocity on the specified axis
        """
        return self.position[axis], self.velocity[axis]


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    start: List[Moon] = []
    # File read stub
    with open(args[1], "r") as f:
        for line in f:
            # Parse input
            x, y, z = tuple(map(int, regex.search(r"<x=(-?\d+), y=(-?\d+), z=(-?\d+)>", line.strip()).groups()))
            start.append(Moon([x, y, z], [0, 0, 0]))

    # Simulate for 1000 cycles
    moons: List[Moon] = deepcopy(start)
    for _ in range(1000):
        for moon in moons:
            for target in moons:
                if moon == target:
                    continue
                for i in range(3):
                    if moon.position[i] != target.position[i]:
                        moon.velocity[i] += 1 if moon.position[i] < target.position[i] else -1

        for moon in moons:
            moon.position = list(map(operator.add, moon.position, moon.velocity))

    # Get the energy after
    energy: int = 0
    for moon in moons:
        energy += sum(map(abs, moon.position)) * sum(map(abs, moon.velocity))
    print(energy)

    # Simulate until a loop is achieved on each axis
    x_time = test_axis(start, 0)
    y_time = test_axis(start, 1)
    z_time = test_axis(start, 2)
    # Get the LCM of these cycles
    print(lcm([x_time, y_time, z_time]))


def test_axis(moons: List[Moon], axis: int) -> int:
    """
    Finds a cycle on a specified axis for the given moons
    :param moons: List of moons to simulate
    :param axis: Axis (x, y, or z) to simulate on
    :return: The time before a loop happens in the states for this axis
    """
    # States set
    states: Set[Tuple[State, State, State, State]] = set()
    steps: int = 0
    while True:
        # Simulate
        for moon in moons:
            for target in moons:
                if moon == target:
                    continue
                if moon.position[axis] != target.position[axis]:
                    moon.velocity[axis] += 1 if moon.position[axis] < target.position[axis] else -1
        for moon in moons:
            moon.position[axis] += moon.velocity[axis]

        # Check if a similar state has been achieved
        state: Tuple[State, State, State, State]
        state = moons[0].to_state(axis), moons[1].to_state(axis), moons[2].to_state(axis), moons[3].to_state(axis)
        # If not, keep simulating
        if state not in states:
            states.add(state)
            steps += 1
        # Else, return the amount of steps taken to get her
        else:
            return steps


def lcm(nums: List[int]) -> int:
    """
    Computes the LCM of the specified numbers
    :param nums: Numbers to computer the LCM for
    :return: LCM of the numbers
    """
    n = nums[0]
    for i in nums[1:]:
        n = n * i // math.gcd(n, i)
    return n


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
