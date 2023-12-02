import sys
from typing import List, Set, Tuple, Final
from utils import IntcodeComp, Grid, Vector, Direction
from threading import Thread
import time


def main(args: List[str]) -> None:
    """
    Application entry point
    :param args: Argument list, should contain the file to load at index 1
    """

    # File read stub
    with open(args[1], "r") as f:
        comp: IntcodeComp = IntcodeComp(f.readline().strip(), threaded=True)

    _, painted = paint_hull(comp, False)
    print(painted)

    hull, _ = paint_hull(comp, True)
    print(hull)


def paint_hull(robot: IntcodeComp, start_white: bool) -> Tuple[Grid, int]:
    """
    Uses the specified robot to paint the hull of the ship
    :param robot: The Intcode programmed robot that paints the ship
    :param start_white: If the robot should begin on a white tile or black tile
    :return: A Tuple containing the resulting hull and amount of painted tiles
    """

    # Setup
    size: Final[int] = 150
    black: Final[str] = " "
    white: Final[str] = "▓"
    painted: Set[Vector] = set()
    direction: Direction = Direction.UP

    # Create hull
    position: Vector = Vector(size // 2, size // 2)
    hull: Grid[str] = Grid(size + 1, size + 1, black)
    # Set the starting tile to white if asked
    if start_white:
        hull[position] = white

    # Start the thread that the robot operates in
    thread: Thread = Thread(target=robot.run_program)
    thread.start()

    # Loop until the robot is done
    while True:
        # Input the current tile
        robot.add_input(int(hull[position] == white))
        # Wait for the output buffer to fill out (or the robot to be done)
        while thread.is_alive() and len(robot.output_buffer) < 2:
            time.sleep(1E-6)

        # If the robot shuts off, we are done
        if not thread.is_alive():
            break

        # Paint the new position accordingly
        hull[position] = white if bool(robot.next_output()) else black
        painted.add(position)
        # Move in the correct direction
        if bool(robot.next_output()):
            direction = direction.turn_right()
        else:
            direction = direction.turn_left()
        position = position.move_towards(direction)

    # Join the robot thread and return
    thread.join()
    return hull, len(painted)


# Only run if entry point
if __name__ == "__main__":
    main(sys.argv)
