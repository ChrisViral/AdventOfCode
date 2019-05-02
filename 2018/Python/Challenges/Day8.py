import sys
from collections import deque
from typing import List, Deque


class Node:
    """
    A tree node data structure that holds references to children nodes as well as metadata
    """

    def __init__(self, data: Deque[int]) -> None:
        """
        Creates and populates a new Node with the given data
        :param data: Data list for this node's creation. The data will be consumed by creating the node.
        """

        # Create instance fields
        self.children: List[Node] = []
        self.metadata: List[int] = []
        self.count = data.popleft()
        meta: int = data.popleft()

        for _ in range(self.count):
            self.children.append(Node(data))

        for _ in range(meta):
            self.metadata.append(data.popleft())

    def get_total(self) -> int:
        """
        Gets the total of the metadata for this node and it's children
        :return: Total of the metadata
        """

        # Sum this metadata and the children's
        return sum(self.metadata) + sum(c.get_total() for c in self.children)

    def get_value(self) -> int:
        """
        Gets the value for this node
        If this node has no children, the value is the sum of it's metadata
        Otherwise, the metadata is used as an index to retrieve children nodes if possible, and the
        value becomes the sum of the value of these children
        :return: Value of this node
        """

        if self.count == 0:
            return sum(self.metadata)
        else:
            return sum(self.children[m - 1].get_value() for m in self.metadata if m <= self.count)


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # Read data from file
    with open(args[1], "r") as f:
        data: Deque[int] = deque(map(int, f.readline().strip().split()))

    # Create root node and print results
    root: Node = Node(data)
    print("Part one total:", root.get_total())
    print("Part two value:", root.get_value())


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
