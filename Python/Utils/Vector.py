from __future__ import annotations
from dataclasses import dataclass
from typing import Union
import math


@dataclass
class Vector:
    """
    An object representing a 2D vector
    """

    _x: Union[int, float] = 0
    _y: Union[int, float] = 0

    @property
    def x(self) -> int:
        """
        The X position of the vector
        :return: X component
        """

        return self._x

    @property
    def y(self) -> int:
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

    @staticmethod
    def dot(a: Vector, b: Vector) -> Union[int, float]:
        """
        Returns the dot product between two vectors
        :param a: First vector
        :param b: Second vector
        :return: The dot product of both vectors
        """

        return (a._x * b._x) + (a._y * b._y)

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

    def __mul__(self, scalar: Union[int, float]) -> Vector:
        """
        Multiplies a vector by a scalar
        :param scalar: Scalar to multiply the vector by
        :return: Multiplied/scaled vector
        """

        return Vector(self._x * scalar, self._y * scalar)

    def __truediv__(self, scalar: Union[int, float]) -> Vector:
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
