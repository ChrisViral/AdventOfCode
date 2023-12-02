from __future__ import annotations
import sys
import re as regex
from typing import List, Pattern, Dict


class Node:
    """
    Decision tree node
    """

    def __init__(self, depth: int, name: str = "") -> None:
        """
        Creates a new decision tree node
        :param depth: Depth of the tree from this node
        :param name: Name from the node, T and F will be appended dynamically
        """

        self._name = name
        self._value = False
        # Generate nodes recursively
        if depth > 0:
            self._true: Node = Node(depth - 1, name + "T")
            self._false: Node = Node(depth - 1, name + "F")

    def _get_node(self, path: List[bool]) -> Node:
        """
        Gets the node from the given decisions from this node
        :param path: Decision path through the tree from this node
        :return: The node at the given decision path
        """

        current: Node = self
        # Loop to the depth of the decision path
        for i in range(len(path)):
            # Get correct node
            if path[i]:
                current = current._true
            else:
                current = current._false

        return current

    def set_value(self, path: List[bool], value: bool) -> None:
        """
        Sets the value of the decision node at this given path
        :param path: Path of the decision node
        :param value: Value to set
        """

        self._get_node(path)._value = value

    def get_value(self, path: List[bool]) -> bool:
        """
        Gets the value at the given decision path in the tree
        :param path: Decision path to the node
        :return: The value of the node
        """

        return self._get_node(path)._value

    def __str__(self) -> str:
        """
        String representation of the node
        :return: The name given to the node, plus its decision path
        """

        return self._name


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # Create the decision tree and compile the pattern
    tree: Node = Node(5)
    pattern: Pattern = regex.compile("([#.]{5}) => ([#.])")

    # File read
    with open(args[1], "r") as f:
        # Create the base state with some padding, you might want to increase the offset if your state goes left
        offset: int = 5
        pots: List[bool] = as_bool(("." * offset) + regex.search("[#.]+", f.readline()).group() + "....")

        # Loop through non empty lines
        for line in filter(lambda l: len(l.strip()) > 0, f):
            decision: List[bool]
            value: List[bool]
            decision, value = map(as_bool, pattern.search(line).groups())
            # If a true path, set the decision tree as such
            if value[0]:
                tree.set_value(decision, True)

    # Setup some stuff
    gen20: int
    generations: int = 200
    prev: int = sum_pots(pots, offset)
    diffs: Dict[int, int] = {}

    # Loop through a fixed amount of generations
    for gen in range(1, generations + 1):
        temp: List[bool] = list(pots)
        for i in range(2, len(temp) - 2):
            value: bool = tree.get_value(temp[i - 2:i + 3])
            pots[i] = value

        # Add to the right side if needed
        add: int = sum(1 for i in range(-4, -2) if pots[i])
        for _ in range(add):
            pots.append(False)

        # Get diff
        curr: int = sum_pots(pots, offset)
        diff: int = curr - prev
        # print(f"Generation: {gen}, Current: {curr}, Diff: {curr - prev}")
        prev = curr

        # Get generation 20 score
        if gen == 20:
            gen20 = curr

        # Setup diff frequency
        if diff not in diffs:
            diffs[diff] = 1
        else:
            diffs[diff] += 1

    # print(as_str(pots))
    # Print generation 20 score
    print("Part one score:", gen20)

    # Assume that the diff that appears the most often is gonna be constant
    diff = max(diffs, key=lambda d: diffs[d])
    print("Part two score:", prev + ((50000000000 - generations) * diff))


def as_bool(data: str) -> List[bool]:
    """
    Parses a string into a list of bools, where '#' are true, and everything else is false
    :param data: String to parse
    :return: The generated list of bools
    """

    return list(map(lambda c: c == "#", data))


def as_str(data: List[bool]) -> str:
    """
    Parses a list of bools as a string, where True becomes '#', and False '.'
    :param data: List of bools to parse
    :return: The resulting string
    """

    return "".join(map(lambda b: "#" if b else ".", data))


def sum_pots(pots: List[bool], offset: int) -> int:
    """
    Sums the pots by their index
    :param pots: Pots and their values (alive/dead)
    :param offset: The offset from the start to the zero index
    :return: The sum of all indices with alive plants
    """

    return sum(i - offset for i in range(len(pots)) if pots[i])


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
