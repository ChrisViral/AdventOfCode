from __future__ import annotations
from dataclasses import dataclass
from typing import Union
from enum import Enum
from functools import total_ordering
import math

# Type alias
Number = Union[int, float]


@dataclass
@total_ordering
class Vector:
    """
    An object representing a 2D vector
    """

    # region Fields
    _x: Number = 0
    _y: Number = 0
    # endregion

    # region Properties
    @property
    def x(self) -> Number:
        """
        The X position of the vector
        :return: X component
        """

        return self._x

    @property
    def y(self) -> Number:
        """
        The Y position of the vector
        :return: Y component
        """

        return self._y

    @property
    def normalized(self) -> Vector:
        """
        Returns this vector normalized (of length 1)
        :return: The normalized vector
        """

        return self / len(self)
    # endregion

    # region Static methods
    @staticmethod
    def dot(a: Vector, b: Vector) -> Number:
        """
        Returns the dot product between two vectors
        :param a: First vector
        :param b: Second vector
        :return: The dot product of both vectors
        """

        return (a._x * b._x) + (a._y * b._y)

    @staticmethod
    def distance(a: Vector, b: Vector) -> float:
        """
        Calculates the Euclidean distance between a and b
        :param a: First vector
        :param b: Second vector
        :return: The Euclidean distance between a and b
        """

        return math.sqrt(((a._x - b._x) ** 2) + ((a._y - b._y) ** 2))

    @staticmethod
    def distance_rectilinear(a: Vector, b: Vector) -> Number:
        """
        Calculates the rectilinear (Manhattan) distance between a and b
        :param a: First vector
        :param b: Second vector
        :return: The rectilinear (Manhattan) distance between a and b
        """

        return abs(a._x - b._x) + abs(a._y - b._y)
    # endregion

    # region Methods
    def __neg__(self) -> Vector:
        """
        Negates this vector
        :return: Returns the same vector but with both it's components negated
        """

        return Vector(-self._x, -self._y)

    def __add__(self, other: Vector) -> Vector:
        """
        Adds two vectors together
        :param other: The other vector to add
        :return: The resulting vector
        """

        return Vector(self._x + other._x, self._y + other._y)

    def __sub__(self, other: Vector) -> Vector:
        """
        Subtracts two vectors from each other
        :param other: The other vector to subtract
        :return: The resulting vector
        """

        return Vector(self._x - other._x, self._y - other._y)

    def __mul__(self, scalar: Number) -> Vector:
        """
        Multiplies a vector by a scalar
        :param scalar: Scalar to multiply the vector by
        :return: Multiplied/scaled vector
        """

        return Vector(self._x * scalar, self._y * scalar)

    def __truediv__(self, scalar: Number) -> Vector:
        """
        Divides a vector by a scalar
        :param scalar: Scalar to divide the vector by
        :return: Divided/scaled vector
        """

        return Vector(self._x / scalar, self._y / scalar)

    def __floordiv__(self, scalar: int) -> Vector:
        """
        Divides a vector by a scalar
        :param scalar: Scalar to divide the vector by
        :return: Divided/scaled vector
        """

        return Vector(self._x // scalar, self._y // scalar)

    def __eq__(self, other: Vector) -> bool:
        """
        Checks if the passed vector is equal to this vector
        :param other: Other vector to compare to
        :return: True if both vectors are equal, false otherwise
        """

        return (self._x, self._y) == (other._x, other._y)

    def __lt__(self, other: Vector) -> bool:
        """
        Checks if the passed vector is less than this vector
        Order is checked by row first, then if the rows are equal, by column
        :param other: Other vector to compare to
        :return: True if this vectors is less than the other, false otherwise
        """

        if self._y == other._y:
            return self._x < other._x
        return self._y < other._y

    def __gt__(self, other: Vector) -> bool:
        """
        Checks if the passed vector is greater than this vector
        Order is checked by row first, then if the rows are equal, by column
        :param other: Other vector to compare to
        :return: True if this vectors is greater than the other, false otherwise
        """

        if self._y == other._y:
            return self._x > other._x
        return self._y > other._y

    def __abs__(self) -> Vector:
        """
        Returns the absolute value of the Vector
        :return: Absolute value of the Vector
        """

        return Vector(abs(self._x), abs(self._y))

    def __round__(self, ndigits: int = 0) -> Vector:
        """
        Rounds the vector's values
        :param ndigits: Digits to round the components to
        :return: The rounded vector
        """

        return Vector(round(self._x, ndigits), round(self._y, ndigits))

    def __len__(self) -> float:
        """
        Calculates the magnitude of the vector
        :return: Length/magnitude of the vector
        """

        return math.sqrt((self._x ** 2) + (self._y ** 2))

    def __str__(self) -> str:
        """
        String representation of the vector
        :return: Nicely formatted string of the vector
        """

        return f"({self._x}, {self._y})"
    # endregion


class Direction(Vector, Enum):
    """
    Direction enum
    """

    # region Enum members
    UP = 0, -1
    DOWN = 0, 1
    RIGHT = 1, 0
    LEFT = -1, 0
    NONE = 0, 0
    # endregion

    # region Methods
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
    # endregion
