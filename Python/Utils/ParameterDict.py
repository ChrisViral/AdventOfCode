from typing import Dict, Callable, Generic, TypeVar

# Type parameters
_TKey = TypeVar("_TKey")
_TValue = TypeVar("_TValue")


class ParameterDict(Generic[_TKey, _TValue], Dict):
    """
    A default dict implementations that takes the missing key as a parameter for the factory
    """
    def __init__(self, factory: Callable[[_TKey], _TValue]) -> None:
        """
        Creates a new ParameterDict
        :param factory: The factory to create new values on missing keys
        """
        super().__init__()
        self._factory: Callable[[_TKey], _TValue] = factory

    def __missing__(self, key: _TKey) -> _TValue:
        """
        Handles missing values is the dict000
        :param key: The key that is missing
        :return: The new created value
        """
        value: _TValue = self._factory(key)
        self[key] = value
        return value